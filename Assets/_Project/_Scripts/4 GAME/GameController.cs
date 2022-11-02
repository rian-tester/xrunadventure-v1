using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameController : MonoBehaviour,IPointerDownHandler, IPointerUpHandler
{
    // Controlling
    Animation anim;
    [SerializeField] Image buttonImage;
    [SerializeField] RawImage mapImage;
    [SerializeField] GameObject arSession;
    [SerializeField] GameObject arSessionOrigin;
    //[SerializeField] AbstractMap Map;



    bool gameIsOn;
    [SerializeField] GameMode gameMode = GameMode.Map;

    public event Action OnModeChanged;
    private void Awake()
    {
        anim = GetComponent<Animation>();
        gameMode = GameMode.Map;
        MapActivated(gameMode);

    }
    public void OnPointerDown(PointerEventData eventData)
    {
        // Empty on purpose
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        gameIsOn = !gameIsOn;
        if (gameIsOn)
        {
            gameMode = GameMode.Catch;
            Animate();
            ChangeButtonSprite();
            MapActivated(gameMode);


            OnModeChanged();
        }
        else
        {
            gameMode = GameMode.Map;
            Animate();
            ChangeButtonSprite();
            MapActivated(gameMode);

            OnModeChanged();
        }
    }
    public GameMode GetGameMode()
    {
        return gameMode;
    }
    private void ChangeButtonSprite()
    {
        if (gameIsOn)
        {
            buttonImage.sprite = Resources.Load<Sprite>("GameMode/Cam");
        }
        else
        {
            
            buttonImage.sprite = Resources.Load<Sprite>("GameMode/Map");
        }
    }

    private void Animate()
    {
        if (gameIsOn)
        {
            
            anim.Play("ToCatchMode");
        }
        else
        {
            anim.Play("ToMapMode");
        }
    }
    private void MapActivated(GameMode gameMode)
    {
        switch (gameMode)
        {
            case GameMode.Map:
                if (!mapImage.isActiveAndEnabled) mapImage.gameObject.SetActive(true);
                //if (!Map.isActiveAndEnabled) Map.gameObject.SetActive(true);


                    break;
                case GameMode.Catch:
                mapImage.gameObject.SetActive(false);
                //Map.gameObject.SetActive(false);


                break;

        }
    }

}
