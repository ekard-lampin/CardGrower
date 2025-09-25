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
        }

        SetOpenView(packScreenObject);
    }

    public void ClickPackCloseButton()
    {
        Debug.Log("Closing pack screen.");
        OpenShopView();
    }
}