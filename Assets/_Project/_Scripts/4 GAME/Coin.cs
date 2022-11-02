using Newtonsoft.Json;
using System;
using System.Collections;
using System.Web;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// this Class placed in every coin instance
// Instantiate runtime by CoinManager

// this class handle : 
// coin spin
// coin blinking called by Player/Raycast
// coin taken called by Player/Raycast
// Destroying self
// update server if taken
public class Coin : MonoBehaviour
{
    public class CoinCheckResponseData
    {
        public class CoinCheck
        {
            public string Count { get; set; }
        }

        public List<CoinCheck> data;
    }

    Player playerRef;
    CoinData thisCoinData;

    [SerializeField] float yRotationSpeed;
    [SerializeField] float delayTime;

    public event Action OnCoinHit;
    public event Action OnCoinRewareded;

    void OnEnable()
    {
        // caching
        thisCoinData = GetComponent<CoinData>();

        // define y axis rotation
        int randomSwitch = UnityEngine.Random.Range(1, 3);
        if (randomSwitch == 1)
        {
            yRotationSpeed = 1f;
        }
        else
        {
            yRotationSpeed = -1f;
        }  

    }

    void Update()
    {
        // rotating
        transform.Rotate(0, yRotationSpeed, 0, Space.World);

    }
    public Sprite GetCoinAdThumbnail()
    {
        string str = thisCoinData.AdThumbnail2;
        byte[] imageBytes = Convert.FromBase64String(str);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(imageBytes);
        Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        return sprite;
    }
    public Sprite GetCoinBrandLogo()
    {
        string str = thisCoinData.BrandLogo;
        byte[] imageBytes = Convert.FromBase64String(str);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(imageBytes);
        Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        return sprite;
    }
    public Sprite GetCoinSymbolLogo()
    {
        string str = thisCoinData.Symbolimg;
        byte[] imageBytes = Convert.FromBase64String(str);
        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(imageBytes);
        Sprite sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        return sprite;
    }
    public void ValidatingCoin(Player player)
    {
        playerRef = player; 
        StartCoroutine(ValidatingCoinProcess());
    }
    IEnumerator ValidatingCoinProcess()
    {
        // building query
        string endpoint = ServerDataStatic.GetGateway();
        var uriBuilder = new UriBuilder(endpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["act"] = "app3000-02";
        query["member"] = PlayerDataStatic.Member;
        query["coin"] = thisCoinData.Coin;
        query["advertisement"] = thisCoinData.Advertisement;
        query["os"] = "3114";
        uriBuilder.Query = query.ToString();
        endpoint = uriBuilder.ToString();
        Debug.Log("Final Uri : \n" + endpoint);


        // web request to server
        using (UnityWebRequest www = UnityWebRequest.Get(endpoint))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                // cahching request response
                var rawData = www.downloadHandler.text;
                Debug.Log(rawData);

                CoinCheckResponseData responseData = new CoinCheckResponseData();
                responseData = JsonConvert.DeserializeObject<CoinCheckResponseData>(rawData);
                Debug.Log(responseData.data[0].Count);
                if (responseData.data[0].Count == "-1")
                {
                    Debug.Log("Coin not exist anymore");
                    // TO DO 
                    // Show prompt : this coin data is not updated we will restart fetching data
                }
                else if (responseData.data[0].Count == "1")
                {
                    Debug.Log("Coin exist");
                    // play sfx hit
                    if (OnCoinHit != null)
                    {
                        OnCoinHit();
                    }
                    gameObject.GetComponent<Collider>().enabled = false;
                    // inform player cs that coin is exist
                    playerRef.CoinValidated(this);

                    
                }
                else if (responseData == null)
                {
                    RemoveCoinFromWorld();
                    // TO DO
                    // Show prompt : this coin not exist we will restart fetching data
                }

            }
        }

    }

    public void RewardingCoin()
    {
        StartCoroutine(RewardingCoinToPlayerProcess());
        print("Run method");
    }

    IEnumerator RewardingCoinToPlayerProcess()
    {
        // building query
        string endpoint = ServerDataStatic.GetGateway();
        var uriBuilder = new UriBuilder(endpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["act"] = "nd-1001";
        query["member"] = PlayerDataStatic.Member;
        query["coin"] = thisCoinData.Coin;
        query["advertisement"] = thisCoinData.Advertisement;
        query["os"] = "3114";
        uriBuilder.Query = query.ToString();
        endpoint = uriBuilder.ToString();
        Debug.Log("Run web request to give coin");


        // web request to server
        using (UnityWebRequest www = UnityWebRequest.Get(endpoint))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Coin Rewarded");
                if (OnCoinRewareded != null)
                {
                    OnCoinRewareded();
                }

                
            }
        }
    }
    public void RemoveCoinFromWorld()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
        Destroy(gameObject);
        
        
    }
}
