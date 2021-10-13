using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using UnityEngine;

public class TextChatManager : MonoBehaviourPunCallbacks, IChatClientListener
{
    public static Action<string> MessageReceived;

    private ChatClient _chatClient;
    private string _currentChannelName;
    
    private bool _connected;
    public bool Connected => _connected; 
    
    public override void OnEnable()
    {
        base.OnEnable();
        ChatLobby.SentMessage += OnChatLobbySentMessage; 
    }

    public override void OnDisable()
    {
        base.OnDisable();
        ChatLobby.SentMessage -= OnChatLobbySentMessage; 
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _chatClient = new ChatClient(this)
        {
            ChatRegion = "EU"
        };
        
        _chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1",
            new AuthenticationValues(PhotonNetwork.LocalPlayer.UserId)); 
    }

    private void Update()
    { 
        _chatClient.Service();
    }

    private void OnChatLobbySentMessage(string message)
    {
        SendMessage(PhotonNetwork.NickName, message);
    }

    public void SendMessage(string nickname, string message)
    {
        if (!_connected) return;
        _chatClient.PublishMessage(_currentChannelName, $"<color=blue>{nickname}:</color> {message}"); 
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        _currentChannelName = PhotonNetwork.CurrentRoom.Name; 
        _chatClient.Subscribe(PhotonNetwork.CurrentRoom.Name); 
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        _chatClient.Unsubscribe(new[]{_currentChannelName}); 
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        MessageReceived?.Invoke(messages[0].ToString());
    }
    
    public void OnDisconnected()
    {
        _connected = false; 
        Debug.Log($"{this} : DISCONNECTED TO CHAT CLIENT");
    }

    public void OnConnected()
    {
        _connected = true;
        Debug.Log($"{this} : CONNECTED TO CHAT CLIENT");
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        
    }

    #region Unused Chat Callbacks

    public void DebugReturn(DebugLevel level, string message)
    {
        
    }

    public void OnChatStateChange(ChatState state)
    {
        
    }
    public void OnSubscribed(string[] channels, bool[] results)
    {
        
    }

    public void OnUnsubscribed(string[] channels)
    {
        
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        
    }

    public void OnUserSubscribed(string channel, string user)
    {
        
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        
    }

    #endregion
}
