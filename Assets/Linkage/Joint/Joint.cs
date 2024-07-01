using UnityEngine;
using UnityEngine.EventSystems;

public class Joint : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [HideInInspector]
    public Linkage parentLinkage;

    public void OnBeginDrag(PointerEventData eventData)
    {
        parentLinkage.OnBeginDrag(LinkagePartType.Joint, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        parentLinkage.OnDrag(LinkagePartType.Joint, eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        parentLinkage.OnEndDrag(LinkagePartType.Joint, eventData);
    }
}
