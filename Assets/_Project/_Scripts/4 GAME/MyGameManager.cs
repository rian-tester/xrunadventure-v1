using AppsFlyerSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MyGameManager : Panel
{
    [SerializeField] Player player;
    [SerializeField] XrunRewarded xrunRewarded;
    [SerializeField] CanvasGroup coinCanvasGroup;
    [SerializeField] TMP_Text coinText;
    
    [SerializeField] UserLocation userLocation;

    [Header("Controlling")]
    [SerializeField] AudioManager audioManager;
    [SerializeField] Image audioButton;

    [Header("Updating")]
    [SerializeField] TMP_Text debugText;

    public event Action OnUserAgreeWatchAds;
    public event Action OnUserDisagreeWatchAds;

    private void Awake()
    {
        if (audioManager == null)
        {
            audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        }
        
        desiredAlpha = 0;
        currentAlpha = 0;
        promptPanel.gameObject.SetActive(false);
        coinCanvasGroup.gameObject.SetActive(false);

        // AF event
        Dictionary<string, string>
            eventValues = new Dictionary<string, string>();
        eventValues.Add("Scene Name", SceneManager.GetActiveScene().name);
        eventValues.Add("Member Number", PlayerDataStatic.Member);
        eventValues.Add("Event time", DateTime.Now.ToString());
        AppsFlyer.sendEvent(AFInAppEvents.CONTENT_VIEW, eventValues);
    }

    private void OnEnable()
    {
        userLocation.OnUserLocationNotAvailable += ActionUserLocationNotAvailable;
        player.OnPlayerCatchCoin += ShowAdsPanel;
        xrunRewarded.OnAdCancel += ActionWhenAdsNotFinishPlay;
    }
    private void OnDisable()
    {
        userLocation.OnUserLocationNotAvailable -= ActionUserLocationNotAvailable;
        player.OnPlayerCatchCoin -= ShowAdsPanel;
        xrunRewarded.OnAdCancel -= ActionWhenAdsNotFinishPlay;
    }

    private void Update()
    {
        // Call this on Update
        promptPanel.alpha = currentAlpha;
        coinCanvasGroup.alpha = currentAlpha;
        currentAlpha = Mathf.MoveTowards(currentAlpha, desiredAlpha, fadeTime * Time.deltaTime);
    }
    public void BacksoundToggle()
    {
        if (!audioManager.backsound.isPlaying)
        {
            audioManager.backsound.Play();
            audioButton.color = new Color(audioButton.color.r, audioButton.color.g, audioButton.color.b, 1f);

        }
        else
        {
            audioManager.backsound.Stop();
            audioButton.color = new Color(audioButton.color.r, audioButton.color.g, audioButton.color.b, 0.25f);

        }
    }

    public void GoToButtonPressed(int indexNumber)
    {
        StartCoroutine(SceneLoader.LoadScene(indexNumber, 0.1f));
    }

    // debugging button function
    public void ReloadScene()
    {
        debugText.text = "Reloading Scene";
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene);
    }
    public void ARTrackingLost()
    {
        debugText.text = "AR DragLocation Manager : tracking is Lost!";
    }
    public void ARTrackingRestored()
    {
        debugText.text = "AR DragLocation Manager : tracking is Restored!";
    }
    public void ARLocationEnabled()
    {
        debugText.text = "AR DragLocation Provider : DragLocation is Enabled!";
    }

    void ActionUserLocationNotAvailable()
    {
        ShowPromptTextPanelForSeconds(userLocationNotAvailable, 1.5f);
    }

    void ActionWhenAdsNotFinishPlay()
    {
        ShowPromptTextPanelForSeconds(AdsDisplayNotCompleted, 1.5f);
    }
    public void ShowAdsPanel()
    {
        string t = "Apakah Anda bersedia menonton iklan untuk mendapatkan koin ini?";
        ShowCostumPanel(coinCanvasGroup, coinText, t);
    }
    public void AgreeWatchAds()
    {
        coinCanvasGroup.gameObject.SetActive(false);
        desiredAlpha = 0f;
        // play ads
        if (OnUserAgreeWatchAds != null)
        {
            OnUserAgreeWatchAds();
            // this event listened by admobs
        }
        
    }
    public void DisagreeWatchAds()
    {
        HideAdsPanel();
        if (OnUserDisagreeWatchAds != null)
        {
            OnUserDisagreeWatchAds();
        }
        
    }
    void HideAdsPanel()
    {
        desiredAlpha = 0f;
        Invoke(invokeCostumGamebjectNotActive, fadeTime);
    }
    void CostumPanelNotActive()
    {
        coinCanvasGroup.gameObject.SetActive(false);
    }
}
