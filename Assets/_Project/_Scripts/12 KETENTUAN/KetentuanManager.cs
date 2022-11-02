using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KetentuanManager : MonoBehaviour
{
    [SerializeField] ScrollRect scrollRect;

    public void ShowContent()
    {
        if (!scrollRect.isActiveAndEnabled)
        {
            scrollRect.gameObject.SetActive(true);
        }
    }

    public void CloseContent()
    {
        if (scrollRect.isActiveAndEnabled)
        {
            scrollRect.gameObject.SetActive(false);
        }     
    }

    public void GoBackToAkunButtonClicked()
    {
        StartCoroutine(SceneLoader.LoadScene(8, 0.25f));
        //SceneManager.LoadScene(8);
    }
}
