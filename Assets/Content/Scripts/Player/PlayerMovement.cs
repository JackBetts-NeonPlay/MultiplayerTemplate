using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Game
{
    public class PlayerMovement : MonoBehaviourPun
    {
        [SerializeField] private float speed, jumpHeight, gravityValue;

        private Vector3 _playerVelocity; 
        private bool _canMove = false;
        private bool _isGrounded = false; 

        private CharacterController _controller;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>(); 
        }

        public void SetCanMove(bool canMove)
        {
            _canMove = canMove; 
        }

        private void Update()
        {
            if (_canMove && photonView.IsMine)
            {
                HandleMovement();
            }
        }

        private void HandleMovement()
        {
            _isGrounded = _controller.isGrounded;
            if (_isGrounded && _playerVelocity.y < 0)
            {
                _playerVelocity.y = 0;
            }
            
            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            _controller.Move(move * Time.deltaTime * speed);

            if (move != Vector3.zero)
            {
                transform.forward = move;
            }
            
            if (Input.GetButtonDown("Jump") && _isGrounded)
            {
                _playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            }

            _playerVelocity.y += gravityValue * Time.deltaTime;
            _controller.Move(_playerVelocity * Time.deltaTime);
        }
    }
}
