using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SlotLever : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private UnityEvent pullLeverEvent;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            pullLeverEvent.Invoke();
    }
}
