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

    [SerializeField]
    private TileState tileState = TileState.Overgrown;
    public TileState GetTileState() { return tileState; }
    public void SetTileState(TileState tileState) { this.tileState = tileState; }

    public void TillTile()
    {
        if (!TileState.Overgrown.Equals(tileState)) { return; }
        if (placement == null) { return; }
        if (placement is PlacementWall) { return; }

        tileState = TileState.Farmland;
        transform.Find("Mesh").GetComponent<MeshRenderer>().material = Resources.Load<Material>("Materials/Farmland");

        Destroy(placement.gameObject);
        placement = null;

        Destroy(
            Instantiate(
                Resources.Load<GameObject>("Prefabs/Map/HoeSfxPrefab"),
                transform.position,
                Quaternion.identity
            ),
            1
        );
    }

    public void PlantSeed(Card card)
    {
        if (placement != null) { Destroy(placement.gameObject); }

        GameObject newPlantObject = Instantiate(
            Resources.Load<GameObject>("Prefabs/Map/PlantPrefab"),
            transform.position,
            Quaternion.identity
        );
        PlacementPlant newPlant = newPlantObject.AddComponent<PlacementPlant>();
        newPlant.SetPlantedSeed(card);

        placement = newPlant;

        tileState = TileState.Planted;

        Destroy(
            Instantiate(
                Resources.Load<GameObject>("Prefabs/Map/SeedSfxPrefab"),
                transform.position,
                Quaternion.identity
            ),
            1
        );
    }

    public void HarvestTile()
    {
        if (placement == null) { return; }
        if (!(placement is PlacementPlant)) { return; }

        Destroy(placement.gameObject);
        placement = null;

        tileState = TileState.Farmland;
    }
}