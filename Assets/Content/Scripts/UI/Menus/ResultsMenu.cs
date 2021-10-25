using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace Game
{
    public class ResultsMenu : Menu
    {
        [SerializeField] private TMP_Text winnerText; 
        
        public override void OnMenuOpened()
        {
            base.OnMenuOpened();
            winnerText.text = $"{GameController.WinningPlayer.NickName} \n Wins!"; 
        }

        public void LeaveButtonPressed()
        {
            PhotonNetwork.LeaveRoom(); 
        }
    }
}
