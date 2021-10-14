using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
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
    public class GameController : MonoBehaviourPun
    {
        public static Action<GameState> OnGameStateChanged;
        public static Action AllPlayersReady; 
        
        [SerializeField] private GameObject playerPrefab;

        private int _playersReady;
        
        private GameState _state;
        public GameState State => _state;

        private void OnEnable()
        {
            CountdownTimer.OnCountdownEnded += OnCountdownEnded; 
        }

        private void OnDisable()
        {
            CountdownTimer.OnCountdownEnded -= OnCountdownEnded; 
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

        private void Update()
        {
            
        }

        private void SpawnPlayer()
        {
            if (playerPrefab == null) return;
            PhotonNetwork.Instantiate(nameof(playerPrefab), Vector3.zero, Quaternion.identity); 
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

        public void EndGame()
        {
            ChangeGameState(GameState.End);
        }
        
    }
}
