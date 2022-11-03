using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using ARLocation;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Web;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

// This class placed in independent Object called Player
// Exist in hierarchy

// This class handle : 
// Raycasting from centre of screen
// Play Crosshair Animation when detecting coin
// checking coin status when collecting/server check
// if true do trigger :
// Trigger bool in coin blinking
// Trigger bool in coin taken
// if false show error messages

public class Player : MonoBehaviour
{
    [SerializeField] XrunRewarded rewardedAd;
    [SerializeField] MyGameManager gameManager;
    [SerializeField] Camera arCamera;
    [SerializeField] Image crossHair = null;
    [SerializeField] Animator uiAnimator;
    [SerializeField] Transform coinsParentTransform;
    [SerializeField] List<Transform> spawnerLocations = new List<Transform>();
    Coin catchedCoin;
    [SerializeField] float normalRaycastDistance;
    [SerializeField] float arrowRaycastDistance;
    float raycastDistance;
    Vector3 originPosition;
    Vector3 direction;
    [SerializeField] bool arrowPowerOn;

    public LayerMask layerMask;
    public float radius = 5f;

    public event Action OnPlayerCatchCoin;

    private void Awake()
    {
        raycastDistance = normalRaycastDistance;
        string playerEmail = PlayerPrefs.GetString("email");
        StartCoroutine(StorePlayerData(playerEmail));
    }
    private void Start()
    {


        gameManager.OnUserDisagreeWatchAds += CancelCoinRewardingProcess;
        // listen when user finish watch ads execute RewardCoinToPlayer()
        rewardedAd.OnAdComplete += RewardCoinToPlayer;
        // listen when user not finish watch ads execute CancelCoinRewardProcess()
        rewardedAd.OnAdCancel += CancelCoinRewardingProcess;

    }

    private void OnDestroy()
    {
        gameManager.OnUserDisagreeWatchAds -= CancelCoinRewardingProcess;
        rewardedAd.OnAdComplete -= RewardCoinToPlayer;
        rewardedAd.OnAdCancel -= CancelCoinRewardingProcess;

    }
    private void Update()
    {
        #region NewCatchSystem
        if (arrowPowerOn)
        {
            ArrowCatch();
        }
        else
        {
            //Catch();
        }

        Catch();

        #endregion
    }

