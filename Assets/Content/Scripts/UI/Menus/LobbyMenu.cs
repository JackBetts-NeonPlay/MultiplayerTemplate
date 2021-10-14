using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace Game
{
    public class LobbyMenu : Menu
    {
        [SerializeField] private GameObject roomCodeObject;
        [SerializeField] private TMP_Text roomCodeText; 
        [Space]
        [SerializeField] private TMP_Text infoText;
        [SerializeField] private TMP_Text[] playerNames;
        [SerializeField] private TMP_Text countdownText; 
        
        private int _countdown = 10;
        private float _lastCountdownUpdateTime; 

        private void Update()
        {
            if (PhotonNetwork.InRoom && NetworkManager.Instance.lobbyCountdown != -1)
            {
                UpdateCountdownTimer();
            }
        }

        private void UpdateCountdownTimer()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (_countdown <= 0) return; 
                if (Time.time > _lastCountdownUpdateTime + 1)
                {
                    _lastCountdownUpdateTime = Time.time;
                    _countdown--;
                    PhotonNetwork.CurrentRoom.CustomProperties[NetworkManager.K_CountdownKey] = _countdown;
                }
            }
            else
            {
                _countdown = (int) PhotonNetwork.CurrentRoom.CustomProperties[NetworkManager.K_CountdownKey]; 
            }

            countdownText.text = _countdown.ToString(); 
        }

        public override void OnJoinedRoom()
        {
            _lastCountdownUpdateTime = Time.time; 
            OpenInLobbyUi();
            CheckHasRoomCode();
            base.OnJoinedRoom();
        }

        private void CheckHasRoomCode()
        {
            bool isPrivateGame = (bool) PhotonNetwork.CurrentRoom.CustomProperties[NetworkManager.K_IsPrivateKey];
            roomCodeObject.SetActive(isPrivateGame);
            
            if (!isPrivateGame) return;

            string roomCode = (string) PhotonNetwork.CurrentRoom.CustomProperties[NetworkManager.K_RoomCodeKey];
            roomCodeText.text = roomCode;
        }

        public void CopyRoomCodePressed()
        {
            TextEditor editor = new TextEditor
            {
                text = roomCodeText.text
            }; 
            
            editor.SelectAll();
            editor.Copy();
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            UpdateLobbyUi();
            UpdatePlayerNames();
            base.OnPlayerEnteredRoom(newPlayer);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            UpdateLobbyUi();
            UpdatePlayerNames();
            base.OnPlayerLeftRoom(otherPlayer);
        }

        private void OpenInLobbyUi()
        {
            UpdateLobbyUi();
            UpdatePlayerNames();
        }

        private void UpdatePlayerNames()
        {
            for(int i = 0; i < playerNames.Length; i++)
            {
                if(PhotonNetwork.CurrentRoom.PlayerCount <= i)
                {
                    playerNames[i].text = "";
                    continue; 
                }

                playerNames[i].text = PhotonNetwork.PlayerList[i].NickName;
            }
        }

        private void UpdateLobbyUi()
        {
            infoText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}"; 
        }

        public override void OnMenuOpened()
        {
            base.OnMenuOpened();
            if (NetworkManager.Instance.lobbyCountdown != -1)
            {
                _countdown = NetworkManager.Instance.lobbyCountdown;
                countdownText.text = _countdown.ToString();
            }
            else
            {
                countdownText.enabled = false; 
            }
        }

        public override void OnMenuClosed()
        {
            base.OnMenuClosed();
            ResetLobbyUi();
        }

        private void ResetLobbyUi()
        {
            infoText.text = "0 / 0"; 
            for(int i = 0; i < playerNames.Length; i++)
            {
                playerNames[i].text = "";
            }
        }

        public void LeaveButtonPressed()
        {
            if (!PhotonNetwork.InRoom) return;
            PhotonNetwork.LeaveRoom(); 
        }
    }
}
