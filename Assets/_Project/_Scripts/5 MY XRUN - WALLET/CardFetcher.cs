using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Web;
using UnityEngine.Networking;
using Newtonsoft.Json;

[Serializable]
public class CardFetcher : MonoBehaviour
{
    [Serializable]
    public class AllCardData
    {
        [SerializeField] public List<CardData> Data;
    }

    [SerializeField] GameObject carousel;
    [SerializeField] Transform carouselContentTransform;
    [SerializeField] GameObject cardPrefab;

    public AllCardData allCardData;

    public delegate void FirstCardFetchHandler(Toggle firstCardToggle);
    public event FirstCardFetchHandler OnFirstCardsFetched;

    void Awake()
    {
        StartCoroutine(RequestCardsData());
    }
    IEnumerator RequestCardsData()
    {
        // building query
        string endpoint = ServerDataStatic.GetGateway();
        var uriBuilder = new UriBuilder(endpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["act"] = "app4000-01-rev-01";
        query["member"] = PlayerDataStatic.Member;
        uriBuilder.Query = query.ToString();
        endpoint = uriBuilder.ToString();

        // requesting cards serverData
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
                AllCardData responseData = new AllCardData();
                responseData = JsonConvert.DeserializeObject<AllCardData>(rawData);
                allCardData = responseData;
                Debug.Log(rawData);
                PopulateCard();
            }
        }
    }
    void PopulateCard()
    {
        // instantiate prefab as per number carddata list count
        // set prefab parent (carousel - content)
        // define rect transform position

        for (int i = 0; i < allCardData.Data.Count; i++)
        {
            GameObject instance = Instantiate(cardPrefab, carouselContentTransform);
            instance.name = allCardData.Data[i].displaystr + " card";
            float k = i  * 2100f;
            RectTransform rect = instance.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(k, -100f);
            

            CardGenerator instanceCardDetail = instance.GetComponent<CardGenerator>();
            instanceCardDetail.Setup(allCardData.Data[i]);

            Toggle instanceToggle = instance.GetComponent<Toggle>();
            instanceToggle.group = carouselContentTransform.GetComponentInChildren<ToggleGroup>();
            
            if (i == 0)
            {
                instanceToggle.isOn = true;
                if (OnFirstCardsFetched != null)
                {
                    OnFirstCardsFetched(instanceToggle);
                } 
            }
            else
            {
                instanceToggle.isOn = false;
            }
            
        }
    }
}
