using AppsFlyerSDK;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameController : MonoBehaviour,IPointerDownHandler, IPointerUpHandler
{

    // Controlling
    Animation anim;
    [SerializeField] Image buttonImage;
    [SerializeField] RawImage mapImage;
    [SerializeField] GameObject MapItems;
    [SerializeField] GameObject mapLighting;

    [SerializeField] GameObject arSession;
    [SerializeField] GameObject arSessionOrigin;
    [SerializeField] MultipleCoinPlacement multipleCoinPlacement;
    [SerializeField] MapController mapController;
    [SerializeField] GameObject environmentLighting;
    
    [SerializeField] TMP_Text loadingText;
    [SerializeField] GameObject loadingPanel;

    [SerializeField] TMP_Text debugText;
    [SerializeField] GameMode gameMode;

    public bool isTheGameStart;

    private void Awake()
    {
        PlayerDataStatic.SpawnAmount = PlayerDataStatic.GetRandomNumber();
    }
    private void Start()
    {
        anim = GetComponent<Animation>();
        loadingPanel.SetActive(true);
        loadingText.text = "Please wait while we're communicating with server";
        isTheGameStart = false;
        gameMode = GameMode.None;

    }

    private void Update()
    {
        // this line of code executed when MapController.cs and MultipleCoinPlacement.cs finished with poppulating coin data
        if (isTheGameStart == false)
        {
            if (mapController.isCoinPopulated == true && multipleCoinPlacement.isCoinPopulated == true)
            {
                loadingPanel.SetActive(false);
                gameMode = GameMode.Map;
                ChangeGameMode(gameMode);
                isTheGameStart = true;
            }
        }
    }

    private void ChangeButtonSprite()
    {
        if (gameMode == GameMode.Catch)
        {
            buttonImage.sprite = Resources.Load<Sprite>("GameMode/Cam");
        }
        else if (gameMode == GameMode.Map)
        {
            
            buttonImage.sprite = Resources.Load<Sprite>("GameMode/Map");
        }
    }

    private void Animate()
    {
        if (gameMode == GameMode.None)
        {
            return;
        }
        else if (gameMode == GameMode.Catch)
        {
            
            anim.Play("ToCatchMode");
        }
        else if(gameMode == GameMode.Map)
        {
            anim.Play("ToMapMode");
        }
    }
    private void ChangeGameMode(GameMode gameMode)
    {
        switch (gameMode)
        {
            case GameMode.Map:
                // Map
                if (!mapImage.isActiveAndEnabled) mapImage.gameObject.SetActive(true);
                if (!MapItems.activeSelf) MapItems.gameObject.SetActive(true);
                OnlineMaps.instance.Redraw();
                mapLighting.gameObject.SetActive(true);

                if (debugText.gameObject.activeSelf)
                {
                    if (mapController.thisAllCoinData != null)
                    {
                        debugText.text = $"Amount of mapController Server Data : {mapController.thisAllCoinData.data.Count}";
                    }
                    
                    debugText.text = debugText.text + $"\n Amount om map marker : {OnlineMapsMarkerManager.instance.Count}";
                }
                

                //Catch
                arSession.SetActive(false);
                arSessionOrigin.SetActive(false);
                environmentLighting.gameObject.SetActive(false);
                break;


                case GameMode.Catch:
                // Catch
                arSession.SetActive(true);
                arSessionOrigin.SetActive(true);
                environmentLighting.gameObject.SetActive(true);

                if (debugText.gameObject.activeSelf)
                {
                    debugText.text = $" Amount of multipleCoinPlacement Server data : {multipleCoinPlacement.serverRawData.data.Count}";
                }
                
                // Map
                mapImage.gameObject.SetActive(false);
                MapItems.gameObject.SetActive(false);
                mapLighting.gameObject.SetActive(false);
                break;

            case GameMode.None:
                //
                break;

        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        if (gameMode == GameMode.Map || gameMode == GameMode.None)
        {
            gameMode = GameMode.Catch;
            Animate();
            ChangeButtonSprite();
            ChangeGameMode(gameMode);

        }
        else if (gameMode == GameMode.Catch || gameMode == GameMode.None)
        {
            gameMode = GameMode.Map;
            Animate();
            ChangeButtonSprite();
            ChangeGameMode(gameMode);
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        // Empty on purpose
    }
    public GameMode GetGameMode()
    {
        return gameMode;
    }
}
