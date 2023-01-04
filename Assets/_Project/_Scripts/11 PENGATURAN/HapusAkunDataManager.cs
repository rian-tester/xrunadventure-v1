using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class HapusAkunDataManager : MonoBehaviour
{
    public class ServerDataStructure
    {
        public class ServerDataItem
        {
            public string count;
        }
        public List<ServerDataItem> data = new List<ServerDataItem>();
    }
        
    [SerializeField] TMP_InputField passwordInputField;

    string reason = "";
    ServerDataStructure serverRawData;
    public void ConfirmToDeleteAccount()
    {
        FillReasonString();
        Debug.Log("All reason : " + reason);
        if (!AllDataReady()) return;
        else
        {
            StartCoroutine(DeleteAccount());
        }
        reason = "";
    }
    void FillReasonString()
    {
        var manager = GetComponent<HapusAkunManager>();
        List<UltimateUICheckbox> checkbox = manager.GetAllCheckbox();
        foreach(UltimateUICheckbox data in checkbox)
        {
            if (data.Value == true)
            {
                var go = data.gameObject;
                reason = reason + go.name + ".";
            }
        }
    }
    bool AllDataReady()
    {
        if (passwordInputField == null)
        {
            return false;
        }
        return true;
    }
    IEnumerator DeleteAccount()
    {
        yield return null;
        // building query
        string endpoint = ServerDataStatic.GetGateway();
        var uriBuilder = new UriBuilder(endpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["act"] = "app8080-01";
        query["member"] = PlayerDataStatic.Member.ToString();
        query["pin"] = passwordInputField.text.ToString();
        query["reason"] = reason;
        query["os"] = "3114";
        uriBuilder.Query = query.ToString();
        endpoint = uriBuilder.ToString();

        // web request to server
        var manager = GetComponent<HapusAkunPrompt>();
        using (UnityWebRequest www = UnityWebRequest.Get(endpoint))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);

                manager.ClosePrompt();
            }
            else
            {
                // cahching request responseTwo
                var rawData = www.downloadHandler.text;
                serverRawData = JsonConvert.DeserializeObject<ServerDataStructure>(rawData);
                if (serverRawData.data[0].count == "-1")
                {
                    Debug.Log($"Delete account failed, responseTwo code : {serverRawData.data[0].count}");
                    manager.ClosePrompt();
                }
                else
                {
                    Debug.Log($"Delete account succeed, responseTwo code : {serverRawData.data[0].count}");
                    manager.ClosePrompt();
                }
            }
        }
    }
}
