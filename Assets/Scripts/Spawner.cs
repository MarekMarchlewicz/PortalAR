using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class Spawner : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private GameObject prefab;
    
    public void OnPointerDown(PointerEventData eventData)
    {
        Instantiate(prefab).GetComponent<DragBehaviour>().StartDragging(eventData.pointerId, Camera.main);        
    }
}
