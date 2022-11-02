using AppsFlyerSDK;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AkunManager : MonoBehaviour
{
    private void Awake()
    {
        // AF event
        Dictionary<string, string>
            eventValues = new Dictionary<string, string>();
        eventValues.Add("Scene Name", SceneManager.GetActiveScene().name);
        eventValues.Add("Member Number", PlayerDataStatic.Member);
        eventValues.Add("Event time", DateTime.Now.ToString());
        AppsFlyer.sendEvent(AFInAppEvents.CONTENT_VIEW, eventValues);
    }
    private void Start()
    {

    }
    public void GoBackToHomeButtonClicked()
    {
        StartCoroutine(SceneLoader.LoadScene(4, 0.25f));
        //SceneManager.LoadScene(4);
    }
    public void MiddleButtonsClicked(int buttonCode)
    {
        StartCoroutine(MiddleButtonsClickedSequence(buttonCode));
    }

    IEnumerator MiddleButtonsClickedSequence(int buttoncode)
    {
        switch (buttoncode)
        {
            case 10: // Ubah Info
                yield return new WaitForSeconds(0.5f);
                SceneManager.LoadScene(10);
                break;
            case 11: // Pengaturan
                yield return new WaitForSeconds(0.5f);
                SceneManager.LoadScene(11);
                break;
            case 12: // Ketentuan
                yield return new WaitForSeconds(0.5f);
                SceneManager.LoadScene(12);
                break;
            case 13: // Costumer Care 
                yield return new WaitForSeconds(0.5f);
                SceneManager.LoadScene(13);
                break;
            case 14: // Informasi Aplikasi
                yield return new WaitForSeconds(0.5f);
                SceneManager.LoadScene(14);
                break;
            case 99: // Log out Button
                yield return new WaitForSeconds(0.5f);
                SceneManager.LoadScene(14);
                break;
            case 98: // Share button
                yield return new WaitForSeconds(0.5f);
                SceneManager.LoadScene(14);
                break;


        }
    }
    public void LogOut()
    {
        StartCoroutine(SceneLoader.LoadScene(0, 0.25f));
        //SceneManager.LoadScene(0);
        PlayerDataStatic.DeleteData();
        PlayerPrefs.SetString("email", "");
    }
}
