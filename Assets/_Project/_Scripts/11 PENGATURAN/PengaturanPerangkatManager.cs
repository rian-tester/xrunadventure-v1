using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class PengaturanPerangkatManager : Panel
{
    [SerializeField] UltimateUISwitch swithGps;
    [SerializeField] UltimateUISwitch swithKamera;
    [SerializeField] UltimateUISwitch swithPush;

    private void Awake()
    {
        currentAlpha = 0;
        desiredAlpha = 0;
        promptPanel.gameObject.SetActive(false);

    }
    private void Start()
    {
        ShowPromptTextPanel(startMessage);
        CheckPermission();
    }
    private void Update()
    {
        promptPanel.alpha = currentAlpha;
        currentAlpha = Mathf.MoveTowards(currentAlpha, desiredAlpha, fadeTime * Time.deltaTime);
    }
    public void GoBackToPengaturan()
    {
        StartCoroutine(SceneLoader.LoadScene(11, 0.25f));
        //SceneManager.LoadScene(11);
    }
    public void GpsSwitch(bool value)
    {
        
        bool currentValue = swithGps.Value;


        if (currentValue == true)
        {
            var callbacks = new PermissionCallbacks();
            callbacks.PermissionDenied += PermissionCallbacks_PermissionDenied;
            callbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;
            callbacks.PermissionDeniedAndDontAskAgain += PermissionCallbacks_PermissionDeniedDontAskAgain;
            Permission.RequestUserPermission(Permission.FineLocation, callbacks);
        }
        else
        {
            CheckPermission();
        }
    }
    public void KameraSwitch(bool value)
    {
        bool currentValue = swithKamera.Value;
        if (currentValue == true)
        {
            var callbacks = new PermissionCallbacks();
            callbacks.PermissionDenied += PermissionCallbacks_PermissionDenied;
            callbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;
            callbacks.PermissionDeniedAndDontAskAgain += PermissionCallbacks_PermissionDeniedDontAskAgain;
            Permission.RequestUserPermission(Permission.Camera, callbacks);
        }
        else
        {
            CheckPermission();
        }
    }
    public void PushSwitch(bool value)
    {
       // TO DO
    }

    public void HidePanel()
    {
        HidePromptTextPanel();
    }

    void CheckPermission()
    {
        if (Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            swithGps.Value = true;
        }
        else
        {
            swithGps.Value = false;
        }
        if (Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            swithKamera.Value = true;
        }
        else
        {
            swithKamera.Value= false;   
        }
    }
    void PermissionCallbacks_PermissionGranted(string permissionName)
    {
        string permissionAndResult = permissionName + "_TRUE";
        AdjustSwith(permissionAndResult);
    }

    void PermissionCallbacks_PermissionDenied(string permissionName)
    {
        string permissionAndResult = permissionName + "_FALSE";
        AdjustSwith(permissionAndResult);
    }

    void PermissionCallbacks_PermissionDeniedDontAskAgain(string permissionName)
    {
        string permissionAndResult = permissionName + "_FALSE";
        AdjustSwith(permissionAndResult);
    }

    void AdjustSwith(string result)
    {

        switch (result)
        {
            case "android.permission.CAMERA_TRUE":
                swithKamera.Value = true;
                break;
            case "android.permission.CAMERA_FALSE":
                swithKamera.Value = false;
                ShowPromptTextPanel(cameraceWarning);
                break;
            case "android.permission.ACCESS_FINE_LOCATION_TRUE":
                swithGps.Value = true;
                break;
            case "android.permission.ACCESS_FINE_LOCATION_FALSE":
                swithGps.Value = false;
                ShowPromptTextPanel(locationServiceWarning);
                break;
        }
    }
}
