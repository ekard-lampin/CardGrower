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
        ProcessPlayerOptionsInput();

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

            if (playerDeckManager.GetSelectedCard().Length > 0 && IsTilling(highlightedTile, playerDeckManager.GetSelectedCard()[0].GetCardId()))
            { // Tilling
                Debug.Log("Tilling");
                highlightedTile.TillTile();
                playerDeckManager.RemoveCardFromDeck(playerDeckManager.GetSelectedCard()[0]);
                ViewManager.instance.SetOpenView(null);
                TutorialManager.instance.UpdateTrackedTillAction();
            }
            else if (playerDeckManager.GetSelectedCard().Length > 0 && IsPlanting(highlightedTile.GetTileState(), playerDeckManager.GetSelectedCard()[0].GetCardType()))
            { // Planting
                Debug.Log("Planting");
                highlightedTile.PlantSeed(playerDeckManager.GetSelectedCard()[0]);
                playerDeckManager.RemoveCardFromDeck(playerDeckManager.GetSelectedCard()[0]);
                ViewManager.instance.SetOpenView(null);
                TutorialManager.instance.UpdateTrackedPlantingAction();
            }
            else if (IsHarvesting(highlightedTile, playerDeckManager.GetSelectedCard()))
            {// Harvesting
                Debug.Log("Harvesting");
                GenerateCrops(highlightedTile.GetPlacement() as PlacementPlant, playerDeckManager.GetSelectedCard());
                highlightedTile.HarvestTile();
                TutorialManager.instance.UpdateTrackedHarvestAction();
            }
            else if (playerDeckManager.GetSelectedCard().Length > 0 && IsBoosting(highlightedTile, playerDeckManager.GetSelectedCard()))
            { // Boosting
                BoostCrop(highlightedTile, playerDeckManager.GetSelectedCard());
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

    private bool IsHarvesting(Tile tile, Card[] selectedCard)
    {
        bool isHarvesting = true;

        if (!TileState.Planted.Equals(tile.GetTileState())) { isHarvesting = false; }
        if (tile.GetPlacement() == null) { isHarvesting = false; }
        if (isHarvesting && !(tile.GetPlacement() is PlacementPlant)) { isHarvesting = false; }
        if (isHarvesting && !((PlacementPlant)tile.GetPlacement()).IsFinishedGrowing()) { isHarvesting = false; }
        if (selectedCard.Length > 0 && !CardType.Tool.Equals(selectedCard[0].GetCardType())) { isHarvesting = false; }
        if (selectedCard.Length > 0 && CardId.Hoe.Equals(selectedCard[0].GetCardId())) { isHarvesting = false; }

        return isHarvesting;
    }

    private bool IsBoosting(Tile tile, Card[] selectedCard)
    {
        bool isBoosting = true;

        if (!TileState.Planted.Equals(tile.GetTileState())) { isBoosting = false; }
        if (tile.GetPlacement() == null) { isBoosting = false; }
        if (isBoosting && !(tile.GetPlacement() is PlacementPlant)) { isBoosting = false; }
        if (selectedCard.Length > 0 && !CardType.Booster.Equals(selectedCard[0].GetCardType())) { isBoosting = false; }

        return isBoosting;
    }

    private void GenerateCrops(PlacementPlant plant, Card[] selectedCard)
    {
        Card[] newCropCards = GameManager.instance.GenerateCardsForSeed(plant, selectedCard);
        foreach (Card newCropCard in newCropCards) { GetComponent<PlayerDeckManager>().AddCardToDeck(newCropCard); }
    }

    private void BoostCrop(Tile tile, Card[] selectedCard)
    {
        PlacementPlant plant = tile.GetPlacement() as PlacementPlant;
        if (plant.DoesPlantHaveBooster(selectedCard[0])) { return; }

        // Check for any limitations.
        switch (selectedCard[0].GetCardId())
        {
            case CardId.WaterBucket:
            case CardId.Fertilizer:
            case CardId.GrowthHormone:
                if (plant.GetGrowthStage() == 4) { return; }
                break;
            default:
                break;
        }

        plant.AddBooster(selectedCard[0]);
        GetComponent<PlayerDeckManager>().RemoveCardFromDeck(selectedCard[0]);
        GetComponent<PlayerDeckManager>().SetSelectedCard(new Card[0]);
        ViewManager.instance.SetOpenView(null);
        TutorialManager.instance.UpdateTrackedBoosterUseAction();
    }

    private void ProcessPlayerDeckInput()
    {
        if (!InputManager.instance.GetFPress()) { return; }
        if (!PlayerViewState.Game.Equals(GameManager.instance.GetPlayerViewState()) && !PlayerViewState.Deck.Equals(GameManager.instance.GetPlayerViewState())) { return; }

        ViewManager.instance.ToggleDeckView();
    }

    private void ProcessPlayerShopInput()
    {
        if (!InputManager.instance.GetTabPress()) { return; }
        if (!PlayerViewState.Game.Equals(GameManager.instance.GetPlayerViewState()) && !PlayerViewState.Shop.Equals(GameManager.instance.GetPlayerViewState())) { return; }
        if (!TutorialManager.instance.IsTutorialFlagSet(TutorialState.ToolShop)) { return; }

        ViewManager.instance.ToggleShopView();
    }

    private void ProcessPlayerOptionsInput()
    {
        if (!InputManager.instance.GetVPress()) { return; }

        if (PlayerViewState.Game.Equals(GameManager.instance.GetPlayerViewState())) { ViewManager.instance.OpenOptionsView(); }
        else if (PlayerViewState.Options.Equals(GameManager.instance.GetPlayerViewState()))
        {
            GameManager.instance.SetPlayerViewState(PlayerViewState.Game);
            ViewManager.instance.DestroyOpenView();
        }
    }
}