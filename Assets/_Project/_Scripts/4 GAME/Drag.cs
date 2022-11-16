using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, IPointerDownHandler, IPointerUpHandler , IDragHandler
{
    public class DragLocation
    {
        public double Lng;
        public double Lat;

        public DragLocation(double lng, double lat)
        {
            Lng = lng;
            Lat = lat;
        }   
    }

    [SerializeField] Camera mapCamera;
    OnlineMaps map;
    DragLocation startDraglocation;
    DragLocation lastDraglocation;
    DragLocation originLocation;
    DragLocation targetLocation;

    [SerializeField] float draggingModifier;
    private void Start()
    {
        map = OnlineMaps.instance;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        DebugLine(eventData, Color.blue);

        // coordinates when start dragging
        startDraglocation = GetTouchPointInCoordinates(eventData);
        Debug.Log($"Start drag location {startDraglocation.Lat}, {startDraglocation.Lng}");
    }
    public void OnDrag(PointerEventData eventData)
    {
        DebugLine(eventData, Color.white);

        // coordinates last position when dragging
        lastDraglocation = GetTouchPointInCoordinates(eventData);
        Debug.Log($"Last drag location {lastDraglocation.Lat}, {lastDraglocation.Lng}");

        // calculate offset between start drag and last drag location
        double offsetLng = (lastDraglocation.Lng  - startDraglocation.Lng) * draggingModifier ;
        double offsetLat = (lastDraglocation.Lat - startDraglocation.Lat) * draggingModifier;
        Debug.Log($"offset Lng:{offsetLng}, Lat:{offsetLat}");
        
        // get orgin of the drag location
        originLocation = GetCenterCameraCoordinates();
        
        // add the offset into origin of the drag location
        DragLocation toGoLocation = new DragLocation(originLocation.Lng + offsetLng, originLocation.Lat + offsetLat);

        //set map position into that new location 
        targetLocation = toGoLocation;
        map.SetPositionAndZoom(targetLocation.Lng, targetLocation.Lat, map.floatZoom);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        DebugLine(eventData, Color.yellow);
              
        if (targetLocation != null)
        {
            map.SetPositionAndZoom(targetLocation.Lng, targetLocation.Lat, map.floatZoom);
        }

        targetLocation = null;
    }

    DragLocation GetTouchPointInCoordinates(PointerEventData eventData)
    {
        var clickPoint = eventData.position;

        Vector3 hitPointInworld = new Vector3(0, 0, 0);
        Ray ray = mapCamera.ScreenPointToRay(clickPoint);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            hitPointInworld = hit.point;
        }

        double lng = 0;
        double lat = 0;
        OnlineMapsControlBase.instance.GetCoords(new Vector2(hitPointInworld.x, hitPointInworld.y), out lng, out lat);
        DragLocation result = new DragLocation(lng, lat);
        return result;
    }
    DragLocation GetCenterCameraCoordinates()
    {
        Vector3 hitPointInworld = new Vector3(0, 0, 0);
        Ray ray = mapCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            hitPointInworld = hit.point;
        }

        double lng = 0;
        double lat = 0;
        OnlineMapsControlBase.instance.GetCoords(new Vector2(hitPointInworld.x, hitPointInworld.y), out lng, out lat);
        DragLocation result = new DragLocation(lng, lat);
        return result;
    }
    void DebugLine(PointerEventData eventData, Color lineColor)
    {
        Ray ray = mapCamera.ScreenPointToRay(eventData.position);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.DrawLine(mapCamera.transform.position, hit.point, lineColor, 1f);
        }
    }


}
