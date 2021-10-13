using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PhotonViewIdSelector : MonoBehaviour
    {
        private static int _id = 999;

        public static int GetId()
        {
            _id--;
            return _id; 
        }
    }
}
