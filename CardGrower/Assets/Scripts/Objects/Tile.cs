using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private Vector2 tileCoords = Vector2.zero;
    public Vector2 GetTileCoords() { return tileCoords; }
    public void SetTileCoords(Vector2 tileCoords) { this.tileCoords = tileCoords; }

    [SerializeField]
    Placement placement = null;
    public Placement GetPlacement() { return placement; }
    public void SetPlacement(Placement placement) { this.placement = placement; }

    private TileState tileState = TileState.Overgrown;
    public TileState GetTileState() { return tileState; } public void SetTileState(TileState tileState) { this.tileState = tileState; }

    public void TillTile()
    {
        if (!TileState.Overgrown.Equals(tileState)) { return; }
        if (placement is PlacementWall) { return; }

        tileState = TileState.Farmland;
        transform.Find("Mesh").GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Farmland");

        Destroy(placement.gameObject);
        placement = null;
    }
}

public enum TileState
{
    Overgrown,
    Farmland
}