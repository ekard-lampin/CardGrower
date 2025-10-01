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
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDeckManager>().SetSelectedCard(null);
        }

        if (openView != null && !openView.name.Equals("ShopViewPrefab")) { sellStartIndex = 0; }
        if (sellStartIndex > 0 && sellStartIndex >= GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDeckManager>().GetDeck().Count) { sellStartIndex -= GameManager.instance.GetCardSellCount(); }
    }

    [SerializeField]
    private int deckViewStartIndex = 0;

    [SerializeField]
    private Card displayCard = null;

    [SerializeField]
    private int sellStartIndex = 0;

    public void ToggleDeckView()
    {
        if (PlayerViewState.Game.Equals(GameManager.instance.GetPlayerViewState()))
        {
            OpenDeckView();
        }
        else if (PlayerViewState.Deck.Equals(GameManager.instance.GetPlayerViewState()))
        {
            GameManager.instance.SetPlayerViewState(PlayerViewState.Game);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

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

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDeckManager>().SetSelectedCard(displayCard);

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
}