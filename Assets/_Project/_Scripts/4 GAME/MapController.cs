using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARLocation;
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using static OnlineMapsMapboxClipper;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Collections.Specialized;
using System.Web;
using TMPro;

public class MapController : MonoBehaviour
{
    OnlineMapsMarker playerMarker;
    Location thisPlayerLocation;
    [SerializeField] MultipleCoinPlacement coinPlacement;
    [SerializeField] Texture2D silverCoinTex;
    [SerializeField] GameController gameController;
    [SerializeField] TMP_Text debugText;
    [SerializeField] TMP_Text loadingText;

    public AllCoinData thisAllCoinData;

    // This event is notfying thepinch script for hand gesture
    public event Action OnMapSetup;
    public event Action<string, string> OnCoinMarkerPopulated;

    public bool isCoinPopulated;

    private void Start()
    {
        thisAllCoinData = new AllCoinData();
        thisPlayerLocation = new Location();
        
        OnlineMapsLocationService locationService = OnlineMapsLocationService.instance;

        if (locationService == null)
        {
            Debug.LogError(
                "DragLocation Service not found.\nAdd DragLocation Service Component (Component / Infinity Code / Online Maps / Plugins / DragLocation Service).");
            return;
        }

        locationService.OnLocationChanged += OnLocationChanged;

        ARLocationProvider.Instance.OnEnabled.AddListener(SubscribeToArManager);
        if (OnlineMaps.instance != null) OnlineMaps.instance.OnChangeZoom += OnZoomChanged;

    }

    private void SubscribeToArManager(Location _)
    {
        StartCoroutine(StartCallServer(_));
    }

    private IEnumerator StartCallServer(Location _)
    {

        Location playerLocation = ARLocationManager.Instance.GetLocationForWorldPosition(Camera.main.gameObject.transform.position);
        thisPlayerLocation.Latitude = _.Latitude;
        thisPlayerLocation.Longitude = _.Longitude;

        // Call this function because has an event in it
        SetupMap(thisPlayerLocation);

        string endpoint = ServerDataStatic.GetGateway();
        UriBuilder uriBuilder = new UriBuilder(endpoint);
        uriBuilder.Port = -1;
        NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
        //server down
        //query["act"] = "app2000-01";
        query["act"] = "app2000-02";
        query["member"] = PlayerDataStatic.Member.ToString();
        query["limit"] = PlayerDataStatic.SpawnAmount.ToString();
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
                    debugText.text = www.error;
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    debugText.text = www.error;
                    break;
                case UnityWebRequest.Result.Success:

                    var rawData = www.downloadHandler.text;
                    thisAllCoinData = JsonConvert.DeserializeObject<AllCoinData>(rawData);

                    // Setup for player marker
                    playerMarker = OnlineMapsMarkerManager.CreateItem(thisPlayerLocation.Longitude, thisPlayerLocation.Latitude, null, "Player");

                    if (thisAllCoinData.data.Count > 0)
                    {
                        // coin marker populating
                        for (int i = 0; i < thisAllCoinData.data.Count; i++)
                        {
                            double latitude = double.Parse(thisAllCoinData.data[i].lat);
                            double longitude = double.Parse(thisAllCoinData.data[i].lng);
                            OnlineMapsMarker marker = OnlineMapsMarkerManager.CreateItem(longitude, latitude, silverCoinTex, thisAllCoinData.data[i].coin);
                            marker.scale = 0.5f;

                            if (debugText.gameObject.activeSelf)
                            {
                                debugText.text = $"Finish creating marker for coin number {i}";
                            }

                            OnlineMaps.instance.Redraw();
                        }

                        if (OnCoinMarkerPopulated != null)
                        {
                            OnCoinMarkerPopulated(thisAllCoinData.data[0].advertisement.ToString(), thisAllCoinData.data.Count.ToString());
                        }

                        isCoinPopulated = true;

                    }
                    else if (thisAllCoinData.data.Count == 0 || thisAllCoinData.data.Count < 0)
                    {
                        loadingText.text = "We're very sorry. Either your device is not compatible to retrieve our data or our service is not available in your region yet";
                        isCoinPopulated = true;
                    }


                    break;
            }
        }

    }

    private void SetupMap(Location playerLocation)
    {
        OnlineMaps.instance.SetPositionAndZoom(playerLocation.Longitude, playerLocation.Latitude,18 );

        if (OnMapSetup != null)
        {
            OnMapSetup();
        }
    }


    // When the location has changed
    private void OnLocationChanged(Vector2 position)
    {
        // Change the position of the marker.
        playerMarker.position = position;

        // Redraw map.
        OnlineMaps.instance.Redraw();
    }

    // redraw map based on zoom level
    private void OnZoomChanged()
    {

        if (thisPlayerLocation.Latitude == 0 || thisPlayerLocation.Longitude == 0)
        {
            Location playerLocationFromAr = ARLocationManager.Instance.GetLocationForWorldPosition(Camera.main.gameObject.transform.position);
            thisPlayerLocation.Latitude = playerLocationFromAr.Latitude;
            thisPlayerLocation.Longitude = playerLocationFromAr.Longitude;
        }

        if (playerMarker != null)
        {
            playerMarker.SetPosition(thisPlayerLocation.Longitude, thisPlayerLocation.Latitude);
        }
        
        // Redraw map.
        OnlineMaps.instance.Redraw();
    }

   

    // This called from UI Button
    public void CenterMap()
    {
        var playerLocation = thisPlayerLocation;
        OnlineMaps.instance.SetPositionAndZoom(playerLocation.Longitude, playerLocation.Latitude, 18);
    }

}