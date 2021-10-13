using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    public class SendEmojiButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private EmojiController emojiController;
        [SerializeField] private Image emojiImage;
        [SerializeField] private EmojiUiSelector[] selectors; 
        
        [Header("Cooldown")]
        [SerializeField] private float cooldown = 3f;
        [SerializeField] private Image cooldownSpinner; 
        
        private readonly int _kShow = Animator.StringToHash("Show");
        private readonly int _kHide = Animator.StringToHash("Hide");

        private Animator _anim;
        private RectTransform _rect;

        private bool _canPress = true;  
        private int _currentEmojiIndex;
        private bool _showingSelectors;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>(); 
            _anim = GetComponent<Animator>(); 
        }

        private void Start()
        {
            SetupSelectors();
            UpdateCurrentEmoji();
            SetSelectorsEnabled(false);
        }

        private void SetupSelectors()
        {
            for (int i = 0; i < selectors.Length; i++)
            {
                Sprite sprite = emojiController.GetEmojiSpriteByIndex(i);
                if (sprite != null)
                {
                    selectors[i].Setup(this, i, sprite);
                }
            }
        }

        public void SetCurrentEmoji(int index)
        {
            _currentEmojiIndex = index;
            UpdateCurrentEmoji();
        }
        
        public void SendEmoji()
        {
            if (!PhotonNetwork.IsConnectedAndReady || !PhotonNetwork.InRoom) return;
            emojiController.SendEmoji(PhotonNetwork.NickName, _currentEmojiIndex);

            if (cooldown > 0)
            {
                StartCoroutine(CoCooldown()); 
            }
        }

        private void UpdateCurrentEmoji()
        {
            emojiImage.sprite = emojiController.GetEmojiSpriteByIndex(_currentEmojiIndex); 
        }

        public void SetSelectorsEnabled(bool enabled)
        {
            _showingSelectors = enabled; 
            _anim.SetTrigger(enabled ? _kShow : _kHide);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_canPress) return; 
            if (_showingSelectors)
            {
                SendEmoji();    
            }
            
            SetSelectorsEnabled(!_showingSelectors);
        }


        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_canPress) return; 
            if (!_showingSelectors) return; 
            if (RectTransformUtility.RectangleContainsScreenPoint(_rect, eventData.position)) return; 
            
            SendEmoji();
            SetSelectorsEnabled(false);
        }

        IEnumerator CoCooldown()
        {
            _canPress = false;
            float timer = 0;

            while (timer < cooldown)
            {
                timer += Time.deltaTime; 
                cooldownSpinner.fillAmount = Mathf.Lerp(1, 0, timer / cooldown); 
                yield return null; 
            }

            _canPress = true; 
        }
    }
}
