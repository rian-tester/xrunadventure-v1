using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Web;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class KirimPertanyaan : MonoBehaviour
{
    [SerializeField] TMP_InputField question;
    string endpoint;
    public void SendQuestion()
    {
        StartCoroutine(Submit());
        endpoint = ServerDataStatic.GetGateway();
    }

    IEnumerator Submit()
    {
        var uriBuilder = new UriBuilder(endpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["act"] = "ap6000-02";
        query["member"] = PlayerDataStatic.Member.ToString();
        query["Question"] = question.text.ToString();
        query["os"] = "3114";
        uriBuilder.Query = query.ToString();
        endpoint = uriBuilder.ToString();
        // web request to server
        using (UnityWebRequest www = UnityWebRequest.Get(endpoint))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                print("Question send!");
                question.text = null;
            }
        }

    }
}
