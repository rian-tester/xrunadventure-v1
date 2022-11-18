using AppsFlyerSDK;
using ARLocation;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameController : MonoBehaviour,IPointerDownHandler, IPointerUpHandler
{

    // Controlling
    Animation anim;
    [SerializeField] Image buttonImage;
    [SerializeField] RawImage mapImage;
    [SerializeField] GameObject MapItems;
    [SerializeField] GameObject mapLighting;

    [SerializeField] GameObject arSession;
    [SerializeField] GameObject arSessionOrigin;
    [SerializeField] MultipleCoinPlacement multipleCoinPlacement;
    [SerializeField] MapController mapController;
    [SerializeField] GameObject environmentLighting;
    
    [SerializeField] TMP_Text loadingText;
    [SerializeField] GameObject loadingPanel;
    [SerializeField]
    AllCoinData serverRawData;

    [SerializeField] TMP_Text debugText;
    [SerializeField] TMP_Text debugTextLocation;

    int spawnAmount = 10;
    double playerLatitude;
    double playerLongitude;

    [SerializeField] GameMode gameMode = GameMode.Map;

    public event Action OnModeChanged;
    public event Action<Location> OnPlayerLocationRetreived;
    public event Action<AllCoinData> OnAllCoinDataRetreived;
    public event Action OnAllCoinDataFailed;
    public event Action OnMapModeActive;
    public event Action OnCatchModeActive;

    private void Start()
    {
        anim = GetComponent<Animation>();
        gameMode = GameMode.None;
        loadingPanel.SetActive(true);
        loadingText.text = "Getting your location for server call";
        ARLocationProvider.Instance.OnEnabled.AddListener(StartCallServer);
        Debug.Log("listener added");
        debugText.text = "Awake complete";
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
        if (gameMode == GameMode.Map || gameMode == GameMode.None)
        {
            gameMode = GameMode.Catch;
            Animate();
            ChangeButtonSprite();
            ChangeGameMode(gameMode);

        }
        else if (gameMode == GameMode.Catch || gameMode == GameMode.None)
        {
            gameMode = GameMode.Map;
            Animate();
            ChangeButtonSprite();
            ChangeGameMode(gameMode);
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        // Empty on purpose
    }
    public GameMode GetGameMode()
    {
        return gameMode;
    }
    private void ChangeButtonSprite()
    {
        if (gameMode == GameMode.Catch)
        {
            buttonImage.sprite = Resources.Load<Sprite>("GameMode/Cam");
        }
        else if (gameMode == GameMode.Map)
        {
            
            buttonImage.sprite = Resources.Load<Sprite>("GameMode/Map");
        }
    }

    private void Animate()
    {
        if (gameMode == GameMode.None)
        {
            return;
        }
        else if (gameMode == GameMode.Catch)
        {
            
            anim.Play("ToCatchMode");
        }
        else if(gameMode == GameMode.Map)
        {
            anim.Play("ToMapMode");
        }
    }
    private void ChangeGameMode(GameMode gameMode)
    {
        switch (gameMode)
        {
            case GameMode.Map:
                // Map
                if (!mapImage.isActiveAndEnabled) mapImage.gameObject.SetActive(true);
                if (!MapItems.activeSelf) MapItems.gameObject.SetActive(true);
                OnlineMaps.instance.Redraw();
                mapLighting.gameObject.SetActive(true);
                //mapController.PopulateCoinMarker();

                //Catch
                multipleCoinPlacement.UnPopulateCoin();
                arSession.SetActive(false);
                arSessionOrigin.SetActive(false);
                environmentLighting.gameObject.SetActive(false);
                
                break;
                case GameMode.Catch:
                // Catch
                arSession.SetActive(true);
                arSessionOrigin.SetActive(true);
                multipleCoinPlacement.PopulateCoins();
                environmentLighting.gameObject.SetActive(true);

                // Map
                mapImage.gameObject.SetActive(false);
                MapItems.gameObject.SetActive(false);
                mapLighting.gameObject.SetActive(false);
                //mapController.UnpopulateCoinMarker();

                
                break;

            case GameMode.None:
                //
                break;

        }
    }
    void StartCallServer()
    {
        loadingText.text = "Your location is succesfully retrieved";
        if (spawnAmount > 0)
        {
            StartCoroutine(CallServer(spawnAmount));
        }
        else if (spawnAmount == 0 || spawnAmount < 1)
        {
            int randomSpawnAmount = UnityEngine.Random.Range(200, 501);
            spawnAmount = randomSpawnAmount;
            StartCoroutine(CallServer(spawnAmount));
        }
    }
    void StartCallServer(Location _)
    {
        loadingText.text = "Your location is succesfully retrieved";
        if (spawnAmount > 0)
        {
            StartCoroutine(CallServer(spawnAmount));    
        }
        else if (spawnAmount == 0 || spawnAmount < 1)
        {
            int randomSpawnAmount = UnityEngine.Random.Range(200, 501);
            spawnAmount = randomSpawnAmount;
            StartCoroutine(CallServer(spawnAmount));
            debugText.text = "Start call server complete";
        }
    }
    IEnumerator CallServer(int spawnAmountFinal)
    {
        debugText.text = "Call server begin ";
        if (loadingText != null)loadingText.text = "Please wait we are loading your data from server based on your location";
        if (!loadingPanel.activeSelf)
        {
            loadingPanel.SetActive(true);
        }

        if (playerLatitude == 0 || playerLongitude == 0)
        {
            // retreive player geo location
            Location playerLocation = ARLocationManager.Instance.GetLocationForWorldPosition(Camera.main.gameObject.transform.position);
            playerLatitude = playerLocation.Latitude;
            playerLongitude = playerLocation.Longitude;
            debugText.text = $"Retreive player geo location {playerLatitude}, {playerLatitude}";
        }
       

        // AF events
        //AppsFlyer.recordLocation(playerLocation.Latitude, playerLocation.Longitude);

        if (playerLatitude == 0 || playerLongitude == 0)
        {
            debugText.text = "Lat Lon still zero after try to get";
            //if (OnUserLocationNotEnabled != null)
            //{
            //    OnUserLocationNotEnabled();
            //    yield break;
            //}
            loadingText.text = "It look like something wrong with your location data, please activate your location service or give access to our app";
            yield return new WaitForSeconds(2f);
            loadingPanel.SetActive(false);
            yield break;
        }



        // building query
        debugText.text = "Start building query";
        string endpoint = ServerDataStatic.GetGateway();
        var uriBuilder = new UriBuilder(endpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["act"] = "coinmapping";
        query["member"] = PlayerDataStatic.Member.ToString();
        query["limit"] = spawnAmountFinal.ToString();
        query["lat"] = playerLatitude.ToString();
        query["lng"] = playerLongitude.ToString();
        query["os"] = "3114";
        uriBuilder.Query = query.ToString();
        endpoint = uriBuilder.ToString();

        // web request to server
        using (UnityWebRequest www = UnityWebRequest.Get(endpoint))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                if (OnAllCoinDataFailed != null)
                {
                    OnAllCoinDataFailed();
                }
                debugText.text = "Web request failed ";
                Debug.Log(www.error);
                loadingPanel.SetActive(false);
            }
            else
            {

                // cahching request response
                string rawData = www.downloadHandler.text;
                // store into costum class
                serverRawData = new AllCoinData();
                serverRawData = JsonConvert.DeserializeObject<AllCoinData>(rawData);
                if (serverRawData.data.Count == 0)
                {
                    debugText.text = "Waiting 5 second because data array is 0";
                    yield return new WaitForSeconds(5f);
                }

                
                debugTextLocation.text = $"Device Location result = {playerLatitude}, {playerLongitude}";
                debugText.text = $"Raw server result = {www.downloadHandler.text}";

                if (OnPlayerLocationRetreived != null)
                {
                    Location playerLocation = new Location(playerLatitude, playerLongitude);
                    OnPlayerLocationRetreived(playerLocation);
                }

                
                loadingPanel.SetActive(false);
                gameMode = GameMode.Map;
                ChangeGameMode(gameMode);

                if (OnAllCoinDataRetreived != null)
                {

                  if (serverRawData.data.Count > 0) OnAllCoinDataRetreived(serverRawData);

                }

            }
        }
    }
}
