using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class NotifikasiDataManager : MonoBehaviour
{
    public class ChatItemArray
    {
        public List<ChatItem> data = new List<ChatItem>();
    }

    [SerializeField] ChatItem prefab;
    [SerializeField] Transform parent;
    [SerializeField] TMP_InputField question;

    string endpoint = "https://app.xrun.run/gateway.php?";

    private void Start()
    {
        StartCoroutine(FetchExistingChat());
    }
    IEnumerator FetchExistingChat()
    {
        
        
        var uriBuilder = new UriBuilder(endpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["act"] = "ap6000-01";
        query["member"] = PlayerDataStatic.Member.ToString();
        query["os"] = "3114";
        uriBuilder.Query = query.ToString();
        endpoint = uriBuilder.ToString();
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
               
                var rawData = www.downloadHandler.text;
                // store into costum class
                ChatItemArray serverRawData;

                yield return serverRawData = JsonConvert.DeserializeObject<ChatItemArray>(rawData);

                foreach (Transform child in parent)
                {
                    Destroy(child.gameObject);
                }


                for (int i = 0; i < serverRawData.data.Count; i++)
                {

                    ChatItem chatItem = Instantiate(prefab, parent);
                    
                    chatItem.title = serverRawData.data[i].title;
                    chatItem.contents = serverRawData.data[i].contents;
                    chatItem.datetime = serverRawData.data[i].datetime;
                    chatItem.time = serverRawData.data[i].time; 
                    chatItem.Setup();
                    Debug.Log(serverRawData.data.Count);
                }                
            }
        }
    }

    public void SendQuestion()
    {
        StartCoroutine(SubmitAndFetch());
    }

    IEnumerator SubmitAndFetch()
    {
        var uriBuilder = new UriBuilder(endpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["act"] = "ap6000-02";
        query["member"] = PlayerDataStatic.Member.ToString();
        query["title"] = question.text.ToString();
        query["os"] = "3114";
        uriBuilder.Query = query.ToString();
        endpoint = uriBuilder.ToString();
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
                StartCoroutine(FetchExistingChat());
            }
        }

    }
}
