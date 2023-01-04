using ARLocation;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class MultipleCoinPlacement : PlaceAtLocations
{
    [Space(2.0f)]
    [Header("Gameobject Setup")]
    [SerializeField] GameObject myPrefab;
    [SerializeField] Camera arCamera;

    [Space(2.0f)]
    [Header("Spawning Setup")]
    [SerializeField] GameController gameController;
    Location thisPlayerLocation;
 

    public AllCoinData serverRawData;

    public bool isCoinPopulated;

    [Space(2.0f)]
    [Header("Debugging")]
    [SerializeField] TMP_Text debugText;
    [SerializeField] TMP_Text loadingText;
    private void Start()
    {
        serverRawData = new AllCoinData();
        thisPlayerLocation = new Location();

        ARLocationProvider.Instance.OnEnabled.AddListener(SubscribeToArManager);
    }

    private void SubscribeToArManager(Location _)
    {
        StartCoroutine(StartCallServer());
    }

    private IEnumerator StartCallServer()
    {
        thisPlayerLocation = ARLocationManager.Instance.GetLocationForWorldPosition(Camera.main.gameObject.transform.position);

        string endpoint = ServerDataStatic.GetGateway();
        UriBuilder uriBuilder = new UriBuilder(endpoint);
        uriBuilder.Port = -1;
        NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["act"] = "app2000-01";
        query["member"] = PlayerDataStatic.Member.ToString();
        query["limit"] = PlayerDataStatic.SpawnAmount.ToString();
        //query["limit"] = 10.ToString();
        query["lat"] = thisPlayerLocation.Latitude.ToString();
        query["lng"] = thisPlayerLocation.Longitude.ToString();
        query["os"] = "3114";
        uriBuilder.Query = query.ToString();
        endpoint = uriBuilder.ToString();

        // web request to server
        using (UnityWebRequest www = UnityWebRequest.Get(endpoint))
        {
            yield return www.SendWebRequest();

            switch (www.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    if (debugText.gameObject.activeSelf)
                    {
                        debugText.text = www.error;
                    }
                    
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    if (debugText.gameObject.activeSelf)
                    {
                        debugText.text = www.error;
                    }
                    
                    break;
                case UnityWebRequest.Result.Success:
                    var rawData = www.downloadHandler.text;
                    serverRawData = JsonConvert.DeserializeObject<AllCoinData>(rawData);

                    System.Random rand = new System.Random();

                    if (serverRawData.data.Count > 0)
                    {
                        for (int i = 0; i < serverRawData.data.Count; i++)
                        {
                            CoinDataComponent prefabCoinDataComponent = myPrefab.GetComponent<CoinDataComponent>();

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
                        }

                        isCoinPopulated = true;
                    }
                    else if (serverRawData.data.Count == 0 || serverRawData.data.Count < 0)
                    {
                        loadingText.text = "We're very sorry. Either your device is not compatible to retrieve our data or our service is not available in your region yet";
                        isCoinPopulated = true;
                    }
                    

                    break;
            }
        }

    }
}
