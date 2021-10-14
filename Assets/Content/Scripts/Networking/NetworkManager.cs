using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

namespace Game 
{
    [RequireComponent(typeof(PhotonView))]
    public class NetworkManager : SingletonPunBehaviour<NetworkManager>
    {
        public static Action StartedSearchingForGame;
        public static Action StartedCreatingGame;
        public static Action JoinGameFailed;
        public static Action JoinGameSuccess;
        public static Action LoadingGameScene;
        public static Action LoadingMainMenu;

        public static string K_IsPrivateKey = "isPrivateGame";
        public static string K_RoomCodeKey = "roomCode";  
        public static string K_CountdownKey = "countdown";  

        [Header("Matchmaking Settings")] 
        [SerializeField] private byte desiredRoomPlayers;
        public int lobbyCountdown; 
        public string gameSceneName, menuSceneName;

        private bool _attemptingJoinPrivateGame; 

        private const string KNicknameKey = "nickname"; 
        
        public int GetPlayersTeam(Player player)
        {
            if (PhotonNetwork.IsConnected && PhotonNetwork.CurrentRoom != null)
            {
                return (int)player.CustomProperties[PhotonTeamsManager.TeamPlayerProp];   
            }

            return -1; 
        }

        public int MyTeam()
        {
            if (PhotonNetwork.IsConnected && PhotonNetwork.LocalPlayer != null)
            {
                return (int)PhotonNetwork.LocalPlayer.CustomProperties[PhotonTeamsManager.TeamPlayerProp];
            }

            return -1; 
        }
        
        private void Start()
        {
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.NickName = PlayerPrefs.GetString(KNicknameKey, "Nickname"); 
            
            StartCoroutine(CoLoadMainMenu()); 
        }

        private IEnumerator CoLoadMainMenu()
        {
            yield return new WaitForSeconds(3); 
            SceneManager.LoadScene(menuSceneName);
        }

        public void SetPlayerNickname(string nickname)
        {
            PlayerPrefs.SetString(KNicknameKey, nickname);
            PhotonNetwork.NickName = nickname; 
        }

        public void JoinGame()
        {
            UiManager.Instance.OpenMenu("Lobby");
            
            StartedSearchingForGame?.Invoke();
            PhotonNetwork.JoinRandomRoom();
        }

        public void JoinGameWithCode(string gameCode)
        {
            _attemptingJoinPrivateGame = true; 
            PhotonNetwork.JoinRoom(gameCode); 
            UiManager.Instance.OpenMenu("Lobby");
        }

        public void CreateCodedGame()
        {
            StartedCreatingGame?.Invoke();
            string randomCode = GenerateRandomCode(4);

            Hashtable roomProps = new Hashtable()
            {
                { K_IsPrivateKey , true },
                { K_RoomCodeKey, randomCode },
                { K_CountdownKey, lobbyCountdown }
            };
            
            RoomOptions options = new RoomOptions
            {
                IsVisible = false,
                IsOpen = true,
                MaxPlayers = desiredRoomPlayers,
                CustomRoomProperties = roomProps
            };
            
            Debug.Log($"{this} : Creating Private Game with code - {randomCode}");
            PhotonNetwork.CreateRoom(randomCode, options); 
            UiManager.Instance.OpenMenu("Lobby");
        }

        private string _codeCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"; 
        private string GenerateRandomCode(int codeLength)
        {
            string code = "";
            for (int i = 0; i < codeLength; i++)
            {
                code += _codeCharacters[Random.Range(0, _codeCharacters.Length)]; 
            }

            return code; 
        }

        public void CreateGame()
        {
            StartedCreatingGame?.Invoke();
            
            Hashtable roomProps = new Hashtable()
            {
                { K_IsPrivateKey , false },
                { K_CountdownKey, lobbyCountdown }
            };
            
            RoomOptions options = new RoomOptions
            {
                IsVisible = false,
                IsOpen = true,
                MaxPlayers = desiredRoomPlayers,
                CustomRoomProperties = roomProps
            };

            PhotonNetwork.CreateRoom(null, options);
        }

        public void OnRoomFilled()
        {
            Debug.Log($"{this} - Room Filled, master client will start game");
            
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError( $"{this} - Trying to Load Game Scene on non master client");
                return;
            }

            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;
            
            LoadGameScene();
        }

        public void SetupPlayerTeams()
        {
            Player[] players = PhotonNetwork.PlayerList;

            for(int i = 0; i < players.Length; i++)
            {
                Hashtable prop = new Hashtable(); 
                prop.Add(PhotonTeamsManager.TeamPlayerProp, i);
                
                bool set = players[i].SetCustomProperties(prop);
                Debug.Log($"{this} - Custom Props for Player {i} Result - {set}");
            }
        }

        public void LoadGameScene()
        {
            photonView.RPC(nameof(ClientLoadingGameScene), RpcTarget.Others);
            LoadingGameScene?.Invoke();
            PhotonNetwork.LoadLevel(gameSceneName);
        }

        [PunRPC]
        public void ClientLoadingGameScene()
        {
            LoadingGameScene?.Invoke();
        }

        public void LoadMainMenu()
        {
            LoadingMainMenu?.Invoke();
            SceneManager.LoadScene(menuSceneName); 
        }

        private bool IsInGameScene => SceneManager.GetActiveScene().name == gameSceneName; 


        #region Pun Callbacks

        public override void OnConnectedToMaster()
        {
            Debug.Log($"{this} - Connected to master");
            base.OnConnectedToMaster();
        }

        public override void OnJoinedRoom()
        {
            Debug.Log($"{this} - Join Room Success! - {PhotonNetwork.CurrentRoom.Name}");
            JoinGameSuccess?.Invoke();

            if (PhotonNetwork.IsMasterClient)
            {
                SetupPlayerTeams();
            }

            _attemptingJoinPrivateGame = false; 

            base.OnJoinedRoom();
        }

        public override void OnCreatedRoom()
        {
            Debug.Log($"{this} - Create Room Success! - {PhotonNetwork.CurrentRoom.Name}");
            base.OnCreatedRoom();
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            if (_attemptingJoinPrivateGame)
            {
                Debug.Log($"{this} : Failed to join room with code");
                _attemptingJoinPrivateGame = false; 
                UiManager.Instance.OpenMenu("PlayWithFriends");
                return; 
            }

            Debug.Log($"{this} - Join Room Failed - Will attempt creating room");
            CreateGame();
            
            base.OnJoinRoomFailed(returnCode, message);
        }
        
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log($"{this} - Join Room Failed - Will attempt creating room");
            CreateGame();
            
            base.OnJoinRandomFailed(returnCode, message);
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.Log($"{this} - Create Room Failed - Will attempt creating room");
            JoinGameFailed?.Invoke();
            
            base.OnCreateRoomFailed(returnCode, message);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                SetupPlayerTeams();
            }
            
            if (PhotonNetwork.CurrentRoom.PlayerCount == desiredRoomPlayers && PhotonNetwork.IsMasterClient)
            {
                OnRoomFilled();
            }
            
            base.OnPlayerEnteredRoom(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);

            if (PhotonNetwork.CurrentRoom.PlayerCount <= 1)
            {
                PhotonNetwork.LeaveRoom(); 
            }
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            
            if (IsInGameScene)
            {
                LoadMainMenu();
            }
            else
            {
                UiManager.Instance.OpenMenu("Main");
            }
        }

        #endregion
    }
}
