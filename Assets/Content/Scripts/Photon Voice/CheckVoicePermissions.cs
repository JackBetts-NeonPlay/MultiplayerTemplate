using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class CheckVoicePermissions : MonoBehaviour
{
    private void Awake()
    {
#if PLATFORM_ANDROID
        CheckPermissionsAndroid();        
#endif
    }

    private void CheckPermissionsAndroid()
    {
#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
#endif
    }
}
