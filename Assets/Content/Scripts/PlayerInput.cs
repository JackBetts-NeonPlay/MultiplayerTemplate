using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


namespace Game
{
    public class PlayerInput : MonoBehaviourPun
    {
        public delegate void TouchDown(Vector2 pos);
        public static event TouchDown OnTouchDown;

        public delegate void TouchUp(Vector2 pos);
        public static event TouchUp OnTouchUp;

        public bool isTouching { get; private set; }

        public delegate void Tap();
        public static event Tap OnTap;
        [SerializeField] private float maxTapTime;
        [SerializeField] private Vector2 minTapCancelDist;
        [SerializeField] private float swipeThreshold;

        public static Action<Vector2> Swipe;


        private Vector2 _touchDelta;
        public Vector2 TouchDelta => _touchDelta;

        private Vector2 _scaledTouchDelta;
        public Vector2 ScaledTouchDelta => _scaledTouchDelta;

        private Vector2 _axis;
        public Vector2 Axis => _axis;

        private Vector2 _lastMousePos;
        private Vector2 _touchDownPos;
        private Vector2 _touchUpPos;
        private float _touchDownTime;
        private bool _cancelTap;


        private void Update()
        {
            if (photonView.IsMine == false) return; 

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_STANDALONE_OSX
            CheckGesturesEditor();
#else
            CheckGesturesMobile();
#endif
        }

        private void CheckGesturesMobile()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    _touchDownPos = touch.position;
                    OnTouchDown?.Invoke(touch.position);
                    _touchDownTime = Time.time;
                    isTouching = true;
                }

                if (touch.phase == TouchPhase.Moved)
                {
                    _touchUpPos = touch.position;
                    Vector2 dragDistanceUnscaled = touch.deltaPosition;

                    _scaledTouchDelta = new Vector2(dragDistanceUnscaled.x / Screen.width,
                        dragDistanceUnscaled.y / Screen.height);

                    float _horizontalAxis = (touch.position.x - _touchDownPos.x) / Screen.width;
                    float _verticalAxis = (touch.position.y - _touchDownPos.y) / Screen.height;
                    _axis = new Vector2(_horizontalAxis, _verticalAxis);

                    if (CheckSwipe())
                    {
                        _touchDownPos = touch.position;
                    }
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    _touchUpPos = touch.position;
                    OnTouchUp?.Invoke(_touchUpPos);
                    isTouching = false;
                    if (DidTap(touch.position))
                    {
                        OnTap?.Invoke();
                    }

                    //CheckSwipe();
                }
            }
            else
            {
                _touchDelta = Vector2.zero;
                _scaledTouchDelta = Vector2.zero;
                _axis = Vector2.zero;
            }
        }


        private void CheckGesturesEditor()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePos = Input.mousePosition;
                _lastMousePos = mousePos;
                _touchDownPos = mousePos;
                _touchDownTime = Time.time;
                isTouching = true;
                OnTouchDown?.Invoke(mousePos);
            }

            if (Input.GetMouseButton(0))
            {
                Vector2 mousePos = Input.mousePosition;
                _touchUpPos = mousePos;
                _touchDelta = mousePos - _lastMousePos;
                _scaledTouchDelta = new Vector2(_touchDelta.x / Screen.width,
                    _touchDelta.y / Screen.height);

                _lastMousePos = mousePos;

                float _horizontalAxis = (mousePos.x - _touchDownPos.x) / Screen.width;
                float _verticalAxis = (mousePos.y - _touchDownPos.y) / Screen.height;

                _axis = new Vector2(_horizontalAxis, _verticalAxis);

                if (CheckSwipe())
                {
                    _touchDownPos = mousePos;
                }
            }
            else
            {
                _touchDelta = Vector2.zero;
                _scaledTouchDelta = Vector2.zero;
                _axis = Vector2.zero;
            }

            if (Input.GetMouseButtonUp(0))
            {
                _touchUpPos = Input.mousePosition;
                OnTouchUp?.Invoke(_touchUpPos);
                isTouching = false;

                if (DidTap(Input.mousePosition))
                {
                    OnTap?.Invoke();
                }

                //CheckSwipe();
            }
        }

        private bool DidTap(Vector2 touchPos)
        {
            return Time.time < (_touchDownTime + maxTapTime) &&
                (touchPos.x - _touchDownPos.x) < minTapCancelDist.x &&
                (touchPos.y - _touchDownPos.y) < minTapCancelDist.y;
        }

        private bool CheckSwipe()
        {
            if (VerticalMove() > swipeThreshold && VerticalMove() > HorizontalMove())
            {
                if (_touchDownPos.y - _touchUpPos.y > 0)
                {
                    Swipe?.Invoke(new Vector2(0, -1));
                    return true;
                }
                else
                {
                    Swipe?.Invoke(new Vector2(0, 1));
                    return true;
                }
            }
            else if (HorizontalMove() > swipeThreshold && HorizontalMove() > VerticalMove())
            {
                if (_touchDownPos.x - _touchUpPos.x > 0)
                {
                    Swipe?.Invoke(new Vector2(-1, 0));
                    return true;
                }
                else
                {
                    Swipe?.Invoke(new Vector2(1, 0));
                    return true;
                }
            }

            return false;
        }

        private float VerticalMove()
        {
            return Mathf.Abs(_touchUpPos.y - _touchDownPos.y);
        }

        private float HorizontalMove()
        {
            return Mathf.Abs(_touchUpPos.x - _touchDownPos.x);
        }
    }
}