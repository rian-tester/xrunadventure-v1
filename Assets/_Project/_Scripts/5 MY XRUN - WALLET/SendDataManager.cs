using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class SendDataManager : MonoBehaviour
{
    [SerializeField] TMP_InputField sendAmount;
    [SerializeField] TMP_InputField sendAdress;
    [SerializeField] TMP_InputField sendMemo;
    [SerializeField] TMP_Text activeCardBalance;

    public event Action OnSendProcessFinish;

    private void Start()
    {
        activeCardBalance.text = $"{ActiveCardDataStatic.Amount} {ActiveCardDataStatic.Symbol}";
    }
    public void ProcessSend()
    {
        StartCoroutine(BeforeSendToken());
    }
    IEnumerator BeforeSendToken()
    {
        var endpoint = ServerDataStatic.GetGateway();
        var uriBuilder = new UriBuilder(endpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["act"] = "app4300-01";
        query["member"] = PlayerDataStatic.Member.ToString();
        query["currency"] = ActiveCardDataStatic.Currency.ToString();
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
                Debug.Log($"Wallet serverData send");
            }
        }
        StartCoroutine(SendToken());
    }

    IEnumerator SendToken()
    {
        var endpoint = ServerDataStatic.GetGateway();
        var uriBuilder = new UriBuilder(endpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["act"] = "app4300-02";
        query["member"] = PlayerDataStatic.Member.ToString();
        query["amount"] = sendAmount.text;
        query["addrto"] = sendAdress.text;
        query["memo"] = sendMemo.text;
        query["currency"] = ActiveCardDataStatic.Currency.ToString();
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
                Debug.Log($"Data send to server");

                if (OnSendProcessFinish != null)
                {
                    OnSendProcessFinish();
                }
            }
        }
    }

}
