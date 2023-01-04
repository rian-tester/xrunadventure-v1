using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using System.Web;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class HomeManager : MonoBehaviour
{
    [SerializeField] UnderDevelopment underDevelopmentGO;

    //string serverEndpoint = "https://app.xrun.run/gateway.php?";
    private void Start()
    {
        string playerEmail = PlayerPrefs.GetString("email");
        StartCoroutine(StorePlayerData(playerEmail));
    }
    public void HomeButtonsClicked(int buttonCode)
    {
        StartCoroutine(HomeButtonsClickedSequence(buttonCode));
    }

    IEnumerator HomeButtonsClickedSequence(int buttoncode)
    {
        switch (buttoncode)
        {
            case 4: // Play button
                yield return new WaitForSeconds(0.5f);
                SceneManager.LoadScene(4);
                break;
                case 5: // My Xrun Button
                yield return new WaitForSeconds(0.5f);
                SceneManager.LoadScene(5);
                break;
            case 6: // My Ads Button
                yield return new WaitForSeconds(0.5f);
                SceneManager.LoadScene(6);
                break;
            case 7: // Shop Button
                yield return new WaitForSeconds(0.5f);
                SceneManager.LoadScene(7);
                break;
            case 8: // Setting Button
                yield return new WaitForSeconds(0.5f);
                SceneManager.LoadScene(8);
                break;
            case 9: // notification button
                SceneManager.LoadScene(9);
                break;
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
            // sending serverData request
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                // cahching request responseTwo
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
                Debug.Log($"Player number {PlayerDataStatic.Member} Succesfully stored!");
                if (PlayerDataStatic.Member != null)
                {
                    Debug.Log($"Player number : {PlayerDataStatic.Member} serverData succesfully stored");
                }
            }
        }
    }
}
