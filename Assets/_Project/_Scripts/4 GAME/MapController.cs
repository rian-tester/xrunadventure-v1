using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARLocation;
using System;

public class MapController : MonoBehaviour
{
    OnlineMapsMarker playerMarker;
    [SerializeField] MultipleCoinPlacement coinPlacement;
    [SerializeField] Texture2D silverCoinTex;
    [SerializeField] GameController gameController;

    AllCoinData thisAllCoinData = new AllCoinData();
    Location thisPlayerLocation = new Location();
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
                "DragLocation Service not found.\nAdd DragLocation Service Component (Component / Infinity Code / Online Maps / Plugins / DragLocation Service).");
            return;
        }

        // Subscribe to the change Online Map location event.
        locationService.OnLocationChanged += OnLocationChanged;
        gameController.OnPlayerLocationRetreived += SetupMap;
        gameController.OnAllCoinDataRetreived += SetCoinData;
    }
    private void OnEnable()
    {
        //coinPlacement.OnServerDataChanged += SetupMap;
        OnlineMaps.instance.OnChangeZoom += OnZoomChanged;
        gameController.OnPlayerLocationRetreived += SetupMap;
        gameController.OnAllCoinDataRetreived+= SetCoinData;
    }
    private void OnDisable()
    {
        //coinPlacement.OnServerDataChanged -= SetupMap;
        OnlineMaps.instance.OnChangeZoom -= OnZoomChanged;
        gameController.OnPlayerLocationRetreived -= SetupMap;
        gameController.OnAllCoinDataRetreived -= SetCoinData;
    }


    public void SetupMap(Location playerLocation)
    {
        thisPlayerLocation = playerLocation;
        OnlineMaps.instance.SetPositionAndZoom(thisPlayerLocation.Longitude, thisPlayerLocation.Latitude,18 );


        if (OnMapSetup != null)
        {
            OnMapSetup();
        }
    }

    public void SetCoinData(AllCoinData serverCoinData)
    {
        thisAllCoinData = serverCoinData;
    }

    public void CenterMap()
    {
        // TO DO
        var playerLocation = thisPlayerLocation;
        OnlineMaps.instance.SetPositionAndZoom(playerLocation.Longitude, playerLocation.Latitude, 18);
    }

    public void PopulateCoinMarker()
    {
        if (!CanPlaceCoinMarker(gameController.GetGameMode())) return;
        if (thisAllCoinData == null) return;

        int i = 0;

        foreach(CoinData coin in thisAllCoinData.data)
        {
            if (i > 100) return ;
            double latitude = double.Parse(coin.Lat); 
            double longitude = double.Parse(coin.Lng);
            OnlineMapsMarker marker = OnlineMapsMarkerManager.CreateItem(longitude, latitude, silverCoinTex, coin.Coin);
            i++;
        }
    }

    public void UnpopulateCoinMarker()
    {
        if (CanPlaceCoinMarker(gameController.GetGameMode())) return;
        if (thisAllCoinData == null) return;

        for (int i = 1; i < OnlineMapsMarkerManager.instance.items.Count; i++)
        {
            OnlineMapsMarkerManager.RemoveItemAt(i);
        }
    }

    // When the location has changed
    void OnLocationChanged(Vector2 position)
    {
        // Change the position of the marker.
        playerMarker.position = position;

        // Redraw map.
        OnlineMaps.instance.Redraw();
    }

    void OnZoomChanged()
    {
        // TO DO
        var playerLocation = thisPlayerLocation;
        playerMarker.SetPosition(playerLocation.Longitude, playerLocation.Latitude);
        // Redraw map.
        OnlineMaps.instance.Redraw();
    }

    public OnlineMapsMarker GetPlayerMarker()
    {
        return playerMarker;
    }

    bool CanPlaceCoinMarker(GameMode currentGameMode)
    {
        if (currentGameMode == GameMode.Map)
        {
            return true;
        }
        return false;
    }
}