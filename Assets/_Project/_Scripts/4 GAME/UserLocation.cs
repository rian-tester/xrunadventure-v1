using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARLocation;
using TMPro;
using System;
using UnityEngine.SceneManagement;

// This class placed in independent Object called UserLocation
// Exist in hierarchy

// This class handle :
// Updating user latitude longitude
// Sending the data into Canvas0/StatsGroup/UserLocation UI

public class UserLocation : MonoBehaviour
{
    [SerializeField] Camera arCamera;
    //[SerializeField] TMP_Text userLocationTextLatitude, userLocationTextLongitude;

    public event Action OnUserLocationNotAvailable;

    private void Awake()
    {
        ActionUserLocationNotEnabled();
    }

    private void Update() //updating runtime
    {      
        //if (userLocationTextLatitude != null && userLocationTextLongitude != null)
        //{
        //    Location cameraLocation = ARLocationManager.Instance.GetLocationForWorldPosition(arCamera.transform.position);
        //    userLocationTextLatitude.text = "Lat : " + cameraLocation.Latitude.ToString();
        //    userLocationTextLongitude.text = "Lng : " + cameraLocation.Longitude.ToString();
        //} 
    }
    void ActionUserLocationNotEnabled()
    {
        StartCoroutine(MakesureUserHasLocation());
    }
    IEnumerator MakesureUserHasLocation()
    {
        yield return new WaitForSeconds(20f);

        Location cameraLocation = ARLocationManager.Instance.GetLocationForWorldPosition(arCamera.transform.position);
        string Lat = cameraLocation.Latitude.ToString();
        string Lon = cameraLocation.Longitude.ToString();

        if (Lat != "0" && Lon != "0") yield break;

        if (OnUserLocationNotAvailable != null)
        {
            OnUserLocationNotAvailable();
        }
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(15);
    }

}
