using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PlayerController : MonoBehaviour
    {

        private PlayerMovement _movement; 
        
        private void OnEnable()
        {
            GameController.OnGameStateChanged += OnGameStateChanged; 
        }

        private void OnDisable()
        {
            GameController.OnGameStateChanged -= OnGameStateChanged; 
        }

        private void Awake()
        {
            _movement = GetComponent<PlayerMovement>(); 
        }

        private void OnGameStateChanged(GameState state)
        {
            _movement.SetCanMove(state == GameState.Playing);
        }
    }
}