    void Catch()
    {
        if (!arCamera.isActiveAndEnabled) return;
        Ray ray;

        if (Input.touchCount > 0)
        {
            Touch theTouch = Input.GetTouch(0);
            ray = Camera.main.ScreenPointToRay(theTouch.position);
            originPosition = ray.origin;
            direction = ray.direction;
            Debug.DrawRay(ray.origin, ray.direction * raycastDistance);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, raycastDistance, layerMask))
            {
                Coin coin = hit.collider.gameObject.GetComponent<Coin>();
                coin.ValidatingCoin(this);
            }
        }
    }

    void ArrowCatch()
    {
        Ray ray;

        if (Input.touchCount > 0)
        {
            Touch theTouch = Input.GetTouch(0);
            ray = Camera.main.ScreenPointToRay(theTouch.position);
            originPosition = ray.origin;
            direction = ray.direction;
            Debug.DrawRay(ray.origin, ray.direction * raycastDistance);

            RaycastHit hit;
            if (Physics.SphereCast(originPosition, radius, direction, out hit, raycastDistance, layerMask))
            {
                Coin coin = hit.collider.gameObject.GetComponent<Coin>();
                coin.ValidatingCoin(this);
            }
        }
    }
    public static IEnumerator StorePlayerData(string email)
    {
        // building query
        string endpoint = ServerDataStatic.GetGateway();
        var uriBuilder = new UriBuilder(endpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["act"] = "login-04-email";
        query["email"] = email;
        query["code"] = "3114";

        uriBuilder.Query = query.ToString();
        endpoint = uriBuilder.ToString();

        // requesting email verification
        using (UnityWebRequest www = UnityWebRequest.Get(endpoint))
        {
            // sending data request
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                // cahching request response
                var rawData = www.downloadHandler.text;
                PlayerData responseData = new PlayerData();
                responseData = JsonConvert.DeserializeObject<PlayerData>(rawData);

                PlayerDataStatic.SetEmail(responseData.email);
                PlayerDataStatic.SetMemberNumber(responseData.member);
                PlayerDataStatic.SetFirstName(responseData.firstname);
                PlayerDataStatic.SetLastName(responseData.lastname);
                PlayerDataStatic.SetGender(responseData.gender);
                PlayerDataStatic.SetExtrastr(responseData.extrastr);
                PlayerDataStatic.SetMobileCode(responseData.mobilecode);
                PlayerDataStatic.SetCountry(responseData.country);
                PlayerDataStatic.SetCountryCode(responseData.countrycode);
                PlayerDataStatic.SetRegion(responseData.region);
                PlayerDataStatic.SetAges(responseData.ages);

                //underDevelopmentGO.ConfirmingMemberNumber();

                if (PlayerDataStatic.Member != null)
                {
                    Debug.Log($"Player number : {PlayerDataStatic.Member} data succesfully stored");
                }
            }
        }
    }

    public void CoinValidated(Coin validatedCoin)
    {
        catchedCoin = validatedCoin;
        if (OnPlayerCatchCoin != null)
        {
            OnPlayerCatchCoin();
        }
    }
    void RewardCoinToPlayer()
    {
        // this executed by event from admobs
        if (catchedCoin != null)
        {
            catchedCoin.RewardingCoin();
            catchedCoin = null;
        }
        
    }

    void CancelCoinRewardingProcess()
    {
        if (catchedCoin != null)
        {
            var coinCollider = catchedCoin.GetComponent<Collider>();
            coinCollider.enabled = true;
            catchedCoin = null;
        }
    }

    #region Disabled System
    public void CatchButtonOnClicked()
    {
        // Collecting Coin System
        // Casting a ray to detect 3D Object on normalRaycastDistance range
        Ray rayOrigin = arCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        RaycastHit hittarget;
        // if its hit something
        if (Physics.Raycast(rayOrigin, out hittarget, raycastDistance))
        {
            Coin coin = hittarget.collider.gameObject.GetComponent<Coin>();
            coin.ValidatingCoin(this);
            if (OnPlayerCatchCoin != null)
            {
                OnPlayerCatchCoin();
            }
            // rest process happen on Coin.cs           
        }
    }
    
    public void ArrowButtonOnClicked()
    {
        //StartCoroutine(ArrowCheatOn());
    }
    public void HammerButtonOnClicked(int amountToSpawn)
    {
        StartCoroutine(HammerCheatOn(amountToSpawn));
    }

    IEnumerator ArrowCheatOn()
    {
        UnityEngine.UI.Image arrowButtonImage = GameObject.FindGameObjectWithTag("Arrow").GetComponent<UnityEngine.UI.Image>();
        Color previousColor = arrowButtonImage.color;
        arrowButtonImage.color = Color.white;
        raycastDistance = arrowRaycastDistance;
        arCamera.farClipPlane = 105f;
        arrowPowerOn = true;
        yield return new WaitForSeconds(20f);
        arrowButtonImage.color = previousColor;
        arCamera.farClipPlane = 20f;
        raycastDistance = normalRaycastDistance;
        arrowPowerOn = false;
    }
    IEnumerator HammerCheatOn(int amountToSpawn)
    {
        for (int i = 0; i < amountToSpawn; i++)
        {
            Transform coinSelected = coinsParentTransform.GetChild(i).GetComponent<Transform>();
            PlaceAtLocation grabbedCoins = coinSelected.GetComponent<PlaceAtLocation>();
            Location spawnLocation = ARLocationManager.Instance.GetLocationForWorldPosition(spawnerLocations[i].position);
            grabbedCoins.Location = spawnLocation;
            coinSelected.SetAsLastSibling();   
        }
        yield return null;
    }
    
    void UpdatingCrosshair(Color crosshairColor, bool setBoolValue)
    {
        crossHair.color = crosshairColor;
        uiAnimator.SetBool("isDetecting", setBoolValue);
    }
    #endregion

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Debug.DrawLine(originPosition, originPosition + direction * raycastDistance);
        Gizmos.DrawWireSphere(originPosition + direction * raycastDistance, radius);

    }

}
