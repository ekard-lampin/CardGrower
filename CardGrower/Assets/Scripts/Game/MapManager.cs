using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    void Awake() { instance = this; }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateBaseMap()
    {
        Vector2 coordsOffset = new Vector2(
            (float)GameManager.instance.GetMapBaseWidth() / 2f * -1f,
            (float)GameManager.instance.GetMapBaseWidth() / 2
        );

        for (int xIndex = 0; xIndex < GameManager.instance.GetMapBaseWidth(); xIndex++)
        {
            for (int zIndex = 0; zIndex < GameManager.instance.GetMapBaseWidth(); zIndex++)
            {
                // Create base tile.
                GameObject newTileObject = Instantiate(
                    Resources.Load<GameObject>("Prefabs/Map/MapTilePrefab"),
                    new Vector3(xIndex + coordsOffset.x, 0, (zIndex * -1) + coordsOffset.y),
                    Quaternion.identity
                );
                newTileObject.name = "Tile_(" + xIndex + ", " + zIndex + ")";
                Tile newTile = newTileObject.AddComponent<Tile>();

                // Create wall tile if on an edge piece.
                if ((xIndex == 0 || xIndex == GameManager.instance.GetMapBaseWidth() - 1) && (zIndex == 0 || zIndex == GameManager.instance.GetMapBaseWidth() - 1))
                {
                    GameObject newWallObject = Instantiate(
                        Resources.Load<GameObject>("Prefabs/Map/Wall_L_Prefab"),
                        new Vector3(xIndex + coordsOffset.x, 0, (zIndex * -1) + coordsOffset.y),
                        Quaternion.identity
                    );
                    newWallObject.name = "Wall_(" + xIndex + ", " + zIndex + ")";
                    // Top left
                    if (xIndex == 0 && zIndex == 0)
                    {
                        // Do nothing
                    }
                    // Bottom left
                    if (xIndex == 0 && zIndex == GameManager.instance.GetMapBaseWidth() - 1)
                    {
                        newWallObject.transform.rotation = Quaternion.Euler(0, -90, 0);
                    }
                    // Top right
                    if (xIndex == GameManager.instance.GetMapBaseWidth() - 1 && zIndex == 0)
                    {
                        newWallObject.transform.rotation = Quaternion.Euler(0, 90, 0);
                    }
                    // Bottom right
                    if (xIndex == GameManager.instance.GetMapBaseWidth() - 1 && zIndex == GameManager.instance.GetMapBaseWidth() - 1)
                    {
                        newWallObject.transform.rotation = Quaternion.Euler(0, 180, 0);
                    }
                    newTile.SetPlacement(newWallObject);
                }
                else if ((xIndex == 0 || xIndex == GameManager.instance.GetMapBaseWidth() - 1) || (zIndex == 0 || zIndex == GameManager.instance.GetMapBaseWidth() - 1))
                {
                    GameObject newWallObject = Instantiate(
                        Resources.Load<GameObject>("Prefabs/Map/Wall_Straight_Prefab"),
                        new Vector3(xIndex + coordsOffset.x, 0, (zIndex * -1) + coordsOffset.y),
                        Quaternion.identity
                    );
                    newWallObject.name = "Wall_(" + xIndex + ", " + zIndex + ")";
                    if (zIndex == 0 || zIndex == GameManager.instance.GetMapBaseWidth() - 1)
                    {
                        newWallObject.transform.rotation = Quaternion.Euler(0, 90, 0);
                    }
                    newTile.SetPlacement(newWallObject);
                }
            }
        }
    }
}
