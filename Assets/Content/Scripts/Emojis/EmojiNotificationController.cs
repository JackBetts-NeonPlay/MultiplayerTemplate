using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class EmojiNotificationController : EmojiListener
    {
        [SerializeField] private EmojiNotification[] notificationPanels; 
        
        protected override void OnDisplayEmoji(string nickname, Sprite emojiSprite)
        {
            if (HasFreePanel(out EmojiNotification panel))
            {
                panel.StartNotification(nickname, emojiSprite);
            }
        }

        private bool HasFreePanel(out EmojiNotification panel)
        {
            foreach (EmojiNotification p in notificationPanels)
            {
                if (!p.CanNotify) continue;
                panel = p;
                return true;
            }

            panel = null; 
            return false; 
        }
    }
}
