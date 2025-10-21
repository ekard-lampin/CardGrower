using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    void Awake()
    {
        instance = this;
        Random.InitState(mapSeed);
        RetrieveSaveFile();
    }

    [Header("Audio Settings")]
    [SerializeField]
    private float audioWildlifeBaseDelay;

    [SerializeField]
    private float audioWildlifeDelayVariance;
    public float GetWildlifeAudioDelay() { return audioWildlifeBaseDelay * Random.Range(1f - audioWildlifeDelayVariance, 1f + audioWildlifeDelayVariance); }

    [SerializeField]
    private int audioAmbienceSoundCount;
    public int GetAmbienceSoundCount() { return audioAmbienceSoundCount; }

    [SerializeField]
    private int audioVoiceSoundCount;
    public int GetVoiceSoundCount() { return audioVoiceSoundCount; }

    [SerializeField]
    private int audioMoneySoundCount;
    public int GetMoneySoundCount() { return audioMoneySoundCount; }

    [SerializeField]
    private int audioPullSoundCount;
    public int GetPullSoundCount() { return audioPullSoundCount; }

    [SerializeField]
    private int audioMusicCount;
    public int GetMusicCount() { return audioMusicCount; }

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
            case CardId.CarrotSeed:
                targetCard = CardId.CarrotCrop;
                break;
            case CardId.CornSeed:
                targetCard = CardId.CornCrop;
                break;
            case CardId.CucumberSeed:
                targetCard = CardId.CucumberCrop;
                break;
            case CardId.PumpkinSeed:
                targetCard = CardId.PumpkinCrop;
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
    public Card GetCardById(CardId cardId)
    {
        Card[] returnCard = new Card[0];

        foreach (Card card in toolCards) { if (cardId.Equals(card.GetCardId())) { returnCard = new Card[] { card }; } }
        foreach (Card card in seedCards) { if (cardId.Equals(card.GetCardId())) { returnCard = new Card[] { card }; } }
        foreach (Card card in boosterCards) { if (cardId.Equals(card.GetCardId())) { returnCard = new Card[] { card }; } }

        return returnCard[0];
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

    [SerializeField]
    private float cardAppearDuration;
    public float GetCardAppearDuration() { return cardAppearDuration; }

    [SerializeField]
    private float cardFlipDuration;
    public float GetCardFlipDuration() { return cardFlipDuration; }

    [Header("Deck Settings")]
    [SerializeField]
    private int deckCardsPerRow;
    public int GetDeckCardsPerRow() { return deckCardsPerRow; }

    [SerializeField]
    private int deckRowCount;
    public int GetDeckRowCount() { return deckRowCount; }

    [Header("Game Settings")]
    [SerializeField]
    [Range(0, 1)]
    private float gameMainVolume;
    public float GetGameMainVolume() { return gameMainVolume; }
    public void SetGameMainVolume(float gameMainVolume) 
    {
        this.gameMainVolume = gameMainVolume;
        AudioManager.instance.SetMainVolume();
    }

    [SerializeField]
    [Range(0, 1)]
    private float gameMusicVolume;
    public float GetGameMusicVolume() { return gameMusicVolume; }
    public void SetGameMusicVolume(float gameMusicVolume)
    {
        this.gameMusicVolume = gameMusicVolume;
        AudioManager.instance.SetMusicVolume();
    }

    [SerializeField]
    [Range(0, 1)]
    private float gameSfxVolume;
    public float GetGameSfxVolume() { return gameSfxVolume; }
    public void SetGameSfxVolume(float gameSfxVolume)
    {
        this.gameSfxVolume = gameSfxVolume;
        AudioManager.instance.SetSfxVolume();
    }

    [SerializeField]
    private float gameMusicDelay;
    public float GetGameMusicDelay() { return gameMusicDelay; }

    [SerializeField]
    private AudioClip gameMenuMusicClip;
    public AudioClip GetGameMenuMusic() { return gameMenuMusicClip; }

    private bool gameIsFirstToolBuy = true;
    public bool IsFirstToolBuy() { return gameIsFirstToolBuy; }
    public void SetFirstToolBuy(bool isFirstToolBuy) { this.gameIsFirstToolBuy = isFirstToolBuy; }

    private bool gameIsFirstSeedBuy = true;
    public bool IsFirstSeedBuy() { return gameIsFirstSeedBuy; }
    public void SetFirstSeedBuy(bool isFirstSeedBuy) { gameIsFirstSeedBuy = isFirstSeedBuy; }

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

    [SerializeField]
    private int mapTreeRingOffset;
    public float GetMapTreeRingOffset() { return (float)mapTreeRingOffset * 2f; }

    [SerializeField]
    private float mapTreeScaleVariance;
    public float GetMapTreeScaleVariance() { return mapTreeScaleVariance; }

    [SerializeField]
    private int mapFillerRingOffset;
    public float GetMapFillerRingOffset() { return (float)mapFillerRingOffset * 2f; }

    [SerializeField]
    private int mapSeed;

    [SerializeField]
    private float mapCloudSpawnX;
    public float GetMapCloudSpawnX() { return mapCloudSpawnX; }

    [SerializeField]
    private float mapCloudSpawnZBreadth;
    public float GetMapCloudSpawnZBreadth() { return mapCloudSpawnZBreadth; }

    [SerializeField]
    private float mapCloudSpawnHeight;
    public float GetMapCloudSpawnHeight() { return mapCloudSpawnHeight; }

    [SerializeField]
    private float mapCloudSpawnDelay;
    public float GetMapCloudSpawnDelay() { return mapCloudSpawnDelay; }

    [SerializeField]
    private float mapCloudSpeed;
    public float GetMapCloudSpeed() { return mapCloudSpeed; }

    [SerializeField]
    private int mapCloudCount;
    public int GetMapCloudCount() { return mapCloudCount; }

    [SerializeField]
    private float mapTreeRippleSpeed;
    public float GetMapTreeRippleSpeed() { return mapTreeRippleSpeed; }

    [SerializeField]
    private float mapTreeRippleMaxLean;
    public float GetMapTreeRippleMaxLean() { return mapTreeRippleMaxLean; }

    [Header("Plant Settings")]
    [SerializeField]
    private float plantGrowthRollRange;
    public float GetPlantGrowthRollRange() { return plantGrowthRollRange; }

    [SerializeField]
    private float plantBaseGrowthChancePercentage;
    public float GetPlantBaseGrowthChangePercentage() { return plantBaseGrowthChancePercentage; }

    [SerializeField]
    private float plantGrowthTickDuration;
    public float GetPlantGrowthTickDuration() { return plantGrowthTickDuration; }

    [SerializeField]
    private float plantTimeSaverFactor;
    public float GetPlantTimeSaverFactor() { return plantTimeSaverFactor; }

    [Header("Player Settings")]
    [SerializeField]
    private float playerHeightMinimum;
    public float GetPlayerHeightMinimum() { return playerHeightMinimum; }
    public void SetPlayerHeightMinimum(float playerHeightMinimum) { this.playerHeightMinimum = playerHeightMinimum; }

    [SerializeField]
    [Range(0, 2)]
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
    public void SetPlayerViewState(PlayerViewState playerViewState) { previousPlayerViewState = this.playerViewState; this.playerViewState = playerViewState; }

    [SerializeField]
    private PlayerViewState previousPlayerViewState = PlayerViewState.Game;
    public PlayerViewState GetPreviousPlayerViewState() { return previousPlayerViewState; }

    [SerializeField]
    private float playerPassiveMoneyTimerDuration;
    public float GetPlayerPassiveMoneyTimerDuration() { return playerPassiveMoneyTimerDuration; }

    [Header("Start Menu Settings")]
    [SerializeField]
    private Vector3 startMenuDisplayObjectLocation;
    public Vector3 GetStartMenuDisplayObjectLocation() { return startMenuDisplayObjectLocation; }

    [SerializeField]
    private SaveObject[] loadedSaveObject = new SaveObject[0];
    public SaveObject GetSaveObject() { return loadedSaveObject[0]; }
    public bool IsSaveFileLoaded() { return loadedSaveObject.Length > 0; }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        previousPlayerViewState = GetPlayerViewState();

        if (PlayerViewState.StartMenu.Equals(GetPlayerViewState()))
        {
            ViewManager.instance.OpenStartMenuView();
            GameObject menuDisplayObject = Instantiate(
                Resources.Load<GameObject>("Prefabs/Map/StartMenuDisplayPrefab"),
                GetStartMenuDisplayObjectLocation(),
                Quaternion.identity
            );
            menuDisplayObject.name = "StartMenuDisplayPrefab";
            Camera.main.transform.position = Vector3.zero;
            Camera.main.transform.rotation = Quaternion.identity;
            for (int lineIndex = -40; lineIndex < 40; lineIndex++)
            {
                for (int tileIndex = 0; tileIndex < 20; tileIndex++)
                {
                    GameObject newTileObject = Instantiate(
                        Resources.Load<GameObject>("Prefabs/Map/ExteriorMapTilePrefab"),
                        new Vector3(lineIndex, -6.5f, 10 + tileIndex),
                        Quaternion.identity
                    );
                    newTileObject.transform.SetParent(GameObject.FindGameObjectWithTag("Trees").transform);
                }
            }

            for (int treeIndex = -20; treeIndex < 20; treeIndex++)
            {
                GameObject newTreeObject = Instantiate(
                    Resources.Load<GameObject>("Prefabs/Map/TreePrefab"),
                    new Vector3(treeIndex, -6.5f, 15 + Random.Range(-5, 5)),
                    Quaternion.identity
                );
                newTreeObject.transform.SetParent(GameObject.FindGameObjectWithTag("Trees").transform);
                newTreeObject.transform.localScale = new Vector3(1, 1 + (Random.Range(0, GetMapTreeScaleVariance())), 1);
                newTreeObject.transform.localRotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
            }

            AudioManager.instance.PlayMenuMusic();
        }
        if (PlayerViewState.Game.Equals(GetPlayerViewState())) {
            MapManager.instance.CreateBaseMap();
            MapManager.instance.CreatePlayer();
        }
        if (PlayerViewState.OpeningCutscene.Equals(GetPlayerViewState())) { CutsceneManager.instance.StartOpeningCutscene(); }
    }

    void Update()
    {
        
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
        Card hoeCard = new Card();
        Card lettuceSeedCard = new Card();
        foreach (Card packCard in packCards)
        {
            if (Rarity.Common.Equals(packCard.GetCardRarity())) { commonCards.Add(packCard); }
            if (Rarity.Uncommon.Equals(packCard.GetCardRarity())) { uncommonCards.Add(packCard); }
            if (Rarity.Rare.Equals(packCard.GetCardRarity())) { rareCards.Add(packCard); }
            if (Rarity.Legendary.Equals(packCard.GetCardRarity())) { legendaryCards.Add(packCard); }
            if (Rarity.Mythical.Equals(packCard.GetCardRarity())) { mythicalCards.Add(packCard); }

            if (CardId.Hoe.Equals(packCard.GetCardId())) { hoeCard = packCard; }
            if (CardId.LettuceSeed.Equals(packCard.GetCardId())) { lettuceSeedCard = packCard; }
        }

        for (int cardIndex = 0; cardIndex < returnCards.Length; cardIndex++)
        {
            if (PackType.Tool.Equals(packType) && IsFirstToolBuy())
            {
                returnCards[cardIndex] = hoeCard.CopyCard();
                continue;
            }
            if (PackType.Seed.Equals(packType) && IsFirstSeedBuy())
            {
                returnCards[cardIndex] = lettuceSeedCard.CopyCard();
                continue;
            }

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
        if (PackType.Tool.Equals(packType) && IsFirstToolBuy()) { SetFirstToolBuy(false); }
        if (PackType.Seed.Equals(packType) && IsFirstSeedBuy()) { SetFirstSeedBuy(false); }

        return returnCards;
    }

    public Card[] GenerateCardsForSeed(PlacementPlant plant, Card[] selectedCard)
    {
        int cropQuantity = 0;
        Card seedCard = plant.GetPlantedSeed();
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

        if (selectedCard.Length > 0 && CardType.Tool.Equals(selectedCard[0].GetCardType()) && !CardId.Hoe.Equals(selectedCard[0].GetCardId()))
        {
            if (CardId.Netting.Equals(selectedCard[0].GetCardId()) && Random.Range(0, 2) > 0) { cropQuantity++; }
            if (CardId.Gloves.Equals(selectedCard[0].GetCardId()))
            {
                if (Random.Range(0, 2) > 0) { cropQuantity += 2; } else { cropQuantity++; }
            }
        }

        Card cropCard = GetCropCardBySeedCardId(seedCard.GetCardId());
        Card[] returnCards = new Card[cropQuantity];
        for (int cardIndex = 0; cardIndex < returnCards.Length; cardIndex++)
        {
            Card newCard = cropCard.CopyCard();
            int qualityOdds = Random.Range(0, 100);
            bool beehiveTool = false;
            bool fungusBooster = false;

            // Check for used tools.
            if (selectedCard.Length > 0 && CardType.Tool.Equals(selectedCard[0].GetCardType()) && !CardId.Hoe.Equals(selectedCard[0].GetCardId()))
            {
                if (CardId.Shears.Equals(selectedCard[0].GetCardId())) { qualityOdds += 15; }
                if (CardId.Beehive.Equals(selectedCard[0].GetCardId()) && cardIndex == 0) { beehiveTool = true; }
            }

            // Check for boosters
            if (plant.IsBoosterActive(CardId.Trellis)) { qualityOdds += 15; }
            if (plant.IsBoosterActive(CardId.Fungus) && cardIndex == 0) { fungusBooster = true; }

            if (qualityOdds < 10)
            { // Bad
                newCard.SetCardQuality((!beehiveTool && !fungusBooster) ? Quality.Bad : Quality.Normal);
            }
            else if (qualityOdds < 80)
            { // Normal
                newCard.SetCardQuality((!beehiveTool && !fungusBooster) ? Quality.Normal : Quality.Good);
            }
            else if (qualityOdds < 90)
            { // Good
                newCard.SetCardQuality((!beehiveTool && !fungusBooster) ? Quality.Good : Quality.Better);
            }
            else if (qualityOdds < 95)
            { // Better
                newCard.SetCardQuality((!beehiveTool && !fungusBooster) ? Quality.Better : Quality.Best);
            }
            else if (qualityOdds == 99)
            { // Best
                newCard.SetCardQuality(Quality.Best);
            }

            returnCards[cardIndex] = newCard;
        }

        if (selectedCard.Length > 0)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDeckManager>().RemoveCardFromDeck(selectedCard[0]);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDeckManager>().SetSelectedCard(new Card[0]);
            ViewManager.instance.SetOpenView(null);
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
        TutorialManager.instance.UpdateTrackedSellingAction();
        AudioManager.instance.PlayMoneySound();
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

    public void StartButtonClicked()
    {
        // Clear save data.
        loadedSaveObject = new SaveObject[0];
        
        // Clean up start menu objects.
        if (GameObject.FindGameObjectWithTag("StartMenuDisplay") != null) { Destroy(GameObject.FindGameObjectWithTag("StartMenuDisplay")); }
        foreach (Transform child in GameObject.FindGameObjectWithTag("Trees").transform) { Destroy(child.gameObject); }
        ViewManager.instance.DestroyOpenView();

        // Call first cutscene start.
        SetPlayerViewState(PlayerViewState.OpeningCutscene);
        CutsceneManager.instance.StartOpeningCutscene();

        // Stop menu music.
        AudioManager.instance.StopMusic();
    }

    public void LoadButtonClicked()
    {
        // Clean up start menu objects.
        if (GameObject.FindGameObjectWithTag("StartMenuDisplay") != null) { Destroy(GameObject.FindGameObjectWithTag("StartMenuDisplay")); }
        foreach (Transform child in GameObject.FindGameObjectWithTag("Trees").transform) { Destroy(child.gameObject); }
        ViewManager.instance.DestroyOpenView();

        // Stop menu music.
        AudioManager.instance.StopMusic();

        // Call map creation.
        mapSeed = loadedSaveObject[0].GetSeed();
        Random.InitState(mapSeed);
        MapManager.instance.CreateBaseMap();
        MapManager.instance.CreatePlayer();
        MapManager.instance.LoadSaveData();

        SetPlayerViewState(PlayerViewState.Game);
    }

    public void StartGame()
    {
        SetPlayerViewState(PlayerViewState.Game);
        MapManager.instance.CreatePlayer();
        TutorialManager.instance.ResetTutorialFlags();
        TutorialManager.instance.SetTutorialState(TutorialState.Look);
    }

    public void SaveGame()
    {
        Debug.Log("Saving game.");
        if (!PlayerViewState.Game.Equals(GetPreviousPlayerViewState())) { return; }
        if (!TutorialState.None.Equals(TutorialManager.instance.GetTutorialState())) { return; }

        SaveObject saveObject = new SaveObject();
        saveObject.SetSeed(mapSeed);

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            SavePlayerObject savePlayerObject = new SavePlayerObject();
            savePlayerObject.SetBalance(playerObject.GetComponent<PlayerMoneyManager>().GetPlayerBalance());

            List<Card> cards = playerObject.GetComponent<PlayerDeckManager>().GetDeck();
            SaveCardObject[] saveCards = new SaveCardObject[cards.Count];
            for (int cardIndex = 0; cardIndex < saveCards.Length; cardIndex++)
            {
                Card card = cards[cardIndex];

                SaveCardObject saveCard = new SaveCardObject();
                saveCard.SetCardId(card.GetCardId());
                saveCard.SetCardType(card.GetCardType());
                saveCard.SetCardTexture(card.GetCardTexture().name);
                saveCard.SetCardDescription(card.GetCardDescription());
                saveCard.SetCardRarity(card.GetCardRarity());
                saveCard.SetCardQuality(card.GetCardQuality());
                saveCard.SetCardValue(card.GetCardValue());

                saveCards[cardIndex] = saveCard;
            }
            savePlayerObject.SetCards(saveCards);

            saveObject.SetSavePlayerObject(savePlayerObject);
        }

        List<SaveTile> saveTiles = new List<SaveTile>();
        foreach (Transform tileTransform in GameObject.FindGameObjectWithTag("FarmTiles").transform)
        {
            if (!tileTransform.gameObject.name.ToLower().Contains("tile")) { continue; }

            Tile tile = tileTransform.gameObject.GetComponent<Tile>();
            if (TileState.Overgrown.Equals(tile.GetTileState())) { continue; }

            SaveTile saveTile = new SaveTile();
            saveTile.SetXCoord((int)tile.GetTileCoords().x);
            saveTile.SetZCoord((int)tile.GetTileCoords().y);
            saveTile.SetTileState(tile.GetTileState());

            if (tile.GetPlacement() != null && tile.GetPlacement() is PlacementPlant)
            {
                PlacementPlant plant = tile.GetPlacement() as PlacementPlant;
                saveTile.SetPlantedSeed(plant.GetPlantedSeed().GetCardId());
                saveTile.SetPlantGrowthStage(plant.GetGrowthStage());
                saveTile.SetActiveBoosters(plant.GetActiveBoosters().ToArray());
            }

            saveTiles.Add(saveTile);
        }
        if (saveTiles.Count > 0) { saveObject.SetSaveTiles(saveTiles.ToArray()); }

        string json = JsonUtility.ToJson(saveObject);

        Debug.Log(json);
        File.WriteAllText(Application.persistentDataPath + "/saveData.json", json);
    }

    private void RetrieveSaveFile()
    {
        DirectoryInfo saveDirectory = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] saveFileInfo = saveDirectory.GetFiles("saveData.json");

        if (saveFileInfo.Length == 0) { return; }

        string saveFileRaw = File.ReadAllText(Application.persistentDataPath + "/saveData.json");
        SaveObject saveObject = JsonUtility.FromJson<SaveObject>(saveFileRaw);
        loadedSaveObject = new SaveObject[] { saveObject };
    }
}