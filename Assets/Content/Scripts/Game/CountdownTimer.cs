using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using ExitGames.Client.Photon;
using TMPro;
using UnityEngine;

namespace Game
{
    public class CountdownTimer : MonoBehaviour
    {
        public static Action OnCountdownEnded; 
        
        [SerializeField] private TMP_Text countdownText;

        private readonly WaitForSeconds _countdownWait = new WaitForSeconds(1);

        private void OnEnable()
        {
            GameController.AllPlayersReady += StartCountdown; 
        }

        private void OnDisable()
        {
            GameController.AllPlayersReady -= StartCountdown; 
        }

        private void Awake()
        {
            countdownText.enabled = false; 
        }

        public void StartCountdown()
        {
            StartCoroutine(CoCountdown()); 
        }

        IEnumerator CoCountdown()
        {
            countdownText.enabled = true; 
            UpdateCountdownText("3");
            yield return _countdownWait;
            UpdateCountdownText("2");
            yield return _countdownWait;
            UpdateCountdownText("1");
            yield return _countdownWait;
            UpdateCountdownText("GO!");
            yield return _countdownWait;
            countdownText.enabled = false; 
            OnCountdownEnded?.Invoke();
        }

        private void UpdateCountdownText(string text)
        {
            countdownText.rectTransform.DOPunchScale(new Vector3(1.1f, 1.1f, 1.1f), .3f, 1, .2f);
            countdownText.text = text;
        }
    }
}
