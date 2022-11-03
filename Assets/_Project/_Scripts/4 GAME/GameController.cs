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
    [Serializable]
    public class AllCoinData
    {
        public List<CoinData> data;
    }
    // Controlling
    Animation anim;
    [SerializeField] Image buttonImage;
    [SerializeField] RawImage mapImage;
    [SerializeField] GameObject MapItems;
    [SerializeField] GameObject mapLighting;

    [SerializeField] GameObject arSession;
    [SerializeField] GameObject arSessionOrigin;
    
    [SerializeField] TMP_Text loadingText;
    [SerializeField] GameObject loadingPanel;

    public AllCoinData serverRawData;

    int spawnAmount = 0;
    bool gameIsOn;
    [SerializeField] GameMode gameMode = GameMode.Map;

    public event Action OnModeChanged;
    public event Action<AllCoinData, Location> OnAllCoinDataRetreived;
    public event Action OnAllCoinDataFailed;
    private void Awake()
    {
        anim = GetComponent<Animation>();
        gameMode = GameMode.None;
        loadingPanel.SetActive(true);
        ARLocationProvider.Instance.OnEnabled.AddListener(StartCallServer);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        // Empty on purpose
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        gameIsOn = !gameIsOn;
        if (gameIsOn)
        {
            gameMode = GameMode.Catch;
            Animate();
            ChangeButtonSprite();
            ChangeGameMode(gameMode);


            OnModeChanged();
        }
        else
        {
            gameMode = GameMode.Map;
            Animate();
            ChangeButtonSprite();
            ChangeGameMode(gameMode);

            OnModeChanged();
        }
    }
    public GameMode GetGameMode()
    {
        return gameMode;
    }
    private void ChangeButtonSprite()
    {
        if (gameIsOn)
        {
            buttonImage.sprite = Resources.Load<Sprite>("GameMode/Cam");
        }
        else
        {
            
            buttonImage.sprite = Resources.Load<Sprite>("GameMode/Map");
        }
    }

    private void Animate()
    {
        if (gameIsOn)
        {
            
            anim.Play("ToCatchMode");
        }
        else
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

                //Catch
                arSession.SetActive(false);
                arSessionOrigin.SetActive(false);

                    break;
                case GameMode.Catch:
                // Map
                mapImage.gameObject.SetActive(false);
                MapItems.gameObject.SetActive(false);
                mapLighting.gameObject.SetActive(false);

                // Catch
                arSession.SetActive(true);
                arSessionOrigin.SetActive(true);
                break;

        }
    }
    void StartCallServer(Location _)
    {

        if (spawnAmount > 0)
        {
            //PopulateCoins(serverRawData);
        }
        else if (spawnAmount == 0 || spawnAmount < 1)
        {
            int randomSpawnAmount = UnityEngine.Random.Range(200, 501);
            spawnAmount = randomSpawnAmount;
            StartCoroutine(CallServer(spawnAmount));
        }
    }
    IEnumerator CallServer(int spawnAmountFinal)
    {
        if (loadingText != null)loadingText.text = "Please wait we are loading your data from server...";
        if (!loadingPanel.activeSelf)
        {
            loadingPanel.SetActive(true);
        }

        // retreive player geo location
        Location playerLocation = ARLocationManager.Instance.GetLocationForWorldPosition(Camera.main.gameObject.transform.position);
        var latitudeApi = playerLocation.Latitude;
        var longitudeApi = playerLocation.Longitude;

        // AF events
        //AppsFlyer.recordLocation(playerLocation.Latitude, playerLocation.Longitude);

        if (playerLocation == null)
        {
            //if (OnUserLocationNotEnabled != null)
            //{
            //    OnUserLocationNotEnabled();
            //    yield break;
            //}
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

                Debug.Log(www.error);
                loadingPanel.SetActive(false);
            }
            else
            {

                // cahching request response
                var rawData = www.downloadHandler.text;
                // store into costum class

                serverRawData = JsonConvert.DeserializeObject<AllCoinData>(rawData);

                loadingPanel.SetActive(false);

                if (OnAllCoinDataRetreived != null)
                {
                    OnAllCoinDataRetreived(serverRawData, playerLocation);
                }

                ChangeGameMode(GameMode.Map);
            }
        }
    }
}
