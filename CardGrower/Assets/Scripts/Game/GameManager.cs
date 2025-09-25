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
    private int cardsPerToolPack;
    public int GetCardsPerToolPack() { return cardsPerToolPack; }
    public void SetCardsPerToolPack(int cardsPerPack) { this.cardsPerToolPack = cardsPerPack; }

    [SerializeField]
    private int cardsPerSeedPack;
    public int GetCardsPerSeedPack() { return cardsPerSeedPack; }
    public void SetCardsPerSeedPack(int cardsPerSeedPack) { this.cardsPerSeedPack = cardsPerSeedPack; }

    [SerializeField]
    private int cardWidth;
    public int GetCardWidth() { return cardWidth; }
    public void SetCardWidth(int cardWidth) { this.cardWidth = cardWidth; }

    [SerializeField]
    private int cardSpacing;
    public int GetCardSpacing() { return cardSpacing; }
    public void SetCardSpacing(int cardSpacing) { this.cardSpacing = cardSpacing; }

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
            int rarity = Random.Range(0, 1000001);
            if (rarity < 800000) // Common
            {
                int rarityIndex = Random.Range(0, commonCards.Count);
                returnCards[cardIndex] = commonCards[rarityIndex].CopyCard();
            }
            else if (rarity < 900000) // Uncommon
            {
                int rarityIndex = Random.Range(0, uncommonCards.Count);
                returnCards[cardIndex] = uncommonCards[rarityIndex].CopyCard();
            }
            else if (rarity < 998000) // Rare
            {
                int rarityIndex = Random.Range(0, rareCards.Count);
                returnCards[cardIndex] = rareCards[rarityIndex].CopyCard();
            }
            else if (rarity < 999500) // Legendary
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
}

public enum PlayerViewState
{
    Game,
    Shop,
    OpenPack
}

public enum PackType
{
    Tool,
    Seed
}

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Legendary,
    Mythical
}

public enum Quality
{
    Bad,
    Normal,
    Good,
    Better,
    Best
}