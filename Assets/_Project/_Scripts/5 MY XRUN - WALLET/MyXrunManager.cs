using AppsFlyerSDK;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MyXrunManager : Panel
{
    [SerializeField] MyQRCreator myQRCreator;

    [SerializeField] CanvasGroup qrPromptPanel;
    [SerializeField] Image qrBgImage;
    
    [SerializeField] Button sevenDaysButton;
    [SerializeField] Button fiveteenDaysButton;
    [SerializeField] Button thirtyDaysButton;

    float qrPromptcurrentAlpha;
    float qrPromptdesiredAlpha;
    string invokeQrGamebjectNotActive = "QrPanelSetNotActive";

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
        currentAlpha = 0;
        desiredAlpha = 0;
        promptPanel.gameObject.SetActive(false);

        qrPromptcurrentAlpha= 0;
        qrPromptdesiredAlpha = 0;
        qrPromptPanel.gameObject.SetActive(false); 
        qrBgImage.gameObject.SetActive(false);
    }
    private void Update()
    {
        UpdatePromptTextPanelAlpha();

        qrPromptPanel.alpha = qrPromptcurrentAlpha;
        qrPromptcurrentAlpha = Mathf.MoveTowards(qrPromptcurrentAlpha, qrPromptdesiredAlpha, fadeTime * Time.deltaTime);
    }

    private void OnEnable()
    {
        myQRCreator.OnQrImageSaved += ActionOnQRImageSaved;
    }
    private void OnDisable()
    {
        myQRCreator.OnQrImageSaved -= ActionOnQRImageSaved;
    }
    public void GoBackToHomeButtonClicked()
    {
        StartCoroutine(SceneLoader.LoadScene(4,0.25f));
    }
    // QR Function
    public void ShowQrCodePanel()
    {
        qrPromptPanel.gameObject.SetActive(true);
        qrBgImage.gameObject.SetActive(true);
        qrPromptdesiredAlpha = 1f;
    }
    public void HideQrCodePanel()
    {
        qrPromptdesiredAlpha = 0f;
        Invoke(invokeQrGamebjectNotActive, fadeTime);
    }
    // Transaction Function
    public void ShowHideDaysTransaction()
    {
        if (sevenDaysButton.isActiveAndEnabled == false)
        {
            sevenDaysButton.gameObject.SetActive(true);
            fiveteenDaysButton.gameObject.SetActive(true);
            thirtyDaysButton.gameObject.SetActive(true);
        }
        else
        {
            sevenDaysButton.gameObject.SetActive(false);
            fiveteenDaysButton.gameObject.SetActive(false);
            thirtyDaysButton.gameObject.SetActive(false);
        }
    }
    public void GoToSendButton()
    {
        StartCoroutine(SceneLoader.LoadScene(17, 0.25f));
    }
    public void GoToExchangeButton()
    {
        StartCoroutine(SceneLoader.LoadScene(18, 0.25f));
    }

    public void ActionOnAdressCopied()
    {
        ShowPromptTextPanelForSeconds(copyAdressToClipboard, 1.5f);
    }
     void ActionOnQRImageSaved()
    {
        ShowPromptTextPanelForSeconds(saveQrImage, 1f);
    }
    void QrPanelSetNotActive()
    {
        qrPromptPanel.gameObject.SetActive(false);
        qrBgImage.gameObject.SetActive(false);  
    }
}
