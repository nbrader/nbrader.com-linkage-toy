using UnityEngine;
using UnityEngine.EventSystems;

public class Joint : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [HideInInspector]
    public Linkage parentLinkage;

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentLinkage.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        parentLinkage.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        parentLinkage.OnEndDrag(eventData);
    }
}
