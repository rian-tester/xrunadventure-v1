using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginVerificationManager : Panel
{
    [SerializeField] LoginVerificationDataManager loginVerificationDataManager;
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
        loginVerificationDataManager.OnCodeNotCorrect += ActionCodeNotCorrect; 
        loginVerificationDataManager.OnCodeCorrect += ActionCodeCorrect; 
    }
    private void OnDisable()
    {
        loginVerificationDataManager.OnCodeNotCorrect -= ActionCodeNotCorrect;
        loginVerificationDataManager.OnCodeCorrect -= ActionCodeCorrect;
    }
    public void GoBackButtonClicked()
    {
        //SceneManager.LoadScene(19);
        StartCoroutine(SceneLoader.LoadScene(19, 0.5f));
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
        //SceneManager.LoadScene(4);
        audioManager.SuccesLoginSound();
        StartCoroutine(SceneLoader.LoadScene(4, 1.5f));

    }


}
