using UnityEngine;
using UnityEngine.EventSystems;

public class HalfBar : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
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

    public void Initialize(Linkage parentLinkage, Joint pivotJoint, Joint oppositeJoint)
    {
        this.parentLinkage = parentLinkage;
        this.pivotJoint = pivotJoint;
        this.oppositeJoint = oppositeJoint;
    }

    public Joint pivotJoint; // The pivot joint
    public Joint oppositeJoint; // The opposite end of the half bar
}
