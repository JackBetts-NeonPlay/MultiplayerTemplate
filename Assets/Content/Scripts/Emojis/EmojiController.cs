using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(PhotonView))]
    public class EmojiController : MonoBehaviourPun
    {
        public static Action<string, Sprite> DisplayEmoji;

        [SerializeField] private bool dontDestroyOnLoad; 
        [SerializeField] private Sprite[] allEmojis;
        
        private void Awake()
        {
            if(dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }

        public void SendEmoji(string myNickname, int emojiId)
        {
            if (emojiId >= allEmojis.Length)
            {
                Debug.LogError($"Emoji List is not long enough to contain ID - {emojiId}");
                return; 
            }

            Debug.Log($"{this} - Sending Emoji");
            photonView.RPC(nameof(EmojiReceived), RpcTarget.Others, myNickname, emojiId);
            DisplayEmoji?.Invoke(myNickname, allEmojis[emojiId]);
        }

        [PunRPC]
        private void EmojiReceived(string nickname, int id)
        {
            DisplayEmoji?.Invoke(nickname, allEmojis[id]);
        }

        public Sprite GetEmojiSpriteByIndex(int index)
        {
            if (index >= allEmojis.Length) return null;
            return allEmojis[index]; 
        }
    }
}
