using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game
{
    public class EmojiUiSelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image emojiImage;
        [SerializeField] private Vector3 lerpUpScale;
        [SerializeField] private float lerpScaleDuration; 

        private int _emojiIndex;
        private Sprite _emojiSprite; 
        private SendEmojiButton _sendButton;

        public void Setup(SendEmojiButton sendButton, int emojiIndex, Sprite emojiSprite)
        {
            _sendButton = sendButton;
            _emojiIndex = emojiIndex;
            _emojiSprite = emojiSprite; 
            
            SetSprite();
        }

        private void SetSprite()
        {
            emojiImage.sprite = _emojiSprite; 
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            StopAllCoroutines();
            StartCoroutine(LerpScale(lerpUpScale, lerpScaleDuration)); 
            _sendButton.SetCurrentEmoji(_emojiIndex);
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            StopAllCoroutines();
            StartCoroutine(LerpScale(Vector3.one, lerpScaleDuration)); 
        }

        private IEnumerator LerpScale(Vector3 targetScale, float duration)
        {
            float timer = 0;
            Vector3 startScale = transform.localScale; 
            
            while (timer < duration)
            {
                transform.localScale = Vector3.Lerp(startScale, targetScale, timer / duration);  
                timer += Time.deltaTime; 
                yield return null;
            }

            transform.localScale = targetScale; 
        }
    }
}
