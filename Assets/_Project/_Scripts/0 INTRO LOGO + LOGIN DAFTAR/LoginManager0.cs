using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class LoginManager0 : MonoBehaviour
{
    [SerializeField]
    float fillingSpeed;

    [SerializeField]
    Canvas mainScreen;
    [SerializeField]
    Image mainLogo;
    [SerializeField]
    Canvas loginCreateMenu;
    [SerializeField]
    AudioManager audioManager;
    [SerializeField]
    Button loginButton;
    [SerializeField]
    Button registerButton;

    private void Awake()
    {
        mainScreen.gameObject.SetActive(true);

        if (audioManager == null)
        {
            audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        }
    }

    private void Start()
    {
        if (audioManager != null)
        {
            AudioSource backsoundPlaying = GameObject.FindGameObjectWithTag("Backsound").GetComponent<AudioSource>();
            if (backsoundPlaying.isPlaying)
            {
                backsoundPlaying.Stop();
            }
        }
        // start sequence
        StartCoroutine(FirstSequence());
    }
    /// <summary>
    /// Will start button sequence depending on the buttoncode
    /// </summary>
    /// <param name="buttonCode"></param>    
    public void OnButtonsClicked(int buttonCode)
    {
        StartCoroutine(ButtonSequence(buttonCode));
    }
    IEnumerator FirstSequence()
    {
        yield return new WaitForSeconds(0.5f);
        mainScreen.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        loginCreateMenu.gameObject.SetActive(true);
        loginButton.interactable = true;
        registerButton.interactable = true;
    }
    IEnumerator ButtonSequence(int buttonCode)
    {
        switch (buttonCode)
        {
            case 0:
                yield return new WaitForSeconds(0.5f);
                StartCoroutine(SceneLoader.LoadScene(2,0.5f));
                break;
                case 1:
                yield return new WaitForSeconds(0.5f);
                StartCoroutine(SceneLoader.LoadScene(1,0.5f));
                break;
        }
    }

}
