using UnityEngine;

public class PlayerInteractionController : MonoBehaviour
{
    private Tile highlightedTile;

    void Start()
    {

    }

    void Update()
    {
        ProcessPlayerViewSelection();
        ProcessPlayerClick();
    }

    private void ProcessPlayerViewSelection()
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

    private void ProcessPlayerClick()
    {
        if (!InputManager.instance.GetMouseLeftClickPress()) { return; }
        if (highlightedTile == null) { return; }

        highlightedTile.TillTile();
    }
}