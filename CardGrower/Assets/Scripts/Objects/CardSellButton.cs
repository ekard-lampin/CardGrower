using UnityEngine;
using UnityEngine.UI;

public class CardSellButton : MonoBehaviour
{
    private Card card;
    public void SetCard(Card card)
    {
        this.card = card;
        transform.Find("Button").GetComponent<Button>().onClick.AddListener(SellCard);
    }

    private void SellCard()
    {
        GameManager.instance.SellCard(card);
    }
}