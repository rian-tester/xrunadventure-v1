using ARFoundationRemote;
using ARLocation;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Pinch : MonoBehaviour
{
    [SerializeField] MapController mapController;
    [SerializeField] float zoomSpeed;
    [SerializeField] float maxZoom;
    [SerializeField] float minZoom;
    [SerializeField] Camera mapCamera;
    [SerializeField] RawImage mapRawImage;
    private MapControl mapControl;
    private OnlineMaps map;
    private Coroutine zoomCoroutine;
    public event Action OnMapZoomChanged;

    [SerializeField] TMP_Text debugText;

    private void Awake()
    {
        mapControl = new MapControl();
    }
    private void OnEnable()
    {
        mapControl.Enable();
    }
    private void OnDisable()
    {
        mapControl.Disable();  
    }
    private void Start()
    {
        map = OnlineMaps.instance;
        mapController.OnMapSetup += SetupControl;
    }

    void SetupControl()
    {
        mapControl.Touch.SecondaryTouchContact.started += _ => ZoomStart();
        mapControl.Touch.SecondaryTouchContact.canceled += _ => ZoomEnd();      
    }
    void ZoomStart()
    {
        zoomCoroutine = StartCoroutine(ZoomDetection());
    }
    void ZoomEnd()
    {
        StopCoroutine(zoomCoroutine);
    }

    IEnumerator ZoomDetection()
    {
        float previousDistance = 0f, distance = 0f;
        
        while (true)
        {
            distance = Vector2.Distance(mapControl.Touch.PrimaryFingerPosition.ReadValue<Vector2>(),
                mapControl.Touch.SecondaryFingerPosition.ReadValue<Vector2>());

            if (distance > previousDistance)
            {
                if (OnlineMaps.instance.floatZoom < maxZoom)
                {
                    // zoom in - finger drag out
                    map.floatZoom += Time.deltaTime * zoomSpeed;

                    if (OnMapZoomChanged != null)
                    {
                        OnMapZoomChanged();
                    }
                }
            }
            
            if (distance < previousDistance)
            {
                if (OnlineMaps.instance.floatZoom > minZoom)
                {
                    map.floatZoom -= Time.deltaTime * zoomSpeed;

                    if (OnMapZoomChanged != null)
                    {
                        OnMapZoomChanged();
                    }
                }
            }
            previousDistance = distance;
            yield return null;
        }           
    }
}
