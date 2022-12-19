using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Leguar.TotalJSON;

public class RegisterDataManager : Fields
{
    

    [SerializeField] TMP_InputField firstName;
    [SerializeField] TMP_InputField email;
    [SerializeField] TMP_Text phoneCode;
    [SerializeField] TMP_InputField phoneNumber;
    [SerializeField] TMP_InputField passwordInputField;
    [SerializeField] TMP_Text region;
    //[SerializeField] TMP_InputField referral;
    [SerializeField] Button registerButton;

    //[SerializeField] TMP_InputField verificationCodeInputField;
    //[SerializeField] Button submitVerificationCodeButton;
    //[SerializeField] Button repeatRegistrationButton;

    public string ageValue = null;
    public string sexValue = null;
   
    public event Action OnRequiredFieldsAreEmpty;
    public event Action OnEmailAlreadyExist;
    public event Action OnEmailIsOkayToUse;
    public event Action OnEmailNotCorrectFormat;
    public event Action OnSendingCodeToEmail;

    private void Awake()
    {
        if (RegistrationForm.Email != null)
        {
            email.text = RegistrationForm.Email;
        }
        if (RegistrationForm.Mobile != null)
        {
            phoneNumber.text = RegistrationForm.Mobile;
        }
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


    public void OnClickRegisterButton(Button button)
    {
        string[] fields = { email.text, passwordInputField.text, phoneNumber.text };
        if (!IsRequiredFieldsAreFilled(fields))
        {
            if (OnRequiredFieldsAreEmpty != null)
            {
                OnRequiredFieldsAreEmpty();
                Debug.Log("OnRequiredFieldsAreEmpty()");
               
            }
        }
        if (IsRequiredFieldsAreFilled(fields))
        {
            if (!IsEmailCorrectFormat(email.text))
            {
                if (OnEmailNotCorrectFormat != null)
                {
                    OnEmailNotCorrectFormat();
                    Debug.Log("OnEmailNotCorrectFormat()");
                }
            }
            else
            {
                RunCheckExistUser(button);
            }
        }
    }

    public void OnVerifyEmail(Button button)
    {
        if (IsEmailFieldAreEmpty(email.text))
        {
            OnRequiredFieldsAreEmpty();
        }
        else 
        {
            RunCheckExistUser(button);
        }
    }
    void RunCheckExistUser(Button button)
    {
        StartCoroutine(CheckExistUser(button)); 
    }
    IEnumerator CheckExistUser(Button button)
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
            registerButton.interactable = false;

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
                if (json.GetString("serverData") == "true")
                {
                    OnEmailAlreadyExist();
                }
                else if (json.GetString("serverData") == "false")
                {
                    // if email not exist in server continue process
                    
                    if (button.tag == "VerifyEmail")
                    {
                        OnEmailIsOkayToUse();
                    }
                    else if (button.tag == "Register")
                    {
                        StartCoroutine(SendCodeToEmail());
                    }
                }
            }
        }
        // button sign up interactable
        registerButton.interactable = true;
        Debug.Log("CheckExistUser Finish");
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

        // sending verification email
        using (UnityWebRequest www = UnityWebRequest.Get(endpoint))
        {
            // button sign up non interactable
            registerButton.interactable = false;

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                // store registration serverData in static class
                RegistrationForm.Firstname = firstName.text.ToString();
                RegistrationForm.Email = email.text.ToString();
                RegistrationForm.Pin = passwordInputField.text.ToString();
                RegistrationForm.Mobilecode = phoneCode.text.ToString();
                RegistrationForm.Mobile = phoneNumber.text.ToString();
                RegistrationForm.Country = phoneCode.text.ToString();
                RegistrationForm.Countrycode = "ID";
                string fullRegionString = region.text.ToString();
                int startIndex = fullRegionString.IndexOf("(");
                int endIndex = fullRegionString.IndexOf(")");
                string regionCode = fullRegionString.Substring(startIndex + 1, endIndex - startIndex - 1);
                RegistrationForm.Region = regionCode;
                RegistrationForm.Age = ageValue; //Int16.Parse(ageValue);
                RegistrationForm.Gender = sexValue; //Int16.Parse(sexValue);

                // for code LoginCodeCounter.cs
                PlayerPrefs.SetString("email", RegistrationForm.Email);

                OnSendingCodeToEmail();
            }

            // button sign up interactable
            registerButton.interactable = true;

        }
        Debug.Log("SendCodeToEmail Finish");
    }
}
