using ARLocation;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;



// This class placed in independent Object called CoinManager
// Exist in hierarchy

// This class handle : 
// Making call to xrun Api coinmapping
// Contain 2 Costum Class for Server Data Deserialiasation
// Place it in 3D World using GeoLocation
// Updating it

// This class info :
// Inherit from ARLocation.PlaceAtLocations

public class MyPlaceAtLocations : PlaceAtLocations
{
    #region CostumClassForCoinData
    public class ServerCoinData
    {
        public List<CoinData> data;
    }
    #endregion

    [Space(2.0f)]
    [Header("Gameobject Setup")]
    [SerializeField] GameObject myPrefab;
    [SerializeField] Camera arCamera;

    [Space(2.0f)]
    [Header("Spawning Setup")]
    ServerCoinData serverRawData;
    [TextArea(5, 8)]
    public string serverResults;

    [SerializeField] int spawnAmount;
    double latitudeApi;
    double longitudeApi;

    [Space(2.0f)]
    [Header("Debugging")]
    [SerializeField]
    TMP_Text debugText;

    //public event Action OnCoinSpawn;
    //public event Action OnCoinSpawnStartCallServer;
    public event Action OnCoinFinishIterating;
    //public event Action OnCoinStartIterating;
    public event Action OnTrackingRestored;

    public void StartPopulateCoins()
    {
        //OnCoinSpawnStartCallServer();

        if (spawnAmount > 0)
        {
            Debug.LogWarning("Server serverData still exist so populate from last serverData");
            debugText.text = "Server serverData still exist so populate from last serverData";
            PopulateCoins(serverRawData);
        }
        else if (spawnAmount == 0 || spawnAmount < 1)
        {
            Debug.LogWarning("No serverData in server, so call server again");
            debugText.text = "No serverData in server, so call server again";
            int randomSpawnAmount = UnityEngine.Random.Range(200, 501);
            spawnAmount = randomSpawnAmount;
            StartCoroutine(CallServer(spawnAmount));
        }
    }

