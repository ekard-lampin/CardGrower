using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInteractionController : MonoBehaviour
{
    private Tile highlightedTile;

    void Start()
    {

    }

    void Update()
    {
        ProcessPlayerShopInput();
        ProcessPlayerDeckInput();

        ProcessPlayerViewSelection();
        ProcessPlayerClick();
    }

    private void ProcessPlayerViewSelection()
    {
        if (PlayerViewState.Game.Equals(GameManager.instance.GetPlayerViewState()))
        {
            if (Physics.Raycast(transform.position, Camera.main.transform.forward * 100, out RaycastHit hit))
            {
                GameObject hitObject = hit.collider.gameObject;

                // If the hit object has a tile, do tile stuff.
                if (hitObject.GetComponent<Tile>() != null)
                {
                    GameObject highlightObject = MapManager.instance.GetHighlightObject();
                    highlightObject.transform.position = hitObject.transform.position;

                    highlightedTile = hitObject.GetComponent<Tile>();
                }
            }
            else
            {
                highlightedTile = null;
                MapManager.instance.DestroyHighlightObject();
            }
        }
    }

    private void ProcessPlayerClick()
    {
        PlayerDeckManager playerDeckManager = GetComponent<PlayerDeckManager>();
        if (PlayerViewState.Game.Equals(GameManager.instance.GetPlayerViewState()))
        {
            if (!InputManager.instance.GetMouseLeftClickPress()) { return; }
            if (highlightedTile == null) { return; }

            if (playerDeckManager.GetSelectedCard() != null && IsTilling(highlightedTile, playerDeckManager.GetSelectedCard().GetCardId()))
            { // Tilling
                Debug.Log("Tilling");
                highlightedTile.TillTile();
                playerDeckManager.RemoveCardFromDeck(playerDeckManager.GetSelectedCard());
                ViewManager.instance.SetOpenView(null);
            }
            else if (playerDeckManager.GetSelectedCard() != null && IsPlanting(highlightedTile.GetTileState(), playerDeckManager.GetSelectedCard().GetCardType()))
            { // Planting
                Debug.Log("Planting");
                highlightedTile.PlantSeed(playerDeckManager.GetSelectedCard());
                playerDeckManager.RemoveCardFromDeck(playerDeckManager.GetSelectedCard());
                ViewManager.instance.SetOpenView(null);
            }
            else if (IsHarvesting(highlightedTile, playerDeckManager.GetSelectedCard()))
            {// Harvesting
                Debug.Log("Harvesting");
                Card plantedSeed = ((PlacementPlant)highlightedTile.GetPlacement()).GetPlantedSeed();
                highlightedTile.HarvestTile();
                GenerateCrops(plantedSeed);
            }
        }
    }

    private bool IsTilling(Tile tile, CardId cardId) {
        bool isTilling = true;

        if (tile.GetPlacement() is PlacementWall) { isTilling = false; }
        if (!TileState.Overgrown.Equals(tile.GetTileState())) { isTilling = false; }
        if (!CardId.Hoe.Equals(cardId)) { isTilling = false; }

        return isTilling;
    }

    private bool IsPlanting(TileState tileState, CardType cardType) {
        bool isPlanting = true;

        if (!TileState.Farmland.Equals(tileState)) { isPlanting = false; }
        if (!CardType.Seed.Equals(cardType)) { isPlanting = false; }

        return isPlanting;
    }

    private bool IsHarvesting(Tile tile, Card selectedCard)
    {
        bool isHarvesting = true;

        // Debug.Log("Tile is not planted: " + (!TileState.Planted.Equals(tile.GetTileState())));
        if (!TileState.Planted.Equals(tile.GetTileState())) { isHarvesting = false; }
        // Debug.Log("Tile does not have placement: " + (tile.GetPlacement() == null));
        if (tile.GetPlacement() == null) { isHarvesting = false; }
        // Debug.Log("Tile placement is not a plant: " + (isHarvesting && !(tile.GetPlacement() is PlacementPlant)));
        if (isHarvesting && !(tile.GetPlacement() is PlacementPlant)) { isHarvesting = false; }
        // Debug.Log("Tile placement has not finished growing: " + (isHarvesting && !((PlacementPlant)tile.GetPlacement()).IsFinishedGrowing()));
        if (isHarvesting && !((PlacementPlant)tile.GetPlacement()).IsFinishedGrowing()) { isHarvesting = false; }
        // Debug.Log("Player is holding card: " + (!selectedCard.GetCardId().Equals(CardId.None)));
        if (selectedCard == null || !selectedCard.GetCardId().Equals(CardId.None)) { isHarvesting = false; }
        // TODO: Implement tools that will improve yield or quality.

        return isHarvesting;
    }

    private void GenerateCrops(Card seedCard)
    {
        Card[] newCropCards = GameManager.instance.GenerateCardsForSeed(seedCard);
        foreach (Card newCropCard in newCropCards) { GetComponent<PlayerDeckManager>().AddCardToDeck(newCropCard); }
    }

    private void ProcessPlayerDeckInput()
    {
        if (!InputManager.instance.GetFPress()) { return; }

        ViewManager.instance.ToggleDeckView();
    }

    private void ProcessPlayerShopInput()
    {
        if (!InputManager.instance.GetTabPress()) { return; }

        ViewManager.instance.ToggleShopView();
    }
}