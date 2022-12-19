using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using static System.Net.WebRequestMethods;

public class WebRequestTest : MonoBehaviour
{
    [SerializeField] TMP_Text logText;
    [SerializeField] TMP_InputField indexNumber;

    public AllCoinData serverData;
    public List<CoinData> data;

    string app2000_02 = "https://app.xrun.run/gateway.php?act=app2000-01&lat=-6.244058443686903&lng=106.82540435767132&member=1102&limit=50";
    string app4100_02 = "https://app.xrun.run/gateway.php?startwith=0&member=1102&act=app4100-02&currency=11&daysbefore=30";
    string app4000_01_rev_01 = "https://app.xrun.run/gateway.php?act=app4000-01-rev-01&member=1102&currency=11";

    public void TestCoinCall()
    {
        StartCoroutine(GetWebRequest(app2000_02));
    }

    public void TestTransactionCall()
    {
        StartCoroutine(GetWebRequest(app4100_02));
    }

    public void TestCardCall()
    {
        StartCoroutine(GetWebRequest(app4000_01_rev_01));
    }

    public void ClearText()
    {
        logText.text = "Log text here";
    }

    public void TestVariableByIndex()
    {
        int index;
        int.TryParse(indexNumber.text, out index); 
        logText.text = serverData.data[index].ToString();
    }

    IEnumerator GetWebRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    logText.text = webRequest.downloadHandler.text;
                    var rawData = webRequest.downloadHandler.text;
                    serverData = JsonConvert.DeserializeObject<AllCoinData>(rawData);

                    foreach(CoinData coinData in serverData.data)
                    {
                        data.Add(coinData);
                    }

                    break;
            }
        }
    }
}