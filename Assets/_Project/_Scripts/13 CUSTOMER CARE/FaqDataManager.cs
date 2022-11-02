using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Web;
using UnityEngine;
using UnityEngine.Networking;

public class FaqDataManager : MonoBehaviour
{
    public class FaqItemArray
    {
        public List<FaqItemData> data = new List<FaqItemData>();
    }

    [SerializeField] GameObject prefab;
    [SerializeField] Transform parent;


    private void Start()
    {
        StartCoroutine(FetchAllFaq());
    }
    IEnumerator FetchAllFaq()
    {
        yield return null;
        var endpoint = ServerDataStatic.GetGateway();
        var uriBuilder = new UriBuilder(endpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["act"] = "app7310-01";
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
                FaqItemArray serverRawData;

                yield return serverRawData = JsonConvert.DeserializeObject<FaqItemArray>(rawData);
                //Debug.Log($"Result of server call for FAQ : {serverRawData.data.Count}");
                foreach (Transform child in parent)
                {
                    Destroy(child.gameObject);
                }


                for (int i = 0; i < serverRawData.data.Count; i++)
                {

                    var faqItem = Instantiate(prefab, parent);
                    faqItem.name = $"Faq no.{i + 1}";
                    var faqItemDataInstance = faqItem.GetComponent<FaqItem>();
                    faqItemDataInstance.Question = serverRawData.data[i].title;
                    faqItemDataInstance.Answer = serverRawData.data[i].contents;
                    //Debug.Log($"Checking question no {i + 1} : question is {serverRawData.data[i].title}");
                    //Debug.Log($"Checking answer no {i + 1} : answer is {serverRawData.data[i].contents}");
                    faqItemDataInstance.Setup();              
                }
                Debug.Log(serverRawData.data.Count);
            }
        }
    }
}
