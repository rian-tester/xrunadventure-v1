using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARLocation;
using System;

public class MapController : MonoBehaviour
{
    [SerializeField] OnlineMapsMarker playerMarker;
    [SerializeField] MultipleCoinPlacement coinPlacement;
    [SerializeField] Texture2D silverCoinTex;

    public event Action OnMapSetup;
    private void Start()
    {
        // Create a new marker.
        playerMarker = OnlineMapsMarkerManager.CreateItem(new Vector2(0, 0), null, "Player");

        // Get instance of LocationService.
        OnlineMapsLocationService locationService = OnlineMapsLocationService.instance;

        if (locationService == null)
        {
            Debug.LogError(
                "Location Service not found.\nAdd Location Service Component (Component / Infinity Code / Online Maps / Plugins / Location Service).");
            return;
        }

        // Subscribe to the change location event.
        locationService.OnLocationChanged += OnLocationChanged;
    }
    private void OnEnable()
    {
        coinPlacement.OnServerDataChanged += SetupMap;
        OnlineMaps.instance.OnChangeZoom += OnZoomChanged;
    }
    private void OnDisable()
    {
        coinPlacement.OnServerDataChanged -= SetupMap;
        OnlineMaps.instance.OnChangeZoom -= OnZoomChanged;
    }


    public void SetupMap()
    {
        var playerLocation = ARLocationManager.Instance.GetLocationForWorldPosition(Camera.main.gameObject.transform.position);
        OnlineMaps.instance.SetPositionAndZoom(playerLocation.Longitude, playerLocation.Latitude,18 );
        PopulateCoin();

        if (OnMapSetup != null)
        {
            OnMapSetup();
        }
    }

    public void CenterMap()
    {
        var playerLocation = ARLocationManager.Instance.GetLocationForWorldPosition(Camera.main.gameObject.transform.position);
        OnlineMaps.instance.SetPositionAndZoom(playerLocation.Longitude, playerLocation.Latitude, 18);
    }

    public void PopulateCoin()
    {
        MultipleCoinPlacement.ServerCoinData rawServerData = coinPlacement.GetRawServerData();

        int i = 0;

        foreach(CoinData coin in rawServerData.data)
        {
            if (i > 100) return ;
            double latitude = double.Parse(coin.Lat); 
            double longitude = double.Parse(coin.Lng);
            OnlineMapsMarker marker = OnlineMapsMarkerManager.CreateItem(longitude, latitude, silverCoinTex, coin.Coin);
            i++;
        }
    }

    // When the location has changed
    void OnLocationChanged(Vector2 position)
    {
        // Change the position of the marker.
        playerMarker.position = position;
        PopulateCoin();

        // Redraw map.
        OnlineMaps.instance.Redraw();
    }

    void OnZoomChanged()
    {
        var playerLocation = ARLocationManager.Instance.GetLocationForWorldPosition(Camera.main.gameObject.transform.position);
        playerMarker.SetPosition(playerLocation.Longitude, playerLocation.Latitude);
        PopulateCoin();
    }

    public OnlineMapsMarker GetPlayerMarker()
    {
        return playerMarker;
    }
}