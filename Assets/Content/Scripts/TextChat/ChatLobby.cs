using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class ChatLobby : MonoBehaviour
{
    public static Action<string> SentMessage;

    [SerializeField] private bool dontDestroyOnLoad;
    [SerializeField] private TMP_InputField inputField; 
    [SerializeField] private TMP_Text lobbyChat;

    private void OnEnable()
    {
        TextChatManager.MessageReceived += OnReceivedMessage; 
    }

    private void OnDisable()
    {
        TextChatManager.MessageReceived -= OnReceivedMessage;
    }

    private void Awake()
    {
        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SendCurrentMessage()
    {
        if (inputField.text == String.Empty) return; 
        
        string message = inputField.text;
        inputField.text = String.Empty;
        SentMessage?.Invoke(message);
    }

    private void OnReceivedMessage(string message)
    {
        lobbyChat.text += $"\n{message}"; 
    }
}
