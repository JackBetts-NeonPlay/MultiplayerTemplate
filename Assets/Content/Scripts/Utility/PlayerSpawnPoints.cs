using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PlayerSpawnPoints : MonoBehaviour
    {
        [SerializeField] private Transform[] spawnPoints;

        public Transform GetSpawnPoint(int playerIndex)
        {
            if (playerIndex >= spawnPoints.Length)
            {
                return spawnPoints[0]; 
            }

            return spawnPoints[playerIndex]; 
        }
    }
}
