using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AllChatToggleButton : MonoBehaviour
{
    public static Action AllChatToggled; 
    
    [SerializeField] private TMP_Text infoText; 
    
    public void ToggleAllChat()
    {
        AllChatToggled?.Invoke();
        infoText.text = VoiceController.TeamChat ? "Team Chat" : "All Chat"; 
    }
}
