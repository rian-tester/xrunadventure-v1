using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using AppsFlyerSDK;

public class LoginManager : Panel
{

    [SerializeField] LoginDataManager loginDataManager;
    [SerializeField] AudioManager audioManager;
    

    private void Awake()
    {
        if (audioManager == null)
        {
            audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        }   
    }
    private void Start()
    {
        currentAlpha = 0;
        desiredAlpha = 0;
        promptPanel.gameObject.SetActive(false);
    }
    private void Update()
    {
        UpdatePromptTextPanelAlpha();
    }
    private void OnEnable()
    {
        loginDataManager.OnRequiredFieldsAreEmpty += ActionIfRequiredFieldsAreEmpty;
        loginDataManager.OnLoginSuccesfull += ActionLoginSuccesfull;
        loginDataManager.OnLoginFailed += ActionLoginFailed;
        loginDataManager.OnEmailFieldsAreEmpty += ActionIfEmailFieldsAreEmpty;
        loginDataManager.OnEmailAlreadyExist += ActionIfEmailExist;
        loginDataManager.OnEmailNotExist += ActionIfEmailNotExist;
        loginDataManager.OnEmailNotCorrectFormat += ActionEmailNotInCorrectFormat;
    }
    private void OnDisable()
    {
        loginDataManager.OnRequiredFieldsAreEmpty -= ActionIfRequiredFieldsAreEmpty;
        loginDataManager.OnLoginSuccesfull -= ActionLoginSuccesfull;
        loginDataManager.OnLoginFailed -= ActionLoginFailed;
        loginDataManager.OnEmailFieldsAreEmpty -= ActionIfEmailFieldsAreEmpty;
        loginDataManager.OnEmailNotExist -= ActionIfEmailNotExist;
        loginDataManager.OnEmailNotCorrectFormat -= ActionEmailNotInCorrectFormat;
    }
    public void GoBackButtonClicked()
    {
        StartCoroutine(SceneLoader.LoadScene(0, 0.5f));
    }
    public void GoToEmailLogin()
    {
        StartCoroutine(SceneLoader.LoadScene(19, 0.5f));
    }
    public void DaftarDiSini()
    {
        StartCoroutine(SceneLoader.LoadScene(1, 0.5f));
    }
    public void OnClickNextPromptButton()
    {
        HidePromptTextPanel();
    }
    void ActionLoginSuccesfull()
    {
        audioManager.SuccesLoginSound();
        StartCoroutine(SceneLoader.LoadScene(4, 1.5f));
    }
    void ActionLoginFailed()
    {
        ShowPromptTextPanel(dataNotValid);
    }
    void ActionIfRequiredFieldsAreEmpty()
    {
        ShowPromptTextPanel(requiredFieldsEmpty);
    }

    void ActionIfEmailFieldsAreEmpty()
    {
        ShowPromptTextPanel(pleaseFillEmail);
    }

    void ActionIfEmailExist()
    {
        ShowPromptTextPanel(emailExist);
    }

    void ActionIfEmailNotExist()
    {
        ShowPromptTextPanel(emailNotExist);
    }
    void ActionEmailNotInCorrectFormat()
    {
        ShowPromptTextPanel(emailFormatnotCorrect);
    }
}
