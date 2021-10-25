using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Game
{
    public enum GameState
    {
        WaitingForPlayers, 
        Playing,
        End
    }
    
    [RequireComponent(typeof(PhotonView))]
    public class GameController : MonoBehaviourPunCallbacks
    {
        public static Action<GameState> OnGameStateChanged;
        public static Action AllPlayersReady;

        public static Player WinningPlayer; 
        
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private PlayerSpawnPoints spawnPoints; 

        private int _playersReady;
        
        private GameState _state;
        public GameState State => _state;

        public override void OnEnable()
        {
            base.OnEnable();
            CountdownTimer.OnCountdownEnded += OnCountdownEnded;
            PlayerCollisions.ReachedEnd += OnPlayerReachedEnd; 
        }

        public override void OnDisable()
        {
            base.OnDisable();
            CountdownTimer.OnCountdownEnded -= OnCountdownEnded; 
            PlayerCollisions.ReachedEnd -= OnPlayerReachedEnd;
        }

        private void Start()
        {
            SpawnPlayer();
            ChangeGameState(GameState.WaitingForPlayers);
            
            if (!PhotonNetwork.IsMasterClient)
            {
                photonView.RPC(nameof(OtherPlayerReady), RpcTarget.OthersBuffered);
            }
            else
            {
                _playersReady++; 
            }
        }

        private void SpawnPlayer()
        {
            if (playerPrefab == null) return;
            int playerNum = (int)PhotonNetwork.LocalPlayer.CustomProperties[NetworkManager.K_SpawnPointKey];

            Debug.Log($"Spawn Point {playerNum}");
            
            GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoints.GetSpawnPoint(playerNum).position, Quaternion.identity); 
            CameraController.Instance.SetNewTarget(player.transform);
        }

        private void ChangeGameState(GameState state)
        {
            Debug.Log($"{this} : Changing Game State -- {state.ToString()}");
            _state = state; 
            OnGameStateChanged?.Invoke(state);
        }
        
        private void OnCountdownEnded()
        {
            ChangeGameState(GameState.Playing);
        }

        [PunRPC]
        private void OtherPlayerReady()
        {
            _playersReady++;

            if (_playersReady == PhotonNetwork.CurrentRoom.MaxPlayers)
            {
                photonView.RPC(nameof(PlayersReady), RpcTarget.All);
            }
        } 

        [PunRPC]
        private void PlayersReady()
        {
            AllPlayersReady?.Invoke();
        }

        private void OnPlayerReachedEnd(Player player)
        {
            EndGame(player);
        }

        public void EndGame(Player wonPlayer)
        {
            WinningPlayer = wonPlayer; 
            ChangeGameState(GameState.End);
            photonView.RPC(nameof(RpcEndGame), RpcTarget.Others, wonPlayer);
        }

        [PunRPC]
        private void RpcEndGame(Player wonPlayer)
        {
            Debug.Log($"Ending game because player {wonPlayer.NickName} has Won!");
            WinningPlayer = wonPlayer;
            ChangeGameState(GameState.End);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            if (PhotonNetwork.CurrentRoom.PlayerCount < 2)
            {
                EndGame(PhotonNetwork.LocalPlayer);
            }
        }
    }
}
