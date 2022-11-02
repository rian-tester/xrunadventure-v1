using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HapusAkunPrompt : MonoBehaviour
{
    [SerializeField]
    CanvasGroup promptPanel;
    [SerializeField] TMP_Text promptText;
    [SerializeField] TMP_InputField passwordInputField;
    float fadeTime = 1;
    float currentAlphaPromptPanel;
    float desiredAlphaPromptPanel;
    string invokeGamebjectNotActive = "PrompTextPanelSetNotActive";
    private void Start()
    {
        promptPanel.gameObject.SetActive(false);
        currentAlphaPromptPanel = 0;
        desiredAlphaPromptPanel = 0;
    }
    private void Update()
    {
        promptPanel.alpha = currentAlphaPromptPanel;
        currentAlphaPromptPanel = Mathf.MoveTowards(currentAlphaPromptPanel, desiredAlphaPromptPanel, fadeTime * Time.deltaTime);
    }
    public void AgreeToDelete()
    {
        promptText.text = "Mohon masukkan password Anda untuk mengkonfirmasi penghapusan akun ini";
        desiredAlphaPromptPanel = 1f;
        promptPanel.gameObject.SetActive(true);
    }

    public void ClosePrompt()
    {
        desiredAlphaPromptPanel = 0f;
        // This is to delay deactivate game object before it fade out
        Invoke(invokeGamebjectNotActive, fadeTime);
    }
    public void ShowPassword()
    {
        switch (passwordInputField.contentType)
        {
            case TMP_InputField.ContentType.Standard:
                passwordInputField.contentType = TMP_InputField.ContentType.Password;
                passwordInputField.ForceLabelUpdate();
                break;
            case TMP_InputField.ContentType.Password:
                passwordInputField.contentType = TMP_InputField.ContentType.Standard;
                passwordInputField.ForceLabelUpdate();
                break;
        }
    }
    void PrompSetNotActive()
    {
        promptPanel.gameObject.SetActive(false);
    }


}
