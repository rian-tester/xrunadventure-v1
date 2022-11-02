using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using UnityEngine;
using UnityEngine.Networking;

public class MissonDataManager : MonoBehaviour
{
    public class AllMissionData
    {
        public List <MissionData> data;
    }
    [SerializeField] Transform missionParentTransform;
    [SerializeField] GameObject missionItemPrefab;
    AllMissionData allMissionData;
    void Start()
    {
        StartCoroutine(RequestMissionData());
    }

    IEnumerator RequestMissionData()
    {

        // building query
        string endpoint = ServerDataStatic.GetGateway();
        var uriBuilder = new UriBuilder(endpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["act"] = "app5010-02";
        query["member"] = PlayerDataStatic.Member;
        query["os"] = "3114";
        uriBuilder.Query = query.ToString();
        endpoint = uriBuilder.ToString();

        // requesting mission data
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
                AllMissionData responseData = new AllMissionData();
                responseData = JsonConvert.DeserializeObject<AllMissionData>(rawData);
                foreach (Transform transaction in missionParentTransform)
                {
                    Destroy(transaction.gameObject);
                }
                allMissionData = responseData;
                PopulateMission();

            }
        }
    }

    private void PopulateMission()
    {

        for (int i = 0; i < allMissionData.data.Count; i++)
        {
            GameObject mission = Instantiate(missionItemPrefab, missionParentTransform);
            mission.name = $"Mission completed on : {allMissionData.data[i].datebegin}";
            MissionItemGenerator instanceTransactionGenerator = mission.GetComponent<MissionItemGenerator>();
            instanceTransactionGenerator.Setup(allMissionData.data[i]);
        }

    }
}