    // ARLocationManager.Instance.GetLocationForWorldPosition() --> to get lat lon from Unity transform
    IEnumerator CallServer(int spawnAmountFinal)
    {
        // retreive player geo location
        Location playerLocation = ARLocationManager.Instance.GetLocationForWorldPosition(arCamera.gameObject.transform.position);
        latitudeApi = playerLocation.Latitude;
        longitudeApi = playerLocation.Longitude;

        // building query
        string endpoint = "https://app.xrun.run/gateway.php?";
        var uriBuilder = new UriBuilder(endpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["act"] = "coinmapping";
        query["member"] = PlayerDataStatic.Member.ToString();
        query["limit"] = spawnAmountFinal.ToString();
        query["lat"] = latitudeApi.ToString();
        query["lng"] = longitudeApi.ToString();
        query["os"] = "3114";
        uriBuilder.Query = query.ToString();
        endpoint = uriBuilder.ToString();
        debugText.text = "finish building query";
        // web request to server
        using (UnityWebRequest www = UnityWebRequest.Get(endpoint))
        {
            yield return www.SendWebRequest();
            debugText.text = "finish calling the server api";
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                debugText.text = "succesfully calling raw serverData from server";
                // cahching request responseTwo
                var rawData = www.downloadHandler.text;
                // store into costum class
                serverRawData = new ServerCoinData();
                serverRawData = JsonConvert.DeserializeObject<ServerCoinData>(rawData);
                serverResults = rawData;
                debugText.text = "succesfully deserialize server serverData into custom class";

                Debug.Log($"Total coins is {serverRawData.data.Count}");

                PopulateCoins(serverRawData);
                debugText.text = $"Server Called finish calling {spawnAmountFinal} coins";
            }
        }
    }
    void PopulateCoins(ServerCoinData coinsData)
    {
        //OnCoinStartIterating();

        debugText.text = coinsData.ToString();
        System.Random rand = new System.Random();
        for (int i = 0; i < coinsData.data.Count; i++)
        {
            CoinData prefabCoinDataComponent = myPrefab.GetComponent<CoinData>();




            prefabCoinDataComponent.coin = serverRawData.data[i].coin;
            prefabCoinDataComponent.cointype = serverRawData.data[i].cointype;
            prefabCoinDataComponent.amount = serverRawData.data[i].amount;
            prefabCoinDataComponent.countlimit = serverRawData.data[i].countlimit;
            prefabCoinDataComponent.lng = serverRawData.data[i].lng;
            prefabCoinDataComponent.lat = serverRawData.data[i].lat;
            prefabCoinDataComponent.distance = serverRawData.data[i].distance;
            prefabCoinDataComponent.advertisement = serverRawData.data[i].advertisement;
            prefabCoinDataComponent.brand = serverRawData.data[i].brand;
            prefabCoinDataComponent.title = serverRawData.data[i].title;
            prefabCoinDataComponent.contents = serverRawData.data[i].contents;
            prefabCoinDataComponent.currency = serverRawData.data[i].currency;
            prefabCoinDataComponent.adColor1 = serverRawData.data[i].adColor1;
            prefabCoinDataComponent.adColor2 = serverRawData.data[i].adColor2;
            prefabCoinDataComponent.coins = serverRawData.data[i].coins;
            prefabCoinDataComponent.adThumbnail = serverRawData.data[i].adThumbnail;
            prefabCoinDataComponent.adThumbnail = null;
            prefabCoinDataComponent.adThumbnail2 = serverRawData.data[i].adThumbnail2;
            prefabCoinDataComponent.adThumbnail2 = null;
            prefabCoinDataComponent.tracking = serverRawData.data[i].tracking;
            prefabCoinDataComponent.isBigcoin = serverRawData.data[i].isBigcoin;
            prefabCoinDataComponent.symbol = serverRawData.data[i].symbol;
            prefabCoinDataComponent.brandLogo = serverRawData.data[i].brandLogo;
            prefabCoinDataComponent.brandLogo = null;
            prefabCoinDataComponent.symbolimg = serverRawData.data[i].symbolimg;
            prefabCoinDataComponent.symbolimg = null;
            prefabCoinDataComponent.exad = serverRawData.data[i].exad;
            prefabCoinDataComponent.exco = serverRawData.data[i].exco;

            LocationData locationData = ScriptableObject.CreateInstance<LocationData>();
            PlaceAtLocation.LocationSettingsData locationSettinsData = new PlaceAtLocation.LocationSettingsData();
            locationData.Location = new Location();

            locationData.Location.Latitude = double.Parse(prefabCoinDataComponent.lat, System.Globalization.CultureInfo.InvariantCulture);
            locationData.Location.Longitude = double.Parse(prefabCoinDataComponent.lng, System.Globalization.CultureInfo.InvariantCulture);
            locationData.Location.Altitude = (rand.NextDouble() + 0.2);
            locationData.Location.AltitudeMode = AltitudeMode.GroundRelative;
            locationData.Location.Label = prefabCoinDataComponent.coin;

            locationSettinsData.LocationInput.LocationInputType = LocationPropertyData.LocationPropertyType.LocationData;
            locationSettinsData.LocationInput.LocationData = locationData;
            locationSettinsData.LocationInput.Location = locationData.Location;
            locationSettinsData.LocationInput.OverrideAltitudeData.OverrideAltitude = false;

            AddLocation(locationSettinsData.LocationInput.Location, myPrefab, i);

            //OnCoinSpawn();

        }
        OnCoinFinishIterating();
    }

    public void UpdatingExistingCoin()
    {
        
        Transform ARLocationRootTransform = ARLocationManager.Instance.transform;
        if (ARLocationRootTransform.childCount > 0)
        {
            foreach (Transform coin in ARLocationRootTransform)
            {
                CoinData coinData = coin.GetComponent<CoinData>();
                var newLocation = new Location()
                {
                    Latitude = Convert.ToDouble(coinData.lat),
                    Longitude = Convert.ToDouble(coinData.lng),
                    AltitudeMode = AltitudeMode.GroundRelative
                };
                var placeAtLocation = coin.GetComponent<PlaceAtLocation>();
                placeAtLocation.Location = newLocation;
            }
        }
        debugText.text = "Updating coin location and available coin UI";
        OnTrackingRestored();
    }
}

