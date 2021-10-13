using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoiceToggleButton : MonoBehaviour
{
    public static Action ToggleSelfMuted;
    public static Action ToggleOthersMuted; 

    [SerializeField] private bool toggleSelfMuted;
    [SerializeField] private Image micImage;
    [SerializeField] private Sprite micOnSprite, micOffSprite;
    
    private void Start()
    {
        SetSprites();
    }

    public void ToggleVoice()
    {
        if (toggleSelfMuted)
        {
            ToggleSelfMuted?.Invoke();
        }
        else
        {
            ToggleOthersMuted?.Invoke();
        }
        
        SetSprites();
    }

    private void SetSprites()
    {
        if (!micImage) return;
        if (toggleSelfMuted)
        {
            micImage.sprite = VoiceController.SelfMuted ? micOffSprite : micOnSprite;
        }
        else
        {
            micImage.sprite = VoiceController.OthersMuted ? micOffSprite : micOnSprite;   
        }
    }
}
