using AppsFlyerSDK;
using ARLocation;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.XR.ARSubsystems.XRCpuImage;

public class MultipleCoinPlacement : PlaceAtLocations
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
    [SerializeField] GameObject loadingPanel;
    [SerializeField] TMP_Text loadingText;

    [Space(2.0f)]
    [Header("Spawning Setup")]
    [SerializeField] GameController gameController;
    ServerCoinData serverRawData = new ServerCoinData();
    [TextArea(5, 8)]
    public string serverResults;

    [SerializeField] int spawnAmount;
    double latitudeApi;
    double longitudeApi;
    string advertisement;

    [Space(2.0f)]
    [Header("Debugging")]
    [SerializeField]
    TMP_Text debugText;

    GameMode gameMode;

    //public event Action OnCoinSpawn;
    //public event Action OnStartCallServer;
    public event Action OnFinishCallServer;
    //public event Action OnCoinFinishIterating;
    //public event Action OnCoinStartIterating;
    //public event Action OnTrackingRestored;
    //public event Action OnPlayerLocationEnabled;
    public event Action OnServerDataChanged;
    public event Action OnUserLocationNotEnabled;
    private void Awake()
    {
        gameMode = GameMode.Map;
        loadingPanel.SetActive(true);
        loadingText.text = "Waiting for your location service enable...";
    }
    private void OnEnable()
    {
        gameController.OnModeChanged += UpdateGameMode;
        ARLocationProvider.Instance.OnEnabled.AddListener(StartPopulateCoins);
    }
    private void OnDisable()
    {
        gameController.OnModeChanged -= UpdateGameMode;
    }

    public int GetAmountCoinCalledFromServer()
    {
        return serverRawData.data.Count;
    }

    public ServerCoinData GetRawServerData()
    {
        return serverRawData;
    }
    void StartPopulateCoins(Location _)
    {
        
        if (spawnAmount > 0)
        {
            Debug.LogWarning("Server data still exist so populate from last data");
            if (debugText != null) debugText.text = "Server data still exist so populate from last data";
            PopulateCoins(serverRawData);
        }
        else if (spawnAmount == 0 || spawnAmount < 1)
        {
            Debug.LogWarning("No data in server, so call server again");
            if (debugText != null) debugText.text = "No data in server, so call server again";
            int randomSpawnAmount = UnityEngine.Random.Range(200, 501);
            spawnAmount = randomSpawnAmount;
            StartCoroutine(CallServer(spawnAmount));
        }
    }

    // ARLocationManager.Instance.GetLocationForWorldPosition() --> to get lat lon from Unity transform
    IEnumerator CallServer(int spawnAmountFinal)
    {
        loadingText.text = "Please wait we are loading your data from server...";
        if (!loadingPanel.activeSelf)
        {
            loadingPanel.SetActive(true);
        }
        UnPopulateCoin();       
        // retreive player geo location
        Location playerLocation = ARLocationManager.Instance.GetLocationForWorldPosition(arCamera.gameObject.transform.position);
        latitudeApi = playerLocation.Latitude;
        longitudeApi = playerLocation.Longitude;

        // AF events

        AppsFlyer.recordLocation(playerLocation.Latitude, playerLocation.Longitude);

        


        if (playerLocation == null)
        {
            if (OnUserLocationNotEnabled != null)
            {
                OnUserLocationNotEnabled();
                yield break;
            }
        }

        if (latitudeApi == 0 || longitudeApi == 0)
        {
            loadingText.text = "We cannot get your exact location, please check your location setting";
        }

        // building query
        string endpoint = ServerDataStatic.GetGateway();
        var uriBuilder = new UriBuilder(endpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["act"] = "app2000-01";
        if (PlayerDataStatic.Member != null)
        {
            query["member"] = PlayerDataStatic.Member.ToString();
        }
        query["limit"] = spawnAmountFinal.ToString();
        query["lat"] = latitudeApi.ToString();
        query["lng"] = longitudeApi.ToString();
        query["os"] = "3114";
        uriBuilder.Query = query.ToString();
        endpoint = uriBuilder.ToString();
        if (debugText != null) debugText.text = "finish building query";
        // web request to server
        using (UnityWebRequest www = UnityWebRequest.Get(endpoint))
        {
            yield return www.SendWebRequest();
            if (debugText != null) debugText.text = "finish calling the server api";
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                loadingPanel.SetActive(false);
            }
            else
            {
                if (debugText != null) debugText.text = "succesfully calling raw data from server";
                // cahching request response
                var rawData = www.downloadHandler.text;
                // store into costum class

                serverRawData = JsonConvert.DeserializeObject<ServerCoinData>(rawData);
                serverResults = rawData;
               
                loadingPanel.SetActive(false);

                if (OnServerDataChanged != null)
                {
                    OnServerDataChanged();
                }
               
                PopulateCoins(serverRawData);
                if (debugText != null) debugText.text = "succesfully deserialize server data into custom class";
                if (debugText != null) debugText.text = $"Server Called finish calling {spawnAmountFinal} coins";
            }
        }
        print($"Data contain {serverRawData.data.Count} from server");

        if (OnFinishCallServer != null)
        {
            OnFinishCallServer();
        }

    }
    public void PopulateCoins(ServerCoinData coinsData)
    {
        //OnCoinStartIterating();


        if (serverRawData == null) return;
        if (!CanPlaceCoins(gameMode)) return;

        if (debugText != null) debugText.text = coinsData.ToString();
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
            prefabCoinDataComponent.AdThumbnail = coinsData.data[i].AdThumbnail;
            prefabCoinDataComponent.AdThumbnail = null;
            prefabCoinDataComponent.AdThumbnail2 = coinsData.data[i].AdThumbnail2;
            prefabCoinDataComponent.AdThumbnail2 = null;
            prefabCoinDataComponent.Tracking = coinsData.data[i].Tracking;
            prefabCoinDataComponent.Isbigcoin = coinsData.data[i].Isbigcoin;
            prefabCoinDataComponent.Symbol = coinsData.data[i].Symbol;
            prefabCoinDataComponent.BrandLogo = coinsData.data[i].BrandLogo;
            prefabCoinDataComponent.BrandLogo = null;
            prefabCoinDataComponent.Symbolimg = coinsData.data[i].Symbolimg;
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
        //OnCoinFinishIterating();
    }
    void UnPopulateCoin()
    {
        Transform arLocationRoot = ARLocationManager.Instance.gameObject.transform;
        
        if (arLocationRoot.childCount > 0)
        {
            foreach (Transform child in arLocationRoot)
            {
                 
                Destroy(child.gameObject);
            }
        }
        print($"total object in AR Location root {arLocationRoot.childCount}");
    }
    bool CanPlaceCoins(GameMode currentGameMode)
    {
        if (gameMode == GameMode.Catch)
        {
            return true;
        }
        return false;
    }
    void UpdateGameMode()
    {
        gameMode = gameController.GetGameMode();
        UnPopulateCoin();
        PopulateCoins(serverRawData);
    }
}
