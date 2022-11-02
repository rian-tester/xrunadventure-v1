using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class KetentuanDataManager : MonoBehaviour
{
    public class KetentuanItemArray
    {
        public List<KetentuanItem> data = new List<KetentuanItem>();
    }

    [SerializeField] KetentuanItem prefab;
    [SerializeField] Transform parent;

    string endpoint = "https://app.xrun.run/gateway.php?";
    public  void SyaratKetentuanClicked(int indexNumber)
    {
        StartCoroutine(FetchAllData(indexNumber));
    }
    IEnumerator FetchAllData(int indexNumber)
    {
        foreach(Transform child in parent)
        {
            Destroy(child.gameObject);
        }
        var uriBuilder = new UriBuilder(endpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["act"] = "app7010-01-id";
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

                KetentuanItemArray serverRawData;
                yield return serverRawData = JsonConvert.DeserializeObject<KetentuanItemArray>(rawData);

                KetentuanItem chatItem = Instantiate(prefab, parent);
                yield return chatItem.c = serverRawData.data[indexNumber].c;
                chatItem.Setup();
            }
        }
    }
}
