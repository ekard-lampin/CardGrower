using System;
using UnityEngine;

[Serializable]
public class Card
{
    [SerializeField]
    private CardId cardId;
    public CardId GetCardId() { return cardId; }
    public void SetCardId(CardId cardId) { this.cardId = cardId; }

    [SerializeField]
    private CardType cardType;
    public CardType GetCardType() { return cardType; }
    public void SetCardType(CardType cardType) { this.cardType = cardType; }

    [SerializeField]
    private Texture cardTexture;
    public Texture GetCardTexture() { return cardTexture; }
    public void SetCardTexture(Texture cardSprite) { this.cardTexture = cardSprite; }

    [SerializeField]
    private string cardDescription;
    public string GetCardDescription() { return cardDescription; }
    public void SetCardDescription(string cardDescription) { this.cardDescription = cardDescription; }

    [SerializeField]
    private Rarity cardRarity;
    public Rarity GetCardRarity() { return cardRarity; }
    public void SetCardRarity(Rarity cardRarity) { this.cardRarity = cardRarity; }

    [SerializeField]
    private Quality cardQuality;
    public Quality GetCardQuality() { return cardQuality; }
    public void SetCardQuality(Quality cardQuality) { this.cardQuality = cardQuality; }

    public Card CopyCard()
    {
        Card newCard = new Card();

        newCard.SetCardId(cardId);
        newCard.SetCardType(cardType);
        newCard.SetCardTexture(cardTexture);
        newCard.SetCardDescription(cardDescription);
        newCard.SetCardRarity(cardRarity);
        newCard.SetCardQuality(cardQuality);

        return newCard;
    }
}