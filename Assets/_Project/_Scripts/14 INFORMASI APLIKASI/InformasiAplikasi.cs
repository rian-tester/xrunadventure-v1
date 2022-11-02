using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InformasiAplikasi : MonoBehaviour
{
    [SerializeField] TMP_Text applicationInformationText;
    private void Awake()
    {
        applicationInformationText.text = $"Version {Application.version}";
    }
    public void GoBackToAkunButtonClicked()
    {
        StartCoroutine(SceneLoader.LoadScene(8, 0.25f));
        //SceneManager.LoadScene(8);
    }
}
