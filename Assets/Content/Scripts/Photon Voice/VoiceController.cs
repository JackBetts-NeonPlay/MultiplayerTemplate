using System;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;

public class VoiceController : MonoBehaviourPunCallbacks
{
    public static bool SelfMuted = false;
    public static bool OthersMuted = false;
    public static bool TeamChat = false; 

    [SerializeField] private GameObject voicePrefab; 
    [SerializeField] private Recorder recorder;
    [SerializeField] private Speaker speaker; 

    private const string KSelfMuted = "self_muted";
    private const string KOthersMuted = "others_muted";

    private GameObject _currVoicePrefab; 
    private byte[] _allByteValues = Enumerable.Range(1, 255).SelectMany(BitConverter.GetBytes).ToArray();


    public override void OnEnable()
    {
        base.OnEnable();
        VoiceToggleButton.ToggleSelfMuted += ToggleSelfMuted;
        VoiceToggleButton.ToggleOthersMuted += ToggleOthersMuted;
        AllChatToggleButton.AllChatToggled += OnAllChatButtonToggled; 
    }

    public override void OnDisable()
    {
        base.OnDisable();
        VoiceToggleButton.ToggleSelfMuted -= ToggleSelfMuted;
        VoiceToggleButton.ToggleOthersMuted -= ToggleOthersMuted;
        AllChatToggleButton.AllChatToggled -= OnAllChatButtonToggled; 
    }
        
    private void SpawnVoicePrefab()
    {
        _currVoicePrefab = PhotonNetwork.Instantiate(voicePrefab.name, Vector3.zero, Quaternion.identity); 
        _currVoicePrefab.transform.SetParent(transform);

        recorder = _currVoicePrefab.GetComponent<Recorder>();
        speaker = _currVoicePrefab.GetComponent<Speaker>();

        PhotonVoiceNetwork.Instance.PrimaryRecorder = recorder; 
        
        CheckVoiceActive();
        JoinAllVoice();
    }

    private void RemoveCurrVoice()
    {
        if (_currVoicePrefab == null) return; 
        Destroy(_currVoicePrefab);
    }

    private void OnAllChatButtonToggled()
    {
        TeamChat = !TeamChat;

        if (TeamChat)
        {
            JoinTeamVoice();
        }
        else
        {
            JoinAllVoice();
        }
    }

    public void JoinTeamVoice()
    {
        if (PhotonVoiceNetwork.Instance.Client.InRoom == false)
        {
            Debug.LogWarning($"{this} : You are not currently in a voice room"); 
        }
        
        PhotonVoiceNetwork.Instance.Client.OpChangeGroups(_allByteValues, new [] {GetTeamByte()});
        PhotonVoiceNetwork.Instance.PrimaryRecorder.InterestGroup = 0; 
    }

    private byte GetTeamByte()
    {
        return (byte)((int)PhotonNetwork.LocalPlayer.CustomProperties[PhotonTeamsManager.TeamPlayerProp] + 1);
    }

    public void JoinAllVoice()
    {
        if (PhotonVoiceNetwork.Instance.Client.InRoom == false)
        {
            Debug.LogWarning($"{this} : You are not currently in a voice room");
        }
        
        PhotonVoiceNetwork.Instance.Client.OpChangeGroups(null, _allByteValues);
        PhotonVoiceNetwork.Instance.PrimaryRecorder.InterestGroup = 0; 
    }

    public void ToggleSelfMuted()
    {
        SelfMuted = !SelfMuted;
        SetMuted(SelfMuted); 
        
        PlayerPrefs.SetInt(KSelfMuted, SelfMuted ? 1 : 0);
    }

    public void ToggleOthersMuted()
    {
        OthersMuted = !OthersMuted; 
        SetMuteOthers(OthersMuted);
        
        PlayerPrefs.SetInt(KOthersMuted, OthersMuted ? 1 : 0);
    }

    private void CheckVoiceActive()
    {
        SelfMuted = PlayerPrefs.GetInt(KSelfMuted, 0) == 1;
        OthersMuted = PlayerPrefs.GetInt(KOthersMuted, 0) == 1; 
        
        SetMuted(SelfMuted);
        SetMuteOthers(OthersMuted);
    }

    private void SetMuted(bool muted)
    {
        recorder.VoiceDetection = !muted;
        recorder.TransmitEnabled = !muted;
    }

    private void SetMuteOthers(bool muted)
    {
        speaker.enabled = !muted; 
    }

    #region Pun Callbacks

    private const byte JoinedRoomCode = 226; 
    
    private void OnOpResponse(OperationResponse response)
    {
        Debug.Log($"{this} : Response : {response.OperationCode}");
        if (response.OperationCode == JoinedRoomCode)
        {
            JoinAllVoice();
        }
    }
    
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        SpawnVoicePrefab();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        RemoveCurrVoice();
    }

    #endregion
}
