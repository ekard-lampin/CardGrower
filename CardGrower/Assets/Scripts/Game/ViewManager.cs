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
    }

    [SerializeField]
    private int deckViewStartIndex = 0;

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
        RenderCards(deckViewStartIndex);

        // Set click actions for buttons.
        GameObject deckBackgroundObject = deckViewObject.transform.Find("DeckBackground").gameObject;
        GameObject backButtonObject = deckBackgroundObject.transform.Find("BackButtonObject").Find("Button").gameObject;
        backButtonObject.GetComponent<Button>().onClick.AddListener(PreviousDeckPage);

        GameObject nextButtonObject = deckBackgroundObject.transform.Find("ForwardButtonObject").Find("Button").gameObject;
        nextButtonObject.GetComponent<Button>().onClick.AddListener(NextDeckPage);

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

                CardComponent newCard = newCardObject.AddComponent<CardComponent>();
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

    private void OpenShopView()
    {
        GameManager.instance.SetPlayerViewState(PlayerViewState.Shop);

        GameObject shopObject = Instantiate(
            Resources.Load<GameObject>("Prefabs/Views/ShopViewPrefab"),
            GameObject.FindGameObjectWithTag("Canvas").transform
        );

        GameObject toolBuyButtonObject = shopObject.transform.Find("ToolPackButtonObject").Find("Button").gameObject;
        toolBuyButtonObject.GetComponent<Button>().onClick.AddListener(ClickToolBuyButton);

        GameObject seedBuyButtonObject = shopObject.transform.Find("SeedPackButtonObject").Find("Button").gameObject;
        seedBuyButtonObject.GetComponent<Button>().onClick.AddListener(ClickSeedBuyButton);

        GameObject boosterPackButtonObject = shopObject.transform.Find("BoosterPackButtonObject").Find("Button").gameObject;
        boosterPackButtonObject.GetComponent<Button>().onClick.AddListener(ClickBoosterBuyButton);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SetOpenView(shopObject);
    }

    public void ClickToolBuyButton()
    {
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

            CardComponent newCard = newCardObject.AddComponent<CardComponent>();
            newCard.SetCard(packCards[cardIndex]);

            xPos += ((GameManager.instance.GetCardWidth() * 2) + GameManager.instance.GetCardSpacing());

            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDeckManager>().AddCardToDeck(packCards[cardIndex]);
        }

        SetOpenView(packScreenObject);
    }

    public void ClickSeedBuyButton()
    {
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

            CardComponent newCard = newCardObject.AddComponent<CardComponent>();
            newCard.SetCard(packCards[cardIndex]);

            xPos += ((GameManager.instance.GetCardWidth() * 2) + GameManager.instance.GetCardSpacing());

            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerDeckManager>().AddCardToDeck(packCards[cardIndex]);
        }

        SetOpenView(packScreenObject);
    }

    public void ClickBoosterBuyButton()
    {
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

            CardComponent newCard = newCardObject.AddComponent<CardComponent>();
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
    }
}