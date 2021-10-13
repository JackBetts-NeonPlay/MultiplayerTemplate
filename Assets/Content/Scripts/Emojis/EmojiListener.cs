using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class EmojiListener : MonoBehaviour
    {
        private void OnEnable()
        {
            EmojiController.DisplayEmoji += OnDisplayEmoji; 
        }

        private void OnDisable()
        {
            EmojiController.DisplayEmoji -= OnDisplayEmoji;
        }

        protected virtual void OnDisplayEmoji(string nickname, Sprite emojiSprite) { } 
    }
}
