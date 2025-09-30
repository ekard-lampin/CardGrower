using UnityEngine;
using UnityEngine.UI;

public class CardComponent : MonoBehaviour
{
    [SerializeField]
    private Card card;
    public Card GetCard() { return card; }
    public void SetCard(Card card)
    {
        this.card = card;
        InitializeCard();
    }

    private void InitializeCard()
    {
        GameObject imageCardObject = transform.Find("Image_Card").gameObject;
        imageCardObject.GetComponent<RawImage>().texture = card.GetCardTexture();

        GameObject imageQualityObject = transform.Find("Image_Quality").gameObject;
        imageQualityObject.GetComponent<RawImage>().texture = Resources.Load<Texture>("Textures/quality_" + card.GetCardQuality().ToString().ToLower());
        // Remove the quality indicator from non-crop cards.
        if (!CardType.Crop.Equals(GetCard().GetCardType())) { imageQualityObject.SetActive(false); }

        GameObject imageRarityObject = transform.Find("Image_Rarity").gameObject;
        imageRarityObject.GetComponent<RawImage>().texture = Resources.Load<Texture>("Textures/rarity_" + card.GetCardRarity().ToString().ToLower());
        // Remove the rarity indicator from crop cards.
        if (CardType.Crop.Equals(GetCard().GetCardType())) { imageRarityObject.SetActive(false); }

        GameObject hitboxObject = transform.Find("Hitbox").gameObject;
        hitboxObject.GetComponent<CardClickController>().SetCardComponent(this);
        // Disable click if shop is open.
        if (PlayerViewState.Shop.Equals(GameManager.instance.GetPlayerViewState())) { hitboxObject.SetActive(false); }
    }

    public void Click()
    {
        if (PlayerViewState.Deck.Equals(GameManager.instance.GetPlayerViewState())) { ViewManager.instance.DisplayCardInfo(GetCard()); }
    }
}