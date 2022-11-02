using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static CreateCodeManager;

public class RegisterManager : Panel
{

    [SerializeField] RegisterDataManager registerDataManager;
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
        registerDataManager.OnRequiredFieldsAreEmpty += ActionIfRequiredFieldsAreEmpty;
        registerDataManager.OnEmailAlreadyExist += ActionEmailAlreadyExist;
        registerDataManager.OnEmailIsOkayToUse += ActionEmailOkayToUse;
        registerDataManager.OnSendingCodeToEmail += ActionSendingCodeToEmail;
        registerDataManager.OnEmailNotCorrectFormat += ActionEmailNotCorrectFormat;
    }
    private void OnDisable()
    {
        registerDataManager.OnRequiredFieldsAreEmpty -= ActionIfRequiredFieldsAreEmpty;
        registerDataManager.OnEmailAlreadyExist -= ActionEmailAlreadyExist;
        registerDataManager.OnEmailIsOkayToUse -= ActionEmailOkayToUse;
        registerDataManager.OnSendingCodeToEmail -= ActionSendingCodeToEmail;
        registerDataManager.OnEmailNotCorrectFormat -= ActionEmailNotCorrectFormat;
    }

    public void GoBackButtonClicked()
    {
        StartCoroutine(SceneLoader.LoadScene(2, 0.5f));
    }
    public void OnClickNextPromptButton()
    {
        HidePromptTextPanel();
    }

    public void OnPhoneCodeButtonClick()
    {
        //print("Phone code clicked");
        SceneManager.LoadScene(22);
    }
    void ActionIfRequiredFieldsAreEmpty()
    {
        ShowPromptTextPanel(requiredFieldsEmpty);
    }
    void ActionEmailAlreadyExist()
    {
        ShowPromptTextPanel(emailExist);
    }
    void ActionEmailOkayToUse()
    {
        ShowPromptTextPanel(emailCanregister);
    }
    void ActionEmailNotCorrectFormat()
    {
        ShowPromptTextPanel(emailFormatnotCorrect);
    }
    void ActionSendingCodeToEmail()
    {
        StartCoroutine(SceneLoader.LoadScene(21, 0.5f));
    }

}
