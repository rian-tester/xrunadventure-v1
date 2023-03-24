using System.Web;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ExchangeDataManager : MonoBehaviour
{
    [SerializeField] TMP_InputField exchangeAmount;
    [SerializeField] TMP_InputField exchangeAddress;
    [SerializeField] TMP_Text currencyTitleText;
    [SerializeField] TMP_Text currencyDescriptionText;
    [SerializeField] bool isFixedRate = true;
    public List<Toggle> availableCurrency = new List<Toggle>();
    string exchangeRate = "0";
    string toCurrency = "";

    public event Action OnExchangeProcessFinish; // not applied yet

    private void Start()
    {
        GameObject[] toggles = GameObject.FindGameObjectsWithTag("Exchange");
        
        for(int i = 0; i < toggles.Length; i++)
        {
            var toggle = toggles[i].GetComponent<Toggle>();
            availableCurrency.Add(toggle);
            if (toggles[i].name == "ETH Button")
            {
                toggle.isOn = true;
                toggle.Select();
            }
        }

        currencyTitleText.text = $"{ActiveCardDataStatic.Symbol} yang akan ditukar";
        currencyDescriptionText.text = "Setiap penukaran akan dikenakan\r\nbiaya gas fee sekitar 0.124%";
        SwitchCurrency();
    }

    public void ProcessExchange()
    {
        StartCoroutine(BeforeExchangeToken());
    }
    public void SwitchCurrency()
    {
        foreach (Toggle currency in availableCurrency)
        {
            if (currency.isOn == true)
            {
                var text = currency.GetComponentInChildren<TMP_Text>();
                toCurrency = text.text;
                GetExchangeRate(isFixedRate);
            }
        }
    }
    double GetExchangeRate(bool isFixedRate)
    {
        double rate = 0;
        if (isFixedRate)
        {
            // dont do anything
            // maybe define fixed rate here or leave it 0

        }
        else
        {
            // if symbol is ETH, rate = amount * (450/2139400.0)
            // if symbol is TRX, rate = amount * (450/86.0)
            // if symbol is MATIC, rate = amount * (450/1230.0)

            string walletSymbol = toCurrency;
            double percentage = 0.0;
            if (walletSymbol == "ETH")
            {
                percentage = (450 / 2139400.0);
                rate = double.Parse(exchangeAmount.text) * percentage;
            }
            else if (walletSymbol == "TRX")
            {
                percentage = (450 / 86.0);
                rate = double.Parse(exchangeAmount.text) * percentage;
            }
            else if (walletSymbol == "MATIC")
            {
                percentage = (450 / 1230.0);
                rate = double.Parse(exchangeAmount.text) * percentage;
            }
            else
            {
                rate = rate = double.Parse(exchangeAmount.text) * 0.00124;
            }

            currencyDescriptionText.text = $"Setiap penukaran akan dikenakan\r\nbiaya gas fee sekitar {percentage}";
        }
        Debug.Log(rate.ToString());
        return rate;
    }
    IEnumerator BeforeExchangeToken()
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
                StartCoroutine(ExchangeToken());
            }
        }
    }

    IEnumerator ExchangeToken()
    {
        var endpoint = ServerDataStatic.GetGateway();
        var uriBuilder = new UriBuilder(endpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["act"] = "app4420-02";
        query["member"] = PlayerDataStatic.Member.ToString();
        query["amount"] = exchangeAmount.text;
        query["fee"] = GetExchangeRate(isFixedRate).ToString() ;
        query["currency"] = ActiveCardDataStatic.Currency.ToString();
        query["extracurrency"] = toCurrency;
        query["adress"] = exchangeAddress.text;
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

                if (OnExchangeProcessFinish != null)
                {
                    OnExchangeProcessFinish();
                }
            }
        }
    }
}
