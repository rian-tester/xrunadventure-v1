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
            Debug.LogWarning("Server data still exist so populate from last data");
            debugText.text = "Server data still exist so populate from last data";
            PopulateCoins(serverRawData);
        }
        else if (spawnAmount == 0 || spawnAmount < 1)
        {
            Debug.LogWarning("No data in server, so call server again");
            debugText.text = "No data in server, so call server again";
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
                debugText.text = "succesfully calling raw data from server";
                // cahching request response
                var rawData = www.downloadHandler.text;
                // store into costum class
                serverRawData = new ServerCoinData();
                serverRawData = JsonConvert.DeserializeObject<ServerCoinData>(rawData);
                serverResults = rawData;
                debugText.text = "succesfully deserialize server data into custom class";

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

            prefabCoinDataComponent.Coin = coinsData.data[i].Coin;
            prefabCoinDataComponent.Cointype = coinsData.data[i].Cointype;
            prefabCoinDataComponent.Amount = coinsData.data[i].Amount;
            prefabCoinDataComponent.Countlimit = coinsData.data[i].Countlimit;
            prefabCoinDataComponent.Lng = coinsData.data[i].Lng;
            prefabCoinDataComponent.Lat = coinsData.data[i].Lat;
            prefabCoinDataComponent.Distance = coinsData.data[i].Distance;
            prefabCoinDataComponent.Advertisement = coinsData.data[i].Advertisement;
            prefabCoinDataComponent.Brand = coinsData.data[i].Brand;
            prefabCoinDataComponent.Title = coinsData.data[i].Title;
            prefabCoinDataComponent.Contents = coinsData.data[i].Contents;
            prefabCoinDataComponent.Currency = coinsData.data[i].Currency;
            prefabCoinDataComponent.AdColor1 = coinsData.data[i].AdColor1;
            prefabCoinDataComponent.AdColor2 = coinsData.data[i].AdColor2;
            prefabCoinDataComponent.Coins = coinsData.data[i].Coins;
            //prefabCoinDataComponent.AdThumbnail = coinsData.data[i].AdThumbnail;
            prefabCoinDataComponent.AdThumbnail = null;
            //prefabCoinDataComponent.AdThumbnail2 = coinsData.data[i].AdThumbnail2;
            prefabCoinDataComponent.AdThumbnail2 = null;
            prefabCoinDataComponent.Tracking = coinsData.data[i].Tracking;
            prefabCoinDataComponent.Isbigcoin = coinsData.data[i].Isbigcoin;
            prefabCoinDataComponent.Symbol = coinsData.data[i].Symbol;
            //prefabCoinDataComponent.BrandLogo = coinsData.data[i].BrandLogo;
            prefabCoinDataComponent.BrandLogo = null;
            //prefabCoinDataComponent.Symbolimg = coinsData.data[i].Symbolimg;
            prefabCoinDataComponent.Symbolimg = null;
            prefabCoinDataComponent.Exad = coinsData.data[i].Exad;
            prefabCoinDataComponent.Exco = coinsData.data[i].Exco;

            LocationData locationData = ScriptableObject.CreateInstance<LocationData>();
            PlaceAtLocation.LocationSettingsData locationSettinsData = new PlaceAtLocation.LocationSettingsData();
            locationData.Location = new Location();

            locationData.Location.Latitude = double.Parse(prefabCoinDataComponent.Lat, System.Globalization.CultureInfo.InvariantCulture);
            locationData.Location.Longitude = double.Parse(prefabCoinDataComponent.Lng, System.Globalization.CultureInfo.InvariantCulture);
            locationData.Location.Altitude = (rand.NextDouble() + 0.2);
            locationData.Location.AltitudeMode = AltitudeMode.GroundRelative;
            locationData.Location.Label = prefabCoinDataComponent.Coin;

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
                    Latitude = Convert.ToDouble(coinData.Lat),
                    Longitude = Convert.ToDouble(coinData.Lng),
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

