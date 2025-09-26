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
        if (PlayerViewState.Game.Equals(GameManager.instance.GetPlayerViewState()))
        {
            if (!InputManager.instance.GetMouseLeftClickPress()) { return; }
            if (highlightedTile == null) { return; }

            highlightedTile.TillTile();
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