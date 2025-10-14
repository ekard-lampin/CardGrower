using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewManager : MonoBehaviour
{
    public static ViewManager instance;
    void Awake() { instance = this; }

    private GameObject openView;
    public GameObject GetOpenView() { return openView; }
    public void SetOpenView(GameObject openView)
    {
        if (this.openView != null) { Destroy(this.openView); }
        this.openView = openView;

        if (openView != null && !openView.name.ToLower().Contains("selected"))
        {
            displayCard = null;
            if (GameObject.FindGameObjectWithTag("Player") != null) { GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDeckManager>().SetSelectedCard(new Card[0]); }
        }

        if (openView != null && !openView.name.Equals("ShopViewPrefab")) { sellStartIndex = 0; }
        if (sellStartIndex > 0 && sellStartIndex >= GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDeckManager>().GetDeck().Count) { sellStartIndex -= GameManager.instance.GetCardSellCount(); }

        characterIndex = 0;
        characterTimer = 0;
    }
    public void DestroyOpenView()
    {
        SetOpenView(null);
    }

    [SerializeField]
    private int deckViewStartIndex = 0;

    [SerializeField]
    private Card displayCard = null;

    [SerializeField]
    private int sellStartIndex = 0;

    [SerializeField]
    private DialogueStep dialogueStep;

    [SerializeField]
    private int characterIndex = 0;

    [SerializeField]
    private float characterTimer = 0;

    void Update()
    {
        if (!PlayerViewState.Game.Equals(GameManager.instance.GetPlayerViewState()) && GameObject.FindGameObjectWithTag("Canvas").transform.Find("UI").gameObject.activeSelf)
        {
            GameObject.FindGameObjectWithTag("Canvas").transform.Find("UI").gameObject.SetActive(false);
        }
        if (PlayerViewState.Game.Equals(GameManager.instance.GetPlayerViewState()) && !GameObject.FindGameObjectWithTag("Canvas").transform.Find("UI").gameObject.activeSelf)
        {
            GameObject.FindGameObjectWithTag("Canvas").transform.Find("UI").gameObject.SetActive(true);
        }

        if (!PlayerViewState.Game.Equals(GameManager.instance.GetPlayerViewState()))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        if (PlayerViewState.Game.Equals(GameManager.instance.GetPlayerViewState()))
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (openView != null && openView.name.Equals("DialoguePrefab"))
        {
            if (characterIndex == dialogueStep.GetText().Length) { return; }
            
            characterTimer += Time.deltaTime;

            if (characterTimer < 0.01f) { return; }
            characterTimer = 0;
            
            string currentText = openView.transform.Find("Background").Find("Text").gameObject.GetComponent<Text>().text;
            if (characterIndex == 0 && !dialogueStep.IsItalicized()) { currentText += "\""; }
            currentText += dialogueStep.GetText()[characterIndex];
            if (characterIndex == dialogueStep.GetText().Length - 1 && !dialogueStep.IsItalicized()) { currentText += "\""; }
            openView.transform.Find("Background").Find("Text").gameObject.GetComponent<Text>().text = currentText;

            characterIndex++;
        }
    }

    public void ToggleDeckView()
    {
        if (PlayerViewState.Game.Equals(GameManager.instance.GetPlayerViewState()))
        {
            OpenDeckView();
        }
        else if (PlayerViewState.Deck.Equals(GameManager.instance.GetPlayerViewState()))
        {
            GameManager.instance.SetPlayerViewState(PlayerViewState.Game);

            SetOpenView(null);
        }
    }

    private void OpenDeckView()
    {
        GameManager.instance.SetPlayerViewState(PlayerViewState.Deck);

        // Create deck view object.
        GameObject deckViewObject = Instantiate(
            Resources.Load<GameObject>("Prefabs/Views/DeckViewPrefab"),
            GameObject.FindGameObjectWithTag("Canvas").transform
        );
        deckViewObject.name = "DeckViewObject";

        deckViewStartIndex = 0;
        displayCard = null;
        RenderCards(deckViewStartIndex);

        // Set click actions for buttons.
        GameObject deckBackgroundObject = deckViewObject.transform.Find("DeckBackground").gameObject;
        GameObject backButtonObject = deckBackgroundObject.transform.Find("BackButtonObject").Find("Button").gameObject;
        backButtonObject.GetComponent<Button>().onClick.AddListener(PreviousDeckPage);

        GameObject nextButtonObject = deckBackgroundObject.transform.Find("ForwardButtonObject").Find("Button").gameObject;
        nextButtonObject.GetComponent<Button>().onClick.AddListener(NextDeckPage);

        GameObject cardInfoBackgroundObject = deckViewObject.transform.Find("CardInfoBackground").gameObject;
        GameObject selectButton = cardInfoBackgroundObject.transform.Find("SelectButtonObject").Find("Button").gameObject;
        selectButton.GetComponent<Button>().onClick.AddListener(SelectCard);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SetOpenView(deckViewObject);
    }

    private void PreviousDeckPage()
    {
        if (deckViewStartIndex == 0) { return; }

        deckViewStartIndex -= (GameManager.instance.GetDeckCardsPerRow() * GameManager.instance.GetDeckRowCount());
        RenderCards(deckViewStartIndex);
    }

    private void NextDeckPage()
    {
        List<Card> playerDeck = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDeckManager>().GetDeck();
        if ((deckViewStartIndex + (GameManager.instance.GetDeckCardsPerRow() * GameManager.instance.GetDeckRowCount())) >= playerDeck.Count) { return; }

        deckViewStartIndex += (GameManager.instance.GetDeckCardsPerRow() * GameManager.instance.GetDeckRowCount());
        RenderCards(deckViewStartIndex);
    }

    private void SelectCard()
    {
        if (displayCard == null) { return; }

        // Set the player back in game mode with no open view.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SetOpenView(null);

        GameManager.instance.SetPlayerViewState(PlayerViewState.Game);

        // Create a new selected card view.
        GameObject selectedCardViewObject = Instantiate(
            Resources.Load<GameObject>("Prefabs/Views/SelectedCardViewPrefab"),
            GameObject.FindGameObjectWithTag("Canvas").transform
        );

        GameObject cardObject = selectedCardViewObject.transform.Find("CardObject").gameObject;
        CardComponent cardComponent = cardObject.GetComponent<CardComponent>();
        cardComponent.SetCard(displayCard);

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDeckManager>().SetSelectedCard(new Card[] { displayCard });

        SetOpenView(selectedCardViewObject);
    }

    private void RenderCards(int startIndex)
    {
        GameObject deckViewObject = GameObject.FindGameObjectWithTag("Canvas").transform.Find("DeckViewObject").gameObject;
        if (deckViewObject == null) { return; }

        // Get deck area and player deck.
        GameObject deckBackgroundObject = deckViewObject.transform.Find("DeckBackground").gameObject;
        List<Card> playerDeck = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDeckManager>().GetDeck();

        // Clear cards, if any are present.
        List<GameObject> objectsToDestroy = new List<GameObject>();
        foreach (Transform child in deckBackgroundObject.transform)
        {
            GameObject childObject = child.gameObject;
            if (childObject.name.ToLower().Contains("card")) { objectsToDestroy.Add(childObject); }
        }
        foreach (GameObject objectToDestroy in objectsToDestroy) { Destroy(objectToDestroy); }

        // Set anchor points and create a card for each of the player's cards.
        float xAnchor = ((float)GameManager.instance.GetCardWidth() * ((float)GameManager.instance.GetDeckCardsPerRow() - 1f) * -1f) - ((float)GameManager.instance.GetCardSpacing() * ((float)(GameManager.instance.GetDeckCardsPerRow() - 1) / 2f));
        float yAnchor = ((float)GameManager.instance.GetCardHeight() * ((float)GameManager.instance.GetDeckRowCount() - 1f)) + ((float)GameManager.instance.GetCardSpacing() * ((float)(GameManager.instance.GetDeckRowCount() - 1) / 2f));
        for (int rowIndex = 0; rowIndex < GameManager.instance.GetDeckRowCount(); rowIndex++)
        {
            for (int columnIndex = 0; columnIndex < GameManager.instance.GetDeckCardsPerRow(); columnIndex++)
            {
                int cardIndex = ((rowIndex * GameManager.instance.GetDeckCardsPerRow()) + columnIndex) + startIndex;
                if (cardIndex >= playerDeck.Count) { break; }

                GameObject newCardObject = Instantiate(
                    Resources.Load<GameObject>("Prefabs/Views/CardPrefab"),
                    deckBackgroundObject.transform
                );
                newCardObject.transform.localPosition = new Vector3(
                    xAnchor + (((GameManager.instance.GetCardWidth() * 2) + GameManager.instance.GetCardSpacing()) * columnIndex),
                    yAnchor + (((GameManager.instance.GetCardHeight() * 2) + GameManager.instance.GetCardSpacing()) * rowIndex * -1),
                    0
                );

                newCardObject.name = "Card_" + cardIndex;

                CardComponent newCard = newCardObject.GetComponent<CardComponent>();
                newCard.SetCard(playerDeck[cardIndex]);
            }
        }
    }

    public void ToggleShopView()
    {
        if (PlayerViewState.Game.Equals(GameManager.instance.GetPlayerViewState()))
        {
            OpenShopView();
        }
        else if (PlayerViewState.Shop.Equals(GameManager.instance.GetPlayerViewState()))
        {
            GameManager.instance.SetPlayerViewState(PlayerViewState.Game);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            SetOpenView(null);
        }
    }

    public void OpenShopView()
    {
        GameManager.instance.SetPlayerViewState(PlayerViewState.Shop);

        // Create shop object.
        GameObject shopObject = Instantiate(
            Resources.Load<GameObject>("Prefabs/Views/ShopViewPrefab"),
            GameObject.FindGameObjectWithTag("Canvas").transform
        );
        shopObject.name = "ShopViewPrefab";

        // Setup balance display.
        shopObject.transform.Find("PackBackground").Find("BalanceText").gameObject.GetComponent<Text>().text = "Balance: $" + GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoneyManager>().GetPlayerBalance();

        // Initialize shop buttons.
        PlayerMoneyManager playerMoneyManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoneyManager>();
        GameObject toolBuyButtonObject = shopObject.transform.Find("PackBackground").Find("ToolPackButtonObject").Find("Button").gameObject;
        toolBuyButtonObject.GetComponent<Button>().onClick.AddListener(ClickToolBuyButton);
        toolBuyButtonObject.transform.Find("Text").gameObject.GetComponent<Text>().text = "Buy ($" + GameManager.instance.GetToolPackPrice() + ")";
        if (playerMoneyManager.GetPlayerBalance() < GameManager.instance.GetToolPackPrice()) { toolBuyButtonObject.GetComponent<Button>().interactable = false; }

        GameObject seedBuyButtonObject = shopObject.transform.Find("PackBackground").Find("SeedPackButtonObject").Find("Button").gameObject;
        seedBuyButtonObject.GetComponent<Button>().onClick.AddListener(ClickSeedBuyButton);
        seedBuyButtonObject.transform.Find("Text").gameObject.GetComponent<Text>().text = "Buy ($" + GameManager.instance.GetSeedPackPrice() + ")";
        if (playerMoneyManager.GetPlayerBalance() < GameManager.instance.GetSeedPackPrice()) { seedBuyButtonObject.GetComponent<Button>().interactable = false; }

        GameObject boosterPackButtonObject = shopObject.transform.Find("PackBackground").Find("BoosterPackButtonObject").Find("Button").gameObject;
        boosterPackButtonObject.GetComponent<Button>().onClick.AddListener(ClickBoosterBuyButton);
        boosterPackButtonObject.transform.Find("Text").gameObject.GetComponent<Text>().text = "Buy ($" + GameManager.instance.GetBoosterPackPrice() + ")";
        if (playerMoneyManager.GetPlayerBalance() < GameManager.instance.GetBoosterPackPrice()) { boosterPackButtonObject.GetComponent<Button>().interactable = false; }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SetOpenView(shopObject);

        // Setup sell screen.
        RenderSellCards(sellStartIndex);

        GameObject sellSectionObject = GetOpenView().transform.Find("SellSectionBackground").gameObject;
        Button previousButton = sellSectionObject.transform.Find("BackButtonObject").Find("Button").gameObject.GetComponent<Button>();
        Button nextButton = sellSectionObject.transform.Find("ForwardButtonObject").Find("Button").gameObject.GetComponent<Button>();

        previousButton.onClick.AddListener(() =>
        {
            if (sellStartIndex == 0) { return; }
            Debug.Log("Rendering previous cards to sell.");
            sellStartIndex -= GameManager.instance.GetCardSellCount();
            RenderSellCards(sellStartIndex);
        });
        nextButton.onClick.AddListener(() =>
        {
            if ((sellStartIndex + GameManager.instance.GetCardSellCount()) >= GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDeckManager>().GetDeck().Count) { return; }
            Debug.Log("Rendering next cards to sell.");
            sellStartIndex += GameManager.instance.GetCardSellCount();
            RenderSellCards(sellStartIndex);
        });

        // Tutorial gatekeepers.
        if (!TutorialManager.instance.IsTutorialFlagSet(TutorialState.ToolShop))
        {
            shopObject.transform.Find("PackBackground").Find("ToolPackObject").gameObject.SetActive(false);
            shopObject.transform.Find("PackBackground").Find("ToolPackButtonObject").gameObject.SetActive(false);
        }

        if (!TutorialManager.instance.IsTutorialFlagSet(TutorialState.SeedShop))
        {
            shopObject.transform.Find("PackBackground").Find("SeedPackObject").gameObject.SetActive(false);
            shopObject.transform.Find("PackBackground").Find("SeedPackButtonObject").gameObject.SetActive(false);
        }

        if (!TutorialManager.instance.IsTutorialFlagSet(TutorialState.BoosterShop))
        {
            shopObject.transform.Find("PackBackground").Find("BoosterPackObject").gameObject.SetActive(false);
            shopObject.transform.Find("PackBackground").Find("BoosterPackButtonObject").gameObject.SetActive(false);
        }

        if (!TutorialState.None.Equals(TutorialManager.instance.GetTutorialState()))
        {
            if (!TutorialState.ToolShop.Equals(TutorialManager.instance.GetTutorialState()))
            {
                shopObject.transform.Find("PackBackground").Find("ToolPackButtonObject").Find("Button").gameObject.GetComponent<Button>().interactable = false;
            }
            if (!TutorialState.SeedShop.Equals(TutorialManager.instance.GetTutorialState()))
            {
                shopObject.transform.Find("PackBackground").Find("SeedPackButtonObject").Find("Button").gameObject.GetComponent<Button>().interactable = false;
            }
            if (!TutorialState.BoosterShop.Equals(TutorialManager.instance.GetTutorialState()))
            {
                shopObject.transform.Find("PackBackground").Find("BoosterPackButtonObject").Find("Button").gameObject.GetComponent<Button>().interactable = false;
            }
        }
    }

    private void RenderSellCards(int startingIndex)
    {
        PlayerDeckManager playerDeckManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDeckManager>();
        List<Card> playerDeck = playerDeckManager.GetDeck();
        GameObject sellSectionObject = GetOpenView().transform.Find("SellSectionBackground").gameObject;

        foreach (Transform child in sellSectionObject.transform)
        {
            if (child.gameObject.name.ToLower().Contains("card")) { Destroy(child.gameObject); }
        }

        int cardsToShow = GameManager.instance.GetCardSellCount();
        float xAnchor = ((float)GameManager.instance.GetCardWidth() * ((float)cardsToShow - 1f) * -1f) - ((float)GameManager.instance.GetCardSpacing() * ((float)(cardsToShow - 1) / 2f));
        for (int cardIndex = 0; cardIndex < cardsToShow; cardIndex++)
        {
            if ((cardIndex + startingIndex) >= playerDeckManager.GetDeck().Count) { break; }

            // Create card.
            GameObject newCardObject = Instantiate(
                Resources.Load<GameObject>("Prefabs/Views/CardPrefab"),
                sellSectionObject.transform
            );
            newCardObject.transform.localPosition = new Vector3(
                xAnchor + (((GameManager.instance.GetCardWidth() * 2) + GameManager.instance.GetCardSpacing()) * (cardIndex)),
                0,
                0
            );

            newCardObject.name = "Card_" + cardIndex;

            CardComponent newCard = newCardObject.GetComponent<CardComponent>();
            newCard.SetCard(playerDeck[cardIndex + startingIndex]);

            // Create sell button.
            GameObject sellButtonObject = Instantiate(
                Resources.Load<GameObject>("Prefabs/Views/SellButtonPrefab"),
                sellSectionObject.transform
            );
            sellButtonObject.transform.localPosition = new Vector3(
                xAnchor + (((GameManager.instance.GetCardWidth() * 2) + GameManager.instance.GetCardSpacing()) * (cardIndex)),
                0,
                0
            );
            sellButtonObject.name = "CardSellButton";
            sellButtonObject.GetComponent<CardSellButton>().SetCard(playerDeck[cardIndex + startingIndex]);

            // Set sell text.
            sellButtonObject.transform.Find("Button").Find("Text").gameObject.GetComponent<Text>().text = "Sell $" + GameManager.instance.GetSaleAmount(playerDeck[cardIndex + startingIndex]);
        }
    }

    public void ClickToolBuyButton()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoneyManager>().DeductMoney(GameManager.instance.GetToolPackPrice());

        GameManager.instance.SetPlayerViewState(PlayerViewState.OpenPack);

        GameObject packScreenObject = Instantiate(
            Resources.Load<GameObject>("Prefabs/Views/PackOpenPrefab"),
            GameObject.FindGameObjectWithTag("Canvas").transform
        );

        GameObject packScreenCloseButtonObject = packScreenObject.transform.Find("CloseButtonObject").Find("Button").gameObject;
        packScreenCloseButtonObject.GetComponent<Button>().onClick.AddListener(ClickPackCloseButton);

        Card[] packCards = GameManager.instance.GetCardsForPack(PackType.Tool);
        float xPos = ((float)GameManager.instance.GetCardWidth() * ((float)packCards.Length - 1f) * -1f) - ((float)GameManager.instance.GetCardSpacing() * ((float)(packCards.Length - 1) / 2f));
        for (int cardIndex = 0; cardIndex < packCards.Length; cardIndex++)
        {
            GameObject newCardObject = Instantiate(
                Resources.Load<GameObject>("Prefabs/Views/CardPrefab"),
                packScreenObject.transform
            );
            newCardObject.transform.localPosition = new Vector3(xPos, 0, 0);

            newCardObject.name = "Card_" + cardIndex;

            CardComponent newCard = newCardObject.GetComponent<CardComponent>();
            newCard.SetCard(packCards[cardIndex]);

            xPos += ((GameManager.instance.GetCardWidth() * 2) + GameManager.instance.GetCardSpacing());

            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDeckManager>().AddCardToDeck(packCards[cardIndex]);
        }
        TutorialManager.instance.UpdateTrackedToolShopAction();

        SetOpenView(packScreenObject);
    }

    public void ClickSeedBuyButton()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoneyManager>().DeductMoney(GameManager.instance.GetSeedPackPrice());

        GameManager.instance.SetPlayerViewState(PlayerViewState.OpenPack);

        GameObject packScreenObject = Instantiate(
            Resources.Load<GameObject>("Prefabs/Views/PackOpenPrefab"),
            GameObject.FindGameObjectWithTag("Canvas").transform
        );

        GameObject packScreenCloseButtonObject = packScreenObject.transform.Find("CloseButtonObject").Find("Button").gameObject;
        packScreenCloseButtonObject.GetComponent<Button>().onClick.AddListener(ClickPackCloseButton);

        Card[] packCards = GameManager.instance.GetCardsForPack(PackType.Seed);
        float xPos = ((float)GameManager.instance.GetCardWidth() * ((float)packCards.Length - 1f) * -1f) - ((float)GameManager.instance.GetCardSpacing() * ((float)(packCards.Length - 1) / 2f));
        for (int cardIndex = 0; cardIndex < packCards.Length; cardIndex++)
        {
            GameObject newCardObject = Instantiate(
                Resources.Load<GameObject>("Prefabs/Views/CardPrefab"),
                packScreenObject.transform
            );
            newCardObject.transform.localPosition = new Vector3(xPos, 0, 0);

            newCardObject.name = "Card_" + cardIndex;

            CardComponent newCard = newCardObject.GetComponent<CardComponent>();
            newCard.SetCard(packCards[cardIndex]);

            xPos += ((GameManager.instance.GetCardWidth() * 2) + GameManager.instance.GetCardSpacing());

            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDeckManager>().AddCardToDeck(packCards[cardIndex]);
        }
        TutorialManager.instance.UpdateTrackedSeedShopAction();

        SetOpenView(packScreenObject);
    }

    public void ClickBoosterBuyButton()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoneyManager>().DeductMoney(GameManager.instance.GetBoosterPackPrice());

        GameManager.instance.SetPlayerViewState(PlayerViewState.OpenPack);

        GameObject packScreenObject = Instantiate(
            Resources.Load<GameObject>("Prefabs/Views/PackOpenPrefab"),
            GameObject.FindGameObjectWithTag("Canvas").transform
        );

        GameObject packScreenCloseButtonObject = packScreenObject.transform.Find("CloseButtonObject").Find("Button").gameObject;
        packScreenCloseButtonObject.GetComponent<Button>().onClick.AddListener(ClickPackCloseButton);

        Card[] packCards = GameManager.instance.GetCardsForPack(PackType.Booster);
        float xPos = ((float)GameManager.instance.GetCardWidth() * ((float)packCards.Length - 1f) * -1f) - ((float)GameManager.instance.GetCardSpacing() * ((float)(packCards.Length - 1) / 2f));
        for (int cardIndex = 0; cardIndex < packCards.Length; cardIndex++)
        {
            GameObject newCardObject = Instantiate(
                Resources.Load<GameObject>("Prefabs/Views/CardPrefab"),
                packScreenObject.transform
            );
            newCardObject.transform.localPosition = new Vector3(xPos, 0, 0);

            newCardObject.name = "Card_" + cardIndex;

            CardComponent newCard = newCardObject.GetComponent<CardComponent>();
            newCard.SetCard(packCards[cardIndex]);

            xPos += ((GameManager.instance.GetCardWidth() * 2) + GameManager.instance.GetCardSpacing());

            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDeckManager>().AddCardToDeck(packCards[cardIndex]);
        }
        TutorialManager.instance.UpdateTrackedBoosterShopAction();

        SetOpenView(packScreenObject);
    }

    public void ClickPackCloseButton()
    {
        OpenShopView();
    }

    public void DisplayCardInfo(Card card)
    {
        if (!PlayerViewState.Deck.Equals(GameManager.instance.GetPlayerViewState())) { return; }

        GameObject spriteObject = GetOpenView().transform.Find("CardInfoBackground").Find("Sprite").gameObject;
        spriteObject.GetComponent<RawImage>().texture = card.GetCardTexture();

        GameObject descriptionObject = GetOpenView().transform.Find("CardInfoBackground").Find("Text").gameObject;
        descriptionObject.GetComponent<Text>().text = card.GetCardDescription();

        displayCard = card;
    }

    public void OpenStartMenuView()
    {
        GameManager.instance.SetPlayerViewState(PlayerViewState.StartMenu);
        GameObject menuObject = Instantiate(
            Resources.Load<GameObject>("Prefabs/Views/StartMenuPrefab"),
            GameObject.FindGameObjectWithTag("Canvas").transform
        );
        menuObject.name = "StartMenuPrefab";

        menuObject.transform.Find("StartButtonObject").Find("Button").gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            GameManager.instance.StartButtonClicked();
        });

        menuObject.transform.Find("OptionsButtonObject").Find("Button").gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            OpenOptionsView();
        });

        SetOpenView(menuObject);
    }

    public void OpenOptionsView()
    {
        GameObject optionsObject = Instantiate(
            Resources.Load<GameObject>("Prefabs/Views/OptionsPrefab"),
            GameObject.FindGameObjectWithTag("Canvas").transform
        );
        optionsObject.name = "OptionsPrefab";
        SetOpenView(optionsObject);

        GameManager.instance.SetPlayerViewState(PlayerViewState.Options);

        // Look sensitivity.
        Text lookSensitivityDisplayText = optionsObject.transform.Find("OptionsSection").Find("Background").Find("LookSensitivitySection").Find("Group").Find("DisplayText").gameObject.GetComponent<Text>();
        Slider lookSensitivitySlider = optionsObject.transform.Find("OptionsSection").Find("Background").Find("LookSensitivitySection").Find("Group").Find("Slider").gameObject.GetComponent<Slider>();
        lookSensitivitySlider.value = GameManager.instance.GetPlayerLookSensitivity() / 2f;
        lookSensitivityDisplayText.text = Mathf.RoundToInt(lookSensitivitySlider.value * 100f).ToString();
        lookSensitivitySlider.onValueChanged.AddListener(delegate
        {
            GameManager.instance.SetPlayerLookSensitivity(lookSensitivitySlider.value * 2f);
            lookSensitivityDisplayText.text = Mathf.RoundToInt(lookSensitivitySlider.value * 100f).ToString();
        });

        // Main volume.
        Text mainVolumeDisplayText = optionsObject.transform.Find("OptionsSection").Find("Background").Find("MainVolumeSection").Find("Group").Find("DisplayText").gameObject.GetComponent<Text>();
        Slider mainVolumeSlider = optionsObject.transform.Find("OptionsSection").Find("Background").Find("MainVolumeSection").Find("Group").Find("Slider").gameObject.GetComponent<Slider>();
        mainVolumeSlider.value = GameManager.instance.GetGameMainVolume();
        mainVolumeDisplayText.text = Mathf.RoundToInt(mainVolumeSlider.value * 100f).ToString();
        mainVolumeSlider.onValueChanged.AddListener(delegate
        {
            GameManager.instance.SetGameMainVolume(mainVolumeSlider.value);
            mainVolumeDisplayText.text = Mathf.RoundToInt(mainVolumeSlider.value * 100f).ToString();
        });

        // Music volume.
        Text musicVolumeDisplayText = optionsObject.transform.Find("OptionsSection").Find("Background").Find("MusicVolumeSection").Find("Group").Find("DisplayText").gameObject.GetComponent<Text>();
        Slider musicVolumeSlider = optionsObject.transform.Find("OptionsSection").Find("Background").Find("MusicVolumeSection").Find("Group").Find("Slider").gameObject.GetComponent<Slider>();
        musicVolumeSlider.value = GameManager.instance.GetGameMusicVolume();
        musicVolumeDisplayText.text = Mathf.RoundToInt(musicVolumeSlider.value * 100f).ToString();
        musicVolumeSlider.onValueChanged.AddListener(delegate
        {
            GameManager.instance.SetGameMusicVolume(musicVolumeSlider.value);
            musicVolumeDisplayText.text = Mathf.RoundToInt(musicVolumeSlider.value * 100f).ToString();
        });

        // SFX volume.
        Text sfxVolumeDisplayText = optionsObject.transform.Find("OptionsSection").Find("Background").Find("SFXVolumeSection").Find("Group").Find("DisplayText").gameObject.GetComponent<Text>();
        Slider sfxVolumeSlider = optionsObject.transform.Find("OptionsSection").Find("Background").Find("SFXVolumeSection").Find("Group").Find("Slider").gameObject.GetComponent<Slider>();
        sfxVolumeSlider.value = GameManager.instance.GetGameSfxVolume();
        sfxVolumeDisplayText.text = Mathf.RoundToInt(sfxVolumeSlider.value * 100f).ToString();
        sfxVolumeSlider.onValueChanged.AddListener(delegate
        {
            GameManager.instance.SetGameSfxVolume(sfxVolumeSlider.value);
            sfxVolumeDisplayText.text = Mathf.RoundToInt(sfxVolumeSlider.value * 100f).ToString();
        });

        // Back button.
        optionsObject.transform.Find("OptionsSection").Find("Background").Find("ButtonSection").Find("Group").Find("BackButton").gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (PlayerViewState.StartMenu.Equals(GameManager.instance.GetPreviousPlayerViewState()))
            {
                // Open start menu.
                OpenStartMenuView();
            }
            if (PlayerViewState.Game.Equals(GameManager.instance.GetPreviousPlayerViewState()))
            {
                // Return to game.
                GameManager.instance.SetPlayerViewState(PlayerViewState.Game);
                DestroyOpenView();
            }
        });

        // Quit button.
    }

    public void OpenDialogueView(DialogueStep newDialogueStep)
    {
        GameManager.instance.SetPlayerViewState(PlayerViewState.Dialogue);
        
        GameObject dialogueObject = Instantiate(
            Resources.Load<GameObject>("Prefabs/Views/DialoguePrefab"),
            GameObject.FindGameObjectWithTag("Canvas").transform
        );
        dialogueObject.name = "DialoguePrefab";

        dialogueObject.transform.Find("Background").Find("Text").gameObject.GetComponent<Text>().text = "";
        dialogueStep = newDialogueStep;

        if (dialogueStep.IsItalicized()) { dialogueObject.transform.Find("Background").Find("Text").gameObject.GetComponent<Text>().fontStyle = FontStyle.BoldAndItalic; }

        dialogueObject.transform.Find("Background").Find("ProceedButton").gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            DialogueManager.instance.ProgressDialogue();
        });

        SetOpenView(dialogueObject);
    }
}