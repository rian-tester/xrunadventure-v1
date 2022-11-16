using AppsFlyerSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyAdsManager : Panel
{
    [SerializeField] XrunRewardedInventory rewardedAds; 
    [SerializeField] RectTransform advertisement;
    [SerializeField] RectTransform missionCompleted;
    [SerializeField] RectTransform cachedPosition;
    [SerializeField] CanvasGroup adsCanvasGroup;
    [SerializeField] TMP_Text adsText;
    [SerializeField] float movingSpeed;

    AdvertisementData advertisementData;

    public event Action<AdvertisementData> OnUserAgreeWatchAds;
    public event Action OnUserDisagreeWatchAds;

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

    private void Start()
    {
        desiredAlpha = 0;
        currentAlpha = 0;
        promptPanel.gameObject.SetActive(false);
        adsCanvasGroup.gameObject.SetActive(false);

        rewardedAds.OnAdCancel += ActionWhenAdsNotFinishPlay;
    }
    private void OnDestroy()
    {
        rewardedAds.OnAdCancel -= ActionWhenAdsNotFinishPlay;
    }

    private void Update()
    {
        promptPanel.alpha = currentAlpha;
        adsCanvasGroup.alpha = currentAlpha;
        currentAlpha = Mathf.MoveTowards(currentAlpha, desiredAlpha, fadeTime * Time.deltaTime);
    }
    public void BackToHomeButtonClicked()
    {
        StartCoroutine(SceneLoader.LoadScene(4, 0.25f));
    }
    public void ShowSavedAdvertisement()
    {
        if (cachedPosition.position != null)
        {
            missionCompleted.position = cachedPosition.position;
        }
        advertisement.gameObject.SetActive(true);
    }
    public void ShowMissionCompleted()
    {
        advertisement.gameObject.SetActive(false);
        StartCoroutine(AnimatedMove(missionCompleted.localPosition,
            advertisement.localPosition,
            missionCompleted,
            movingSpeed));
    }

    public void ShowAdsPanel(AdvertisementData advData)
    {
        string t = "Apakah Anda bersedia menonton iklan untuk mendapatkan koin ini?";
        advertisementData = advData;
        Debug.Log($"{advData.coin} from {gameObject.name}");
        ShowCostumPanel(adsCanvasGroup, adsText, t);
    }

    public void AgreeWatchAds()
    {
        adsCanvasGroup.gameObject.SetActive(false);
        desiredAlpha = 0f;
        // play ads
        if (OnUserAgreeWatchAds != null)
        {
            OnUserAgreeWatchAds(advertisementData);
            // this event listened by admobs
        }

    }
    public void DisagreeWatchAds()
    {
        advertisementData = null;
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
        adsCanvasGroup.gameObject.SetActive(false);
    }
    void ActionWhenAdsNotFinishPlay()
    {
        ShowPromptTextPanelForSeconds(AdsDisplayNotCompleted, 1.5f);
    }

    IEnumerator AnimatedMove(Vector3 origin, Vector3 destination, RectTransform obj, float speed )
    {
        Vector3 from = origin;
        Vector3 to = destination;
        RectTransform tra = obj;
        var t = 0f;

        while (t < 1f)
        {
            t += speed * Time.deltaTime;
            tra.localPosition = Vector3.Lerp(from, to, t);
            yield return null;
        }
    }
}
