using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LayananPelanggan : MonoBehaviour
{
    [SerializeField] ScrollRect faqScrollRect;
    [SerializeField] Image questionPanel;

    public void ShowFaq()
    {
        if (!faqScrollRect.isActiveAndEnabled)
        {
            faqScrollRect.gameObject.SetActive(true);
        }
    }
    public void ShowQuestion()
    {
        if (!questionPanel.isActiveAndEnabled)
        {
            questionPanel.gameObject.SetActive(true);
        }
    }
    public void CloseFaq()
    {
        faqScrollRect.gameObject.SetActive(false);
    }
    public void CloseQuestionPanel()
    {
        questionPanel.gameObject.SetActive(false);
    }
    public void GoBackToAkunButtonClicked()
    {
        StartCoroutine(SceneLoader.LoadScene(8, 0.25f));
        //SceneManager.LoadScene(8);
    }
}
