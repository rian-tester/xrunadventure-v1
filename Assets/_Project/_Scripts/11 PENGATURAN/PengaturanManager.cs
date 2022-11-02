using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PengaturanManager : MonoBehaviour
{
    public void GoBackToAkunButtonClicked()
    {
        StartCoroutine(SceneLoader.LoadScene(8, 0.25f));
        //SceneManager.LoadScene(8);
    }
    public void GoToPengaturanPerangkat()
    {
        SceneManager.LoadScene(15);
    }
    public void LogOut()
    {
        PlayerDataStatic.DeleteData();
        PlayerPrefs.SetString("email", "");
        StartCoroutine(SceneLoader.LoadScene(0, 0.25f));
        //SceneManager.LoadScene(0);

    }
    public void GoToHapusAkun()
    {
        SceneManager.LoadScene(16);
    }
}
