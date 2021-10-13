using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; 
using UnityEngine.UI; 

namespace Game
{
    public class MainMenu : Menu
    {
        [SerializeField] private Button startButton;

        private void Update()
        {
            startButton.interactable = PhotonNetwork.IsConnectedAndReady;
        }

        public void PlayButtonPressed()
        {
            NetworkManager.Instance.JoinGame();
        }

        public void OnNicknameSubmit(string nickname)
        {
            NetworkManager.Instance.SetPlayerNickname(nickname);
        }
    }
}
