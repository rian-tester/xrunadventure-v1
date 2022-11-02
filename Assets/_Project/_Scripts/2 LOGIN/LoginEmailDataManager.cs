using System;
using System.Collections;
using System.Web;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Leguar.TotalJSON;

public class LoginEmailDataManager : Fields
{

    [SerializeField] TMP_InputField email;
    [SerializeField] Button okButton;

    public event Action OnSendingCodeToEmail;
    public event Action OnEmailNotExist;
    public event Action OnEmailFieldAreEmpty;
    public event Action OnEmailNotIncorrectFormat;

    public void OkButtonClicked()
    {
        if (IsEmailFieldAreEmpty(email.text))
        {
            if (OnEmailFieldAreEmpty != null)
            {
                OnEmailFieldAreEmpty();               
                return;
            }
        }
        if (!IsEmailCorrectFormat(email.text))
        {
            if (OnEmailNotIncorrectFormat != null)
            {
                OnEmailNotIncorrectFormat();
                return;
            }                
        }
        StartCoroutine(CheckEmailExist());
    }
    IEnumerator CheckEmailExist()
    {
        // building query
        string endpoint = ServerDataStatic.GetGateway();
        var uriBuilder = new UriBuilder(endpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["act"] = "login-04-email";
        query["email"] = email.text.ToString();
        query["os"] = "3114";
        uriBuilder.Query = query.ToString();
        endpoint = uriBuilder.ToString();


        // checking email existence in server
        using (UnityWebRequest www = UnityWebRequest.Get(endpoint))
        {
            // button sign up non interactable
            okButton.interactable = false;

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                // cahching request response
                var rawData = www.downloadHandler.text;
                JSON json = JSON.ParseString(rawData);
                if (json.GetString("data") == "true")
                {
                    // if email exist please continue login
                    StartCoroutine(SendCodeToEmail());
                }
                else if (json.GetString("data") == "false")
                {
                    // if email not exist please register first
                    if (OnEmailNotExist != null)
                    {
                        OnEmailNotExist();
                    }                    
                }
            }
        }
        okButton.interactable = true;
    }
    IEnumerator SendCodeToEmail()
    {
        // building query
        string endpoint = ServerDataStatic.GetGateway();
        var uriBuilder = new UriBuilder(endpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["act"] = "login-02-email";
        query["email"] = email.text.ToString();
        query["os"] = "3114";
        uriBuilder.Query = query.ToString();
        endpoint = uriBuilder.ToString();

        // checking email existence in server
        using (UnityWebRequest www = UnityWebRequest.Get(endpoint))
        {
            // button sign up non interactable
            okButton.interactable = false;

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                // cahching request response
                var rawData = www.downloadHandler.text;
                JSON json = JSON.ParseString(rawData);
                if (json.GetString("data") == "true")
                {
                    // if email exist please continue login
                    if (OnSendingCodeToEmail != null)
                    {
                        PlayerPrefs.SetString("email", email.text.ToString());
                        OnSendingCodeToEmail();
                    }
                }
                else if (json.GetString("data") == "false")
                {
                    Debug.Log("Server result are false");
                }
            }
        }
        okButton.interactable = true;
    }
}
