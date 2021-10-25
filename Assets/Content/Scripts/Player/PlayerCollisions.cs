using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Game
{
    public class PlayerCollisions : MonoBehaviourPun
    {
        public static Action<Player> ReachedEnd; 
        
        private const string KEndKey = "End"; 
        
        private void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine) return; 
            if (other.CompareTag(KEndKey))
            {
                ReachedEnd?.Invoke(PhotonNetwork.LocalPlayer);
            }
        }
    }
}
