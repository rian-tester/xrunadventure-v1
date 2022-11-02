using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class TransactionFetcher : MonoBehaviour
{
    [Serializable]
    public class AllTransactionData
    {
        public List<TransactionData> Data;
    }

    [SerializeField] ActiveCardManager activeCardManager;
    [SerializeField] RectTransform transactionParentTransform;
    [SerializeField] GameObject transactionPrefab;
    [SerializeField] GameObject nullTransactionPrefab;
    [SerializeField] AllTransactionData allTransactionData;

    string memberNumber;
    string activeCardCurrency;
    public int transactionDays = 30;
    private void Awake()
    {
        activeCardManager.OnActiveCardSet += GenerateTransactionHistory;
    }
    private void OnDestroy()
    {
        activeCardManager.OnActiveCardSet -= GenerateTransactionHistory;
    }
    void GenerateTransactionHistory()
    {
        memberNumber = PlayerDataStatic.Member;
        activeCardCurrency = activeCardManager.GetActiveCard().thisCardData.currency;
        StartCoroutine(RequestTransactionData());
    }
    public void GenerateTransactionHistoryWithDays(int days)
    {
        memberNumber = PlayerDataStatic.Member;
        StartCoroutine(RequestTransactionDataWithDays(days));
    }
    public void GenerateTransactionHistoryWithTransactionTypes(string actCode)
    {
        memberNumber = PlayerDataStatic.Member;
        StartCoroutine(RequestTransactionDataWithTransactionTypes(actCode));
    }
    IEnumerator RequestTransactionData()
    {
        
        // building query
        string endpoint = ServerDataStatic.GetGateway();
        var uriBuilder = new UriBuilder(endpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["act"] = "app4100-02";
        query["member"] = memberNumber;
        query["startwith"] = "0";
        query["currency"] = activeCardCurrency;
        query["os"] = "3114";
        uriBuilder.Query = query.ToString();
        endpoint = uriBuilder.ToString();

        // requesting transaction data
        using (UnityWebRequest www = UnityWebRequest.Get(endpoint))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                // cahching request response
                var rawData = www.downloadHandler.text;
                AllTransactionData responseData = new AllTransactionData();
                responseData = JsonConvert.DeserializeObject<AllTransactionData>(rawData);
                foreach(RectTransform transaction in transactionParentTransform)
                {
                    Destroy(transaction.gameObject);
                }
                allTransactionData.Data.Clear();
                allTransactionData = responseData;
                Debug.Log(allTransactionData.Data.Count);
                PopulateTransaction();

            }   
        }
    }
    IEnumerator RequestTransactionDataWithDays(int days)
    {
        // building query
        string endpoint = ServerDataStatic.GetGateway();
        var uriBuilder = new UriBuilder(endpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["act"] = "app4200-01";
        query["member"] = memberNumber;
        query["startwith"] = "0";
        query["currency"] = activeCardCurrency;
        transactionDays = days;
        query["daysbefore"] = transactionDays.ToString();
        query["os"] = "3114";
        uriBuilder.Query = query.ToString();
        endpoint = uriBuilder.ToString();

        // requesting transaction data
        using (UnityWebRequest www = UnityWebRequest.Get(endpoint))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                // cahching request response
                var rawData = www.downloadHandler.text;
                AllTransactionData responseData = new AllTransactionData();
                responseData = JsonConvert.DeserializeObject<AllTransactionData>(rawData);
                foreach (RectTransform transaction in transactionParentTransform)
                {
                    Destroy(transaction.gameObject);
                }
                allTransactionData.Data.Clear();
                allTransactionData = responseData;
                Debug.Log(allTransactionData.Data.Count);
                PopulateTransaction();

            }
        }
    }
    IEnumerator RequestTransactionDataWithTransactionTypes(string actCode)
    {
        // building query
        string endpoint = ServerDataStatic.GetGateway();
        var uriBuilder = new UriBuilder(endpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["act"] = actCode;
        query["member"] = memberNumber;
        query["startwith"] = "0";
        query["currency"] = activeCardCurrency;
        query["daysbefore"] = transactionDays.ToString();
        //query["code"] = "3114";
        uriBuilder.Query = query.ToString();
        endpoint = uriBuilder.ToString();

        // requesting transaction data
        using (UnityWebRequest www = UnityWebRequest.Get(endpoint))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                // cahching request response
                var rawData = www.downloadHandler.text;
                AllTransactionData responseData = new AllTransactionData();
                responseData = JsonConvert.DeserializeObject<AllTransactionData>(rawData);
                foreach (RectTransform transaction in transactionParentTransform)
                {
                    Destroy(transaction.gameObject);
                }
                allTransactionData.Data.Clear();
                allTransactionData = responseData;
                Debug.Log(allTransactionData.Data.Count);
                PopulateTransaction();

            }
        }
    }
    void PopulateTransaction()
    {
        if (allTransactionData.Data.Count == 0)
        {
            Debug.Log("No transaction!");
            Instantiate(nullTransactionPrefab, transactionParentTransform);
        }
        else
        {
            for (int i = 0; i < allTransactionData.Data.Count; i++)
            {
                GameObject transaction = Instantiate(transactionPrefab, transactionParentTransform);
                transaction.name = $"Transaction number : {allTransactionData.Data[i].transaction}";
                TransactionGenerator instanceTransactionGenerator = transaction.GetComponent<TransactionGenerator>();
                instanceTransactionGenerator.Setup(allTransactionData.Data[i]);
            }
        }

    }
}

