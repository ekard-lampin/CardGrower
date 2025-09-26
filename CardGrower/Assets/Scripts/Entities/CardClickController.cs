using UnityEngine;
using UnityEngine.EventSystems;

public class CardClickController : MonoBehaviour, IPointerDownHandler
{
    CardComponent cardComponent;
    public CardComponent GetCardComponent() { return cardComponent; }
    public void SetCardComponent(CardComponent cardComponent) { this.cardComponent = cardComponent; }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if (cardComponent == null) { return; }

        cardComponent.Click();
    }
}