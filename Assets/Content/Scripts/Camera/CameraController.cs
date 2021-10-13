using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Cinemachine; 

namespace Game
{
    public class CameraController : SingletonPunBehaviour<CameraController>
    {
        [SerializeField] private CinemachineVirtualCamera cam;

        public void SetNewTarget(Transform target)
        {
            cam.Follow = target;
            cam.LookAt = target;
        }
    }
}
