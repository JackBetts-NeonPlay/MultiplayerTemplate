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
        [SerializeField] private TMP_Text infoText;
        [SerializeField] private TMP_Text[] playerNames; 

        public override void OnEnable()
        {
            base.OnEnable();
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }

        public override void OnJoinedRoom()
        {
            OpenInLobbyUi();
            base.OnJoinedRoom();
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
            infoText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers} \n Players"; 
        }
    }
}
