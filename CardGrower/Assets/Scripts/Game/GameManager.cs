using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    void Awake() { instance = this; }

    [Header("Card Settings")]
    [SerializeField]
    private Card[] toolCards;
    public Card[] GetToolCards() { return toolCards; }
    public void SetToolCards(Card[] toolCards) { this.toolCards = toolCards; }

    [SerializeField]
    private Card[] seedCards;
    public Card[] GetSeedCards() { return seedCards; }
    public void SetSeedCards(Card[] seedCards) { this.seedCards = seedCards; }

    [SerializeField]
    private Card[] boosterCards;
    public Card[] GetBoosterCards() { return boosterCards; }
    public void SetBoosterCards(Card[] boosterCards) { this.boosterCards = boosterCards; }

    [SerializeField]
    private Card[] cropCards;
    public Card[] GetCropCards() { return cropCards; }
    public Card GetCropCardBySeedCardId(CardId cardId)
    {
        CardId targetCard = CardId.None;
        switch (cardId)
        {
            case CardId.LettuceSeed:
                targetCard = CardId.LettuceCrop;
                break;
            default:
                break;
        }

        foreach (Card cropCard in cropCards)
        {
            if (cropCard.GetCardId().Equals(targetCard)) { return cropCard; }
        }
        return cropCards[0];
    }

    [SerializeField]
    private int cardsPerToolPack;
    public int GetCardsPerToolPack() { return cardsPerToolPack; }
    public void SetCardsPerToolPack(int cardsPerPack) { this.cardsPerToolPack = cardsPerPack; }

    [SerializeField]
    private int cardsPerSeedPack;
    public int GetCardsPerSeedPack() { return cardsPerSeedPack; }
    public void SetCardsPerSeedPack(int cardsPerSeedPack) { this.cardsPerSeedPack = cardsPerSeedPack; }

    [SerializeField]
    private int cardsPerBoosterPack;
    public int GetCardsPerBoosterPack() { return cardsPerBoosterPack; }
    public void SetCardsPerBoosterPack(int cardsPerBoosterPack) { this.cardsPerBoosterPack = cardsPerBoosterPack; }

    [SerializeField]
    private int cardsToolPackPrice;
    public int GetToolPackPrice() { return cardsToolPackPrice; }

    [SerializeField]
    private int cardsSeedPackPrice;
    public int GetSeedPackPrice() { return cardsSeedPackPrice; }

    [SerializeField]
    private int cardsBoosterPackPrice;
    public int GetBoosterPackPrice() { return cardsBoosterPackPrice; }

    [SerializeField]
    private int cardWidth;
    public int GetCardWidth() { return cardWidth; }
    public void SetCardWidth(int cardWidth) { this.cardWidth = cardWidth; }

    [SerializeField]
    private int cardHeight;
    public int GetCardHeight() { return cardHeight; }

    [SerializeField]
    private int cardSpacing;
    public int GetCardSpacing() { return cardSpacing; }
    public void SetCardSpacing(int cardSpacing) { this.cardSpacing = cardSpacing; }

    [SerializeField]
    private int cardSellCount;
    public int GetCardSellCount() { return cardSellCount; }

    [Header("Deck Settings")]
    [SerializeField]
    private int deckCardsPerRow;
    public int GetDeckCardsPerRow() { return deckCardsPerRow; }

    [SerializeField]
    private int deckRowCount;
    public int GetDeckRowCount() { return deckRowCount; }

    [Header("Map Settings")]
    [SerializeField]
    private int mapBaseWidth;
    public int GetMapBaseWidth() { return mapBaseWidth; }
    public void SetMapBaseWidth(int mapBaseWidth) { this.mapBaseWidth = mapBaseWidth; }

    [SerializeField]
    private float mapOvergrowthLocationVariation;
    public float GetMapOvergrowthLocationVariation() { return mapOvergrowthLocationVariation; }
    public void SetMapOvergrowthLocationVariation(float mapOvergrowthLocationVariation) { this.mapOvergrowthLocationVariation = mapOvergrowthLocationVariation; }

    [SerializeField]
    private float mapOvergrowthMaxHeight;
    public float GetMapOvergrowthMaxHeight() { return mapOvergrowthMaxHeight; }
    public void SetMapOvergrowthMaxHeight(float mapOvergrowthMaxHeight) { this.mapOvergrowthMaxHeight = mapOvergrowthMaxHeight; }

    [SerializeField]
    private float mapOvergrowthScaleVariation;
    public float GetMapOvergrowthScaleVariation() { return mapOvergrowthScaleVariation; }
    public void SetMapOvergrowthScaleVariation(float mapOvergrowthScaleVariation) { this.mapOvergrowthScaleVariation = mapOvergrowthScaleVariation; }

    [Header("Plant Settings")]
    [SerializeField]
    private float plantBaseGrowthChancePercentage;
    public float GetPlantBaseGrowthChangePercentage() { return plantBaseGrowthChancePercentage; }

    [SerializeField]
    private float plantGrowthTickDuration;
    public float GetPlantGrowthTickDuration() { return plantGrowthTickDuration; }

    [Header("Player Settings")]
    [SerializeField]
    private float playerHeightMinimum;
    public float GetPlayerHeightMinimum() { return playerHeightMinimum; }
    public void SetPlayerHeightMinimum(float playerHeightMinimum) { this.playerHeightMinimum = playerHeightMinimum; }

    [SerializeField]
    private float playerLooksSensitivity;
    public float GetPlayerLookSensitivity() { return playerLooksSensitivity; }
    public void SetPlayerLookSensitivity(float playerLooksSensitivity) { this.playerLooksSensitivity = playerLooksSensitivity; }

    [SerializeField]
    private float playerMoveSpeed;
    public float GetPlayerMoveSpeed() { return playerMoveSpeed; }
    public void SetPlayerMoveSpeed(float playerMoveSpeed) { this.playerMoveSpeed = playerMoveSpeed; }

    [SerializeField]
    private PlayerViewState playerViewState = PlayerViewState.Game;
    public PlayerViewState GetPlayerViewState() { return playerViewState; }
    public void SetPlayerViewState(PlayerViewState playerViewState) { this.playerViewState = playerViewState; }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        MapManager.instance.CreateBaseMap();
    }

    public Card[] GetCardsForPack(PackType packType)
    {
        Card[] packCards = new Card[0];
        int totalCards = 0;
        if (PackType.Tool.Equals(packType))
        {
            packCards = GetToolCards();
            totalCards = GetCardsPerToolPack();
        }
        if (PackType.Seed.Equals(packType))
        {
            packCards = GetSeedCards();
            totalCards = GetCardsPerSeedPack();
        }
        if (PackType.Booster.Equals(packType))
        {
            packCards = GetBoosterCards();
            totalCards = GetCardsPerBoosterPack();
        }

        Card[] returnCards = new Card[totalCards];

        List<Card> commonCards = new List<Card>();
        List<Card> uncommonCards = new List<Card>();
        List<Card> rareCards = new List<Card>();
        List<Card> legendaryCards = new List<Card>();
        List<Card> mythicalCards = new List<Card>();
        foreach (Card packCard in packCards)
        {
            if (Rarity.Common.Equals(packCard.GetCardRarity())) { commonCards.Add(packCard); }
            if (Rarity.Uncommon.Equals(packCard.GetCardRarity())) { uncommonCards.Add(packCard); }
            if (Rarity.Rare.Equals(packCard.GetCardRarity())) { rareCards.Add(packCard); }
            if (Rarity.Legendary.Equals(packCard.GetCardRarity())) { legendaryCards.Add(packCard); }
            if (Rarity.Mythical.Equals(packCard.GetCardRarity())) { mythicalCards.Add(packCard); }
        }

        for (int cardIndex = 0; cardIndex < returnCards.Length; cardIndex++)
        {
            int rarity = Random.Range(0, 101);
            if (rarity < 80) // Common
            {
                int rarityIndex = Random.Range(0, commonCards.Count);
                returnCards[cardIndex] = commonCards[rarityIndex].CopyCard();
            }
            else if (rarity < 90) // Uncommon
            {
                int rarityIndex = Random.Range(0, uncommonCards.Count);
                returnCards[cardIndex] = uncommonCards[rarityIndex].CopyCard();
            }
            else if (rarity < 97) // Rare
            {
                int rarityIndex = Random.Range(0, rareCards.Count);
                returnCards[cardIndex] = rareCards[rarityIndex].CopyCard();
            }
            else if (rarity < 99) // Legendary
            {
                int rarityIndex = Random.Range(0, legendaryCards.Count);
                returnCards[cardIndex] = legendaryCards[rarityIndex].CopyCard();
            }
            else // Mythical
            {
                int rarityIndex = Random.Range(0, mythicalCards.Count);
                returnCards[cardIndex] = mythicalCards[rarityIndex].CopyCard();
            }
        }

        return returnCards;
    }

    public Card[] GenerateCardsForSeed(Card seedCard)
    {
        int cropQuantity = 0;
        switch (seedCard.GetCardRarity())
        {
            case Rarity.Common:
                cropQuantity = 1;
                break;
            case Rarity.Uncommon:
                cropQuantity = Random.Range(1, 3);
                break;
            case Rarity.Rare:
                cropQuantity = Random.Range(1, 4);
                break;
            case Rarity.Legendary:
                cropQuantity = Random.Range(1, 5);
                break;
            case Rarity.Mythical:
                cropQuantity = Random.Range(1, 6);
                break;
            default:
                break;
        }

        Card cropCard = GetCropCardBySeedCardId(seedCard.GetCardId());
        Card[] returnCards = new Card[cropQuantity];
        for (int cardIndex = 0; cardIndex < returnCards.Length; cardIndex++)
        {
            Card newCard = cropCard.CopyCard();
            int qualityOdds = Random.Range(0, 100);
            if (qualityOdds < 10)
            { // Bad
                newCard.SetCardQuality(Quality.Bad);
            }
            else if (qualityOdds < 80)
            { // Normal
                newCard.SetCardQuality(Quality.Normal);
            }
            else if (qualityOdds < 90)
            { // Good
                newCard.SetCardQuality(Quality.Good);
            }
            else if (qualityOdds < 95)
            { // Better
                newCard.SetCardQuality(Quality.Better);
            }
            else if (qualityOdds == 99)
            { // Best
                newCard.SetCardQuality(Quality.Best);
            }

            returnCards[cardIndex] = newCard;
        }

        return returnCards;
    }

    public void SellCard(Card card)
    {
        Debug.Log("Selling " + card.GetCardId());

        // Add the sell value of the card to the player's balance.
        int saleAmount = GetSaleAmount(card);
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoneyManager>().AddMoney(saleAmount);

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDeckManager>().RemoveCardFromDeck(card);
        ViewManager.instance.OpenShopView();
    }

    public int GetSaleAmount(Card card)
    {
        // Add the sell value of the card to the player's balance.
        float multiplier = 0;
        switch (card.GetCardQuality())
        {
            case Quality.Bad:
                multiplier = 0.5f;
                break;
            case Quality.Normal:
                multiplier = 1f;
                break;
            case Quality.Good:
                multiplier = 1.5f;
                break;
            case Quality.Better:
                multiplier = 2f;
                break;
            case Quality.Best:
                multiplier = 3f;
                break;
            default:
                break;
        }
        return Mathf.CeilToInt(card.GetCardValue() * multiplier);
    }
}