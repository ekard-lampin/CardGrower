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
        if (selectedCard.Length > 0 && card == selectedCard[0]) { SetSelectedCard(new Card[0]); }

        deck.Remove(card);
    }

    [SerializeField]
    private Card[] selectedCard = new Card[0];
    public Card[] GetSelectedCard() { return selectedCard; }
    public void SetSelectedCard(Card[] selectedCard) { this.selectedCard = selectedCard; }
}