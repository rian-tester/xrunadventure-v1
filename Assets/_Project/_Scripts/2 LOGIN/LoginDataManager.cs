using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System;
using System.Web;
using Leguar.TotalJSON;
using AppsFlyerSDK;
using System.Collections.Generic;

public class LoginDataManager : Fields
{
    [SerializeField] TMP_InputField email;
    [SerializeField] TMP_InputField password;

    public event Action OnRequiredFieldsAreEmpty;
    public event Action OnLoginSuccesfull;
    public event Action OnLoginFailed;
    public event Action OnEmailFieldsAreEmpty;
    public event Action OnEmailAlreadyExist;
    public event Action OnEmailNotExist;
    public event Action OnEmailNotCorrectFormat;

    [Header("Controlling")]
    [SerializeField] Button loginButton;

    /// <summary>
    /// Untuk Melihat password
    /// </summary>
    public void ShowPassword()
    {
        switch (password.contentType)
        {
            case TMP_InputField.ContentType.Standard:
                password.contentType = TMP_InputField.ContentType.Password;
                password.ForceLabelUpdate();
                break;
            case TMP_InputField.ContentType.Password:
                password.contentType = TMP_InputField.ContentType.Standard;
                password.ForceLabelUpdate();
                break;
        }
    }

    public void OnClickLoginButton()
    {
        string[] fields = { email.text, password.text};
        if (!IsRequiredFieldsAreFilled(fields))
        {
            if (OnRequiredFieldsAreEmpty != null)
            {
                OnRequiredFieldsAreEmpty();
            }
        }
        if (IsRequiredFieldsAreFilled(fields))
        {
            if (!IsEmailCorrectFormat(email.text))
            {
                if (OnEmailNotCorrectFormat != null)
                {
                    OnEmailNotCorrectFormat();
                }
            }
            else 
            {
                StartCoroutine(SendLoginRequest());
            }
            
        }
        
    }

    public void OnVerifyEmail()
    {
        if (IsEmailFieldAreEmpty(email.text))
        {
            OnEmailFieldsAreEmpty();
        }
        else
        {
            if (!IsEmailCorrectFormat(email.text))
            {
                if (OnEmailNotCorrectFormat != null)
                {
                    OnEmailNotCorrectFormat();
                }
            }
            else
            {
                StartCoroutine(CheckExistUser());
            }
        }
    }
    IEnumerator SendLoginRequest()
    {
        // building query
        string endpoint = ServerDataStatic.GetGateway();
        var uriBuilder = new UriBuilder(endpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["act"] = "login-checker";
        query["email"] = email.text.ToString();
        query["pin"] = password.text.ToString();
        query["os"] = "3114";
        uriBuilder.Query = query.ToString();
        endpoint = uriBuilder.ToString();

        using (UnityWebRequest www = UnityWebRequest.Get(endpoint))
        {
            loginButton.interactable = false;
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                string response = www.downloadHandler.text;
                if (response == "OK")
                {
                    PlayerPrefs.SetString("email", email.text.ToString());

                    // AF event
                    Dictionary<string, string>
                        eventValues = new Dictionary<string, string>();
                    eventValues.Add("Email",email.text);
                    eventValues.Add("Event time", DateTime.Now.ToString());
                    AppsFlyer.sendEvent(AFInAppEvents.LOGIN, eventValues);

                    OnLoginSuccesfull();
                    
                }
                else
                {
                    OnLoginFailed();
                }
            }
            
        }
        loginButton.interactable = true;
    }

    IEnumerator CheckExistUser()
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
            loginButton.interactable = false;

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
                    OnEmailAlreadyExist();
                }
                else if (json.GetString("data") == "false")
                {
                    // if email not exist please register first
                     OnEmailNotExist();
                }
            }
        }
        // button sign up interactable
        loginButton.interactable = true;
        Debug.Log("CheckExistUser Finish");
    }

}
