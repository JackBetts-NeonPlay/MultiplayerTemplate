using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI; 

namespace Game
{
    public class MainMenu : Menu
    {
        [SerializeField] private TMP_InputField nicknameInput;

        public override void OnMenuOpened()
        {
            nicknameInput.text = PhotonNetwork.NickName; 
            base.OnMenuOpened();
        }

        public void PlayButtonPressed()
        {
            if (!PhotonNetwork.IsConnectedAndReady)
            {
                //Modal Panel saying not yet connected
                return; 
            }
            
            NetworkManager.Instance.JoinGame();
        }

        public void PlayWithFriendsPressed()
        {
            if (!PhotonNetwork.IsConnectedAndReady)
            {
                //Modal Panel saying not yet connected
                return; 
            }
            
            UiManager.Instance.OpenMenu("PlayWithFriends");
        }

        public void OnNicknameSubmit(string nickname)
        {
            NetworkManager.Instance.SetPlayerNickname(nickname);
        }
    }
}
