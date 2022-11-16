using ARLocation;
using System;
using TMPro;
using UnityEngine;

public class MultipleCoinPlacement : PlaceAtLocations
{
    [Space(2.0f)]
    [Header("Gameobject Setup")]
    [SerializeField] GameObject myPrefab;
    [SerializeField] Camera arCamera;

    [Space(2.0f)]
    [Header("Spawning Setup")]
    [SerializeField] GameController gameController;
    [SerializeField]
    AllCoinData serverRawData = new AllCoinData();

    [Space(2.0f)]
    [Header("Debugging")]
    [SerializeField]
    TMP_Text debugText;

    private void OnEnable()
    {
        //gameController.OnModeChanged += UpdateGameMode;
        gameController.OnAllCoinDataRetreived += SetServerData;
    }
    private void OnDisable()
    {
        //gameController.OnModeChanged -= UpdateGameMode;
        gameController.OnAllCoinDataRetreived -= SetServerData;
    }
    void SetServerData(AllCoinData serverData)
    {
        if (serverData != null)
        {
            serverRawData = serverData;
        }
        
    }

    public void PopulateCoins()
    {
        if (serverRawData == null) return;
        if (!CanPlaceCoins(gameController.GetGameMode())) return;

        if (debugText != null) debugText.text = serverRawData.ToString();
        System.Random rand = new System.Random();
        for (int i = 0; i < serverRawData.data.Count; i++)
        {
            CoinData prefabCoinDataComponent = myPrefab.GetComponent<CoinData>();

            prefabCoinDataComponent.Coin = serverRawData.data[i].Coin;
            prefabCoinDataComponent.Cointype = serverRawData.data[i].Cointype;
            prefabCoinDataComponent.Amount = serverRawData.data[i].Amount;
            prefabCoinDataComponent.Countlimit = serverRawData.data[i].Countlimit;
            prefabCoinDataComponent.Lng = serverRawData.data[i].Lng;
            prefabCoinDataComponent.Lat = serverRawData.data[i].Lat;
            prefabCoinDataComponent.Distance = serverRawData.data[i].Distance;
            prefabCoinDataComponent.Advertisement = serverRawData.data[i].Advertisement;
            prefabCoinDataComponent.Brand = serverRawData.data[i].Brand;
            prefabCoinDataComponent.Title = serverRawData.data[i].Title;
            prefabCoinDataComponent.Contents = serverRawData.data[i].Contents;
            prefabCoinDataComponent.Currency = serverRawData.data[i].Currency;
            prefabCoinDataComponent.AdColor1 = serverRawData.data[i].AdColor1;
            prefabCoinDataComponent.AdColor2 = serverRawData.data[i].AdColor2;
            prefabCoinDataComponent.Coins = serverRawData.data[i].Coins;
            prefabCoinDataComponent.AdThumbnail = serverRawData.data[i].AdThumbnail;
            prefabCoinDataComponent.AdThumbnail = null;
            prefabCoinDataComponent.AdThumbnail2 = serverRawData.data[i].AdThumbnail2;
            prefabCoinDataComponent.AdThumbnail2 = null;
            prefabCoinDataComponent.Tracking = serverRawData.data[i].Tracking;
            prefabCoinDataComponent.Isbigcoin = serverRawData.data[i].Isbigcoin;
            prefabCoinDataComponent.Symbol = serverRawData.data[i].Symbol;
            prefabCoinDataComponent.BrandLogo = serverRawData.data[i].BrandLogo;
            prefabCoinDataComponent.BrandLogo = null;
            prefabCoinDataComponent.Symbolimg = serverRawData.data[i].Symbolimg;
            prefabCoinDataComponent.Symbolimg = null;
            prefabCoinDataComponent.Exad = serverRawData.data[i].Exad;
            prefabCoinDataComponent.Exco = serverRawData.data[i].Exco;

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
        }
    }
    public void UnPopulateCoin()
    {
        if (serverRawData == null) return;
        if (CanPlaceCoins(gameController.GetGameMode())) return;

        Transform arLocationRoot = ARLocationManager.Instance.gameObject.transform;
        
        if (arLocationRoot.childCount > 0)
        {
            foreach (Transform child in arLocationRoot)
            {
                 
                Destroy(child.gameObject);
            }
        }
        Debug.Log($"total object in AR DragLocation root {arLocationRoot.childCount}");
    }
    bool CanPlaceCoins(GameMode currentGameMode)
    {
        if (currentGameMode == GameMode.Catch)
        {
            return true;
        }
        return false;
    }

}
