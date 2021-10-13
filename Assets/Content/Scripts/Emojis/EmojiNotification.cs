using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Game
{
    public class EmojiNotification : MonoBehaviour
    {
        [SerializeField] private TMP_Text nicknameText;
        [SerializeField] private Image emojiImage;
        [SerializeField] private float notificationTime; 
        
        private readonly int _kShow = Animator.StringToHash("Show");
        private readonly int _kHide = Animator.StringToHash("Hide");

        public bool CanNotify => _canNotify; 
        private bool _canNotify; 
        
        private Animator _anim;

        private void Start()
        {
            _anim = GetComponent<Animator>();
            _canNotify = true; 
        }

        public void StartNotification(string nickname, Sprite emoji)
        {
            if (!_canNotify) return;

            _canNotify = false; 
            nicknameText.text = nickname;
            emojiImage.sprite = emoji;
            _anim.SetTrigger(_kShow);
            
            StartCoroutine(CoDisableNotification()); 
        }

        private IEnumerator CoDisableNotification()
        {
            yield return new WaitForSeconds(notificationTime); 
            _anim.SetTrigger(_kHide);
            _canNotify = true; 
        }
    }
}
