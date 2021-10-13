using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game
{
    public class PlayWithFriendsMenu : Menu
    {
        [SerializeField] private TMP_InputField codeField;

        public void OnJoinWithCodePressed()
        {
            if (codeField.text == String.Empty)
            {
                //Could bring up a modal panel here "Please Input Code"
                return;
            }
            
            NetworkManager.Instance.JoinGameWithCode(codeField.text);
        }

        public void OnCreateRoomPressed()
        {
            NetworkManager.Instance.CreateCodedGame();
        }

        public void ExitButtonPressed()
        {
            UiManager.Instance.OpenMenu("Main");
        }
    }
}
