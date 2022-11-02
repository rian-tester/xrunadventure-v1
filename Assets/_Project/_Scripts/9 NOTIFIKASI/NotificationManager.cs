using AppsFlyerSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NotificationManager : MonoBehaviour
{
    private void Awake()
    {
        // AF event
        Dictionary<string, string>
            eventValues = new Dictionary<string, string>();
        eventValues.Add("Scene Name", SceneManager.GetActiveScene().name);
        eventValues.Add("Member Number", PlayerDataStatic.Member);
        eventValues.Add("Event time", DateTime.Now.ToString());
        AppsFlyer.sendEvent(AFInAppEvents.CONTENT_VIEW, eventValues);
    }
    public void GoBackToHomeButtonClicked()
    {
        SceneManager.LoadScene(4);
    }
}
