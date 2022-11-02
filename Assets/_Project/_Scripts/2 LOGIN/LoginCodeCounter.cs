using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Xrun
{
    public class LoginCodeCounter : Countdown
    {
        [SerializeField] TMP_Text instructionText;
        [SerializeField] TMP_Text countdownText;

        string emailAdress;
        private void Awake()
        {
            emailAdress = PlayerPrefs.GetString("email");
            instructionText.text = instructionText.text + " " + emailAdress;
            countdownText.gameObject.SetActive(false);
            CountdownStart(300);
        }
        public override void CountdownStart(int second)
        {
            base.CountdownStart(second);
            countdownText.gameObject.SetActive(true);
        }
        public override void CountdownEnd()
        {
            countdownText.gameObject.SetActive(false);
        }

        public override void UpdateText()
        {
            countdownText.text = $"{textToDisplayCountdown} cek email untuk melihat kode verifikasi";
        }
    }
}

