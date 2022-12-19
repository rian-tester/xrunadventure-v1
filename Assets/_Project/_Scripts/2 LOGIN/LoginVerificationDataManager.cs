using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Web;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Leguar.TotalJSON;


public class LoginVerificationDataManager : Fields
{
    [SerializeField] Button okButton;
    [SerializeField] TMP_InputField[] codeFields;
    [SerializeField] TMP_InputField codeFieldsOneLine;

    string submittedCode;

    public event Action OnCodeNotCorrect;
    public event Action OnCodeCorrect;

    private void Start()
    {
        foreach (TMP_InputField inputField in codeFields)
        {
            inputField.onValueChanged.AddListener(delegate {AutoMoveNextCodeInput(inputField, codeFields);} );
        }

        codeFieldsOneLine.onSelect.AddListener(delegate { BringMobileKeyboardUp(); });
    }
    public void okButtonClicked()
    {
        #region Separate Code
        //var sb = new StringBuilder();
        //for(int i = 0; i < codeFields.Length; i++)
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
        Debug.Log(submittedCode);
        // building query
        string endpoint = ServerDataStatic.GetGateway();
        var uriBuilder = new UriBuilder(endpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["act"] = "login-03-email";
        query["email"] = PlayerPrefs.GetString("email");
        query["code"] = submittedCode;
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
                if (json.GetString("serverData") == "login")
                {
                    if (OnCodeCorrect != null)
                    {
                        OnCodeCorrect();
                    }
                }
                else if (json.GetString("serverData") == "false")
                {
                    if (OnCodeNotCorrect != null)
                    {
                        OnCodeNotCorrect();
                    }
                }
            }
        }
        okButton.interactable = true;
    }

}
