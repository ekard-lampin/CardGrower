using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInteractionController : MonoBehaviour, IPointerDownHandler
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

    private void ProcessDeckViewClick()
    {
        
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
        if (PlayerViewState.Game.Equals(GameManager.instance.GetPlayerViewState()) && playerDeckManager.GetSelectedCard() != null)
        {
            if (!InputManager.instance.GetMouseLeftClickPress()) { return; }
            if (highlightedTile == null) { return; }

            // Tilling
            if (TileState.Overgrown.Equals(highlightedTile.GetTileState()) && CardId.Hoe.Equals(playerDeckManager.GetSelectedCard().GetCardId()))
            {
                highlightedTile.TillTile();
                playerDeckManager.RemoveCardFromDeck(playerDeckManager.GetSelectedCard());
                ViewManager.instance.SetOpenView(null);
            }
        }
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

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        Debug.Log(pointerEventData.selectedObject.name + " clicked.");
        ProcessDeckViewClick();
    }
}