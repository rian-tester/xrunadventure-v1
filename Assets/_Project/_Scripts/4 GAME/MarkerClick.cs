using UnityEngine;
using UnityEngine.EventSystems;

public class MarkerClick : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        var clickPoint = eventData.position;
        var mapCamera = GameObject.FindGameObjectWithTag("MapCamera").GetComponent<Camera>();
        Vector3 hitPointInworld = transform.position;
        Ray ray = mapCamera.ScreenPointToRay(clickPoint);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            hitPointInworld = hit.point;
            Debug.DrawLine(mapCamera.transform.position, hit.point, Color.green, 1f);
        }
    }
}
