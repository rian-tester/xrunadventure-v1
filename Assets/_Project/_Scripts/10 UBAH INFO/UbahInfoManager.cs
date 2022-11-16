using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UbahInfoManager : MonoBehaviour
{
    [SerializeField] TMP_Text playerName;

    private void Start()
    {
        playerName.text = PlayerDataStatic.Firstname;
    }
    public void GoBackToAkunButtonClicked()
    {
        StartCoroutine(SceneLoader.LoadScene(8, 0.25f));
        //SceneManager.LoadScene(8);
    }
    public void ChangeUserData()
    {
        // TO DO
    }
}
