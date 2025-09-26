using System.Collections.Generic;
using UnityEngine;

public class PlayerDeckManager : MonoBehaviour
{
    [SerializeField]
    private List<Card> deck = new List<Card>();
    public List<Card> GetDeck() { return deck; }
    public void SetDeck(List<Card> deck) { this.deck = deck; }
    public void AddCardToDeck(Card card) { deck.Add(card); }
    public void RemoveCardFromDeck(Card card)
    {
        if (card == selectedCard) { SetSelectedCard(null); }

        deck.Remove(card);
    }

    [SerializeField]
    private Card selectedCard;
    public Card GetSelectedCard() { return selectedCard; }
    public void SetSelectedCard(Card selectedCard) { this.selectedCard = selectedCard; }
}