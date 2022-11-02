using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginEmailManager : Panel
{
    [SerializeField] LoginEmailDataManager loginEmailDataManager;
 
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
        loginEmailDataManager.OnEmailFieldAreEmpty += ActionEmailFieldAreEmpty;
        loginEmailDataManager.OnEmailNotIncorrectFormat += ActionEmailNotInCorrectFormat;
        loginEmailDataManager.OnEmailNotExist += ActionEmailNotExist;
        loginEmailDataManager.OnSendingCodeToEmail += ActionSendingCodeToEmail;
    }


    private void OnDisable()
    {
        loginEmailDataManager.OnEmailFieldAreEmpty -= ActionEmailFieldAreEmpty;
        loginEmailDataManager.OnEmailNotIncorrectFormat -= ActionEmailNotInCorrectFormat;
        loginEmailDataManager.OnEmailNotExist -= ActionEmailNotExist;
        loginEmailDataManager.OnSendingCodeToEmail -= ActionSendingCodeToEmail;
    }

    public void GoBackButtonClicked()
    {
        //SceneManager.LoadScene(2);
        StartCoroutine(SceneLoader.LoadScene(2, 0.5f));
    }
    public void OnClickNextPromptButton()
    {
        HidePromptTextPanel();
    }
    void ActionSendingCodeToEmail()
    {
        //SceneManager.LoadScene(20);
        StartCoroutine(SceneLoader.LoadScene(20, 0.5f));
    }
    void ActionEmailFieldAreEmpty()
    {
        ShowPromptTextPanel(pleaseFillEmail); 
    }
    void ActionEmailNotInCorrectFormat()
    {
        ShowPromptTextPanel(emailFormatnotCorrect);
    }
    void ActionEmailNotExist()
    {
        ShowPromptTextPanel(emailNotExist);
    }
}
