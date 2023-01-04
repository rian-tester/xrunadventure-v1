using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class AdvertisementDataManager : MonoBehaviour
{
    public class AllAdvertisementData
    {
        public List<AdvertisementData> data;
    }
    [SerializeField] TMP_Dropdown filterDropdown;
    [SerializeField] TMP_Text availableAdsText;
    [SerializeField] TMP_Text filterParameterText;
    [SerializeField] RectTransform advertisementParentTransform;
    [SerializeField] GameObject advertisementPrefab;
    [SerializeField] XrunRewardedInventory rewardedAd;

    AllAdvertisementData allAdvertisementData;
    string selectedDropdownValue;
    private void Start()
    {
        OnDropdownValueChanged();
        filterDropdown.onValueChanged.AddListener(delegate { OnDropdownValueChanged(); });
        rewardedAd.OnAdComplete += RewardCoinToPlayer;
    }
    public void OnDropdownValueChanged()
    {
        int dropdownIndex = filterDropdown.GetComponent<TMP_Dropdown>().value;
        selectedDropdownValue = GetTheOrderFieldParam(filterDropdown.options[dropdownIndex].text);
        StartCoroutine(RequestAdvertisement());
    }

    string GetTheOrderFieldParam(string dropdownText)
    {
        string result;
        switch (dropdownText)
        {
            case "Jumlah":
                result = "amount";
                break;
            case "Tanggal":
                result = "dateleft";
                break;
                default:
                result = "amount";
                break;
        }
        return result;
    }

    IEnumerator RequestAdvertisement()
    {
        // building query
        string endpoint = ServerDataStatic.GetGateway();
        var uriBuilder = new UriBuilder(endpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["act"] = "app5010-01";
        query["orderField"] = selectedDropdownValue;
        query["member"] = PlayerDataStatic.Member;
        query["os"] = "3114";
        uriBuilder.Query = query.ToString();
        endpoint = uriBuilder.ToString();

        // requesting advertisement serverData
        using (UnityWebRequest www = UnityWebRequest.Get(endpoint))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                // cahching request responseTwo
                var rawData = www.downloadHandler.text;
                AllAdvertisementData responseData = new AllAdvertisementData();
                responseData = JsonConvert.DeserializeObject<AllAdvertisementData>(rawData);
                foreach (RectTransform transaction in advertisementParentTransform)
                {
                    Destroy(transaction.gameObject);
                }
                allAdvertisementData = responseData;
                PopulateAdvertisement();

            }
        }
    }
    private void PopulateAdvertisement()
    {
        availableAdsText.text = $"Total {allAdvertisementData.data.Count} iklan tersimpan";
        for (int i = 0; i < allAdvertisementData.data.Count; i++)
        {
            GameObject advertisement = Instantiate(advertisementPrefab, advertisementParentTransform);
            advertisement.name = $"Advertisement number : {allAdvertisementData.data[i].transaction}";
            AdvertisementItemGenerator instanceAdvGenerator = advertisement.GetComponent<AdvertisementItemGenerator>();
            instanceAdvGenerator.Setup(allAdvertisementData.data[i]);
        }

    }

    void RewardCoinToPlayer(AdvertisementData advData)
    {
        Debug.Log($"{advData.coin} from {gameObject.name}");
        StartCoroutine(RewardingCoinToPlayerProcess(advData));
    }

    IEnumerator RewardingCoinToPlayerProcess(AdvertisementData advData)
    {
        // building query
        string endpoint = ServerDataStatic.GetGateway();
        var uriBuilder = new UriBuilder(endpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["act"] = "nd-1001";
        query["member"] = PlayerDataStatic.Member;
        query["coin"] = advData.coin;
        query["advertisement"] = advData.advertisement;
        query["os"] = "3114";
        uriBuilder.Query = query.ToString();
        endpoint = uriBuilder.ToString();
        Debug.Log("Run web request to give coin");


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
                Debug.Log("Coin Rewarded");
                //if (OnCoinRewareded != null)
                //{
                //    OnCoinRewareded();
                //}
                Debug.Log(www.downloadHandler.text);
                int currentScene = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(currentScene);


            }
        }
    }

}
