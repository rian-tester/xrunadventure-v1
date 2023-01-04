using System;
using System.Collections;
using System.Web;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using AppsFlyerSDK;
using System.Collections.Generic;
using Newtonsoft.Json;

public class RegisterVerificationDataManager : Fields
{
    public class Response
    {
        public string data;
        public string value;
        public string mv;
    }

    public class ResponseTwo
    {
        public string data;
        public string value;
    }


    Response response;
    ResponseTwo responseTwo;

    [SerializeField] Button okButton;
    [SerializeField] TMP_InputField[] codeFields;
    [SerializeField] TMP_InputField codeFieldsOneLine;

    string submittedCode;

    public event Action OnCodeNotCorrect;
    public event Action OnCodeCorrect;
    public event Action OnRegistrationFailed;

    private void Start()
    {
        foreach (TMP_InputField inputField in codeFields)
        {
            inputField.onValueChanged.AddListener(delegate { AutoMoveNextCodeInput(inputField, codeFields); });
        }
    }

    public void okButtonClicked()
    {
        #region Separate Code
        //var sb = new StringBuilder();
        //for (int i = 0; i < codeFields.Length; i++)
        //{
        //    sb.Append(codeFields[i].text);
        //}
        //using var reader = new StringReader(sb.ToString());
        //submittedCode = reader.ReadToEnd();
        #endregion

        submittedCode = codeFieldsOneLine.text;
        StartCoroutine(SubmitVerificationcode());
    }
    IEnumerator SubmitVerificationcode()
    {
        // building query
        string endpoint = ServerDataStatic.GetGateway();
        var uriBuilder = new UriBuilder(endpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["act"] = "login-03-email";
        query["email"] = RegistrationForm.Email;
        query["code"] = submittedCode;
        query["os"] = "3114";
        uriBuilder.Query = query.ToString();
        endpoint = uriBuilder.ToString();

        // requesting email verification
        using (UnityWebRequest www = UnityWebRequest.Get(endpoint))
        {
            // ok button non interactable
            okButton.interactable = false;

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                var rawData = www.downloadHandler.text;
                response = JsonConvert.DeserializeObject<Response>(rawData);

                if ( response.data == "false")
                {
                    OnCodeNotCorrect();
                }
                else if (response.data == "login")
                {
                    StartCoroutine(RegisterNewMember());
                }
            }
            okButton.interactable = true;
        }
    }

    IEnumerator RegisterNewMember()
    {
        // building query
        string endpoint = ServerDataStatic.GetGateway();
        var uriBuilder = new UriBuilder(endpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["act"] = "login-05-joinAndAccount";
        query["firstname"] = RegistrationForm.Firstname;
        query["lastname"] = RegistrationForm.Lastname;
        query["email"] = RegistrationForm.Email;
        query["pin"] = RegistrationForm.Pin;
        query["mobilecode"] = RegistrationForm.Mobilecode;
        query["mobile"] = RegistrationForm.Mobile;
        query["countrycode"] = RegistrationForm.Countrycode;
        query["country"] = RegistrationForm.Country;
        query["region"] = RegistrationForm.Region;
        query["ages"] = RegistrationForm.Age;
        query["gender"] = RegistrationForm.Gender;
        query["os"] = "3114";
        uriBuilder.Query = query.ToString();
        endpoint = uriBuilder.ToString();

        // requesting email verification
        using (UnityWebRequest www = UnityWebRequest.Get(endpoint))
        {
            // ok button non interactable
            okButton.interactable = false;

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                OnRegistrationFailed();
            }
            else
            {
                // cahching request responseTwo
                var rawData = www.downloadHandler.text;
                responseTwo = JsonConvert.DeserializeObject<ResponseTwo>(rawData);

                if (responseTwo.data == "ok")
                {
                    Debug.Log("Registration result : \n" + www.downloadHandler.text);
                    PlayerPrefs.SetString("email", RegistrationForm.Email);
                    
                    
                    if (AppsFlyer.instance != null)
                    {
                        // AF event
                        Dictionary<string, string>
                            eventValues = new Dictionary<string, string>();
                        eventValues.Add("Email", RegistrationForm.Email);
                        eventValues.Add("Phone Number", $"{RegistrationForm.Mobilecode} {RegistrationForm.Mobile}");
                        eventValues.Add("Event time", DateTime.Now.ToString());
                        AppsFlyer.sendEvent(AFInAppEvents.COMPLETE_REGISTRATION, eventValues);
                    }

                    if (OnCodeCorrect != null)
                    {
                        OnCodeCorrect();
                    }

                }
            }
        }
        okButton.interactable = true;
    }
}
