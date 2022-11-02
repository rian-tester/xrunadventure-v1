using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterVerificationManager : Panel
{
    [SerializeField] RegisterVerificationDataManager registerVerificationDataManager;
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
        registerVerificationDataManager.OnCodeNotCorrect += ActionCodeNotCorrect;
        registerVerificationDataManager.OnCodeCorrect += ActionCodeCorrect;
        registerVerificationDataManager.OnRegistrationFailed += ActionRegistrationFailed;
    }
    private void OnDisable()
    {
        registerVerificationDataManager.OnCodeNotCorrect -= ActionCodeNotCorrect;
        registerVerificationDataManager.OnCodeCorrect -= ActionCodeCorrect;
        registerVerificationDataManager.OnRegistrationFailed -= ActionRegistrationFailed;
    }
    public void GoBackButtonClicked()
    {
        StartCoroutine(SceneLoader.LoadScene(1, 0.5f));
    }
    public void OnClickNextPromptButton()
    {
        HidePromptTextPanel();
    }
    void ActionCodeNotCorrect()
    {
        ShowPromptTextPanel(codeNotcorrect);
    }
    void ActionCodeCorrect()
    {
        audioManager.SuccesLoginSound();
        StartCoroutine(SceneLoader.LoadScene(4, 1.5f));
    }
    void ActionRegistrationFailed()
    {
        ShowPromptTextPanel(registrationFailed);
    }
}
