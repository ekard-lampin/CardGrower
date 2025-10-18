using System;

[Serializable]
public class SaveCardObject
{
    public CardId cardId;
    public CardId GetCardId() { return cardId; }
    public void SetCardId(CardId cardId) { this.cardId = cardId; }

    public CardType cardType;
    public CardType GetCardType() { return cardType; }
    public void SetCardType(CardType cardType) { this.cardType = cardType; }

    public string cardTexture;
    public string GetCardTexture() { return cardTexture; }
    public void SetCardTexture(string cardTexture) { this.cardTexture = cardTexture; }

    public string cardDescription;
    public string GetCardDescription() { return cardDescription; }
    public void SetCardDescription(string cardDescription) { this.cardDescription = cardDescription; }

    public Rarity cardRarity;
    public Rarity GetCardRarity() { return cardRarity; }
    public void SetCardRarity(Rarity cardRarity) { this.cardRarity = cardRarity; }

    public Quality cardQuality;
    public Quality GetCardQuality() { return cardQuality; }
    public void SetCardQuality(Quality cardQuality) { this.cardQuality = cardQuality; }

    public int cardValue;
    public int GetCardValue() { return cardValue; }
    public void SetCardValue(int cardValue) { this.cardValue = cardValue; }
}