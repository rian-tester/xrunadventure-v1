using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class GameManager : Panel
{
    [SerializeField] UserLocation userLocation;
    
    [Header("Controlling")]
    [SerializeField] AudioManager audioManager;
    [SerializeField] Image audioButton;

    [Header("Updating")]
    [SerializeField] TMP_Text debugText; 

    private void Awake()
    {
        if (audioManager == null)
        {
            audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        }
        desiredAlpha = 0;
        currentAlpha = 0;
        promptPanel.gameObject.SetActive(false);

    }

    private void OnEnable()
    {
        userLocation.OnUserLocationNotAvailable += ActionUserLocationNotAvailable;
    }

    

    private void OnDisable()
    {
        userLocation.OnUserLocationNotAvailable -= ActionUserLocationNotAvailable;
    }

    private void Update()
    {
        // Call this on Update
        promptPanel.alpha = currentAlpha;
        currentAlpha = Mathf.MoveTowards(currentAlpha, desiredAlpha, fadeTime * Time.deltaTime);
    }
    public void BacksoundToggle()
    {
        if (!audioManager.backsound.isPlaying)
        {
            audioManager.backsound.Play();
            audioButton.color = new Color(audioButton.color.r, audioButton.color.g, audioButton.color.b, 1f);

        }
        else
        {
            audioManager.backsound.Stop();
            audioButton.color = new Color(audioButton.color.r, audioButton.color.g, audioButton.color.b, 0.25f);
            
        }       
    }

    public void GoToButtonPressed()
    {
        SceneManager.LoadScene(5);
    }

    public void ReloadScene()
    {
        debugText.text = "Reloading Scene";
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene);
    }
    public void ARTrackingLost()
    {
        debugText.text ="AR DragLocation Manager : tracking is Lost!";
    }
    public void ARTrackingRestored()
    {
        debugText.text = "AR DragLocation Manager : tracking is Restored!";
    }
    public void ARLocationEnabled()
    {
        debugText.text = "AR DragLocation Provider : DragLocation is Enabled!";
    }

    void ActionUserLocationNotAvailable()
    {
        ShowPromptTextPanelForSeconds(userLocationNotAvailable, 1.5f);
    }
}
