using System.Collections;
using System.Collections.Generic;
using Game;
using Photon.Pun;
using UnityEngine;

public abstract class SingletonPunBehaviour<T> : MonoBehaviourPunCallbacks
{
    private static T _Instance;

    public static T Instance
    {
        get { return _Instance; }
    }

    [SerializeField] private bool dontDestroyOnLoad = true;

    public virtual void Awake()
    {
        if (Instance == null)
        {
            _Instance = gameObject.GetComponent<T>();
        }
        else
        {
            Destroy(gameObject);
        }

        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }

        if (photonView != null)
        {
            photonView.ViewID = 0; 
            photonView.ViewID = PhotonViewIdSelector.GetId(); 
        }
    }
}
