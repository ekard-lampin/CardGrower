using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;
    void Awake() { instance = this; }

    private GameObject highlightObject;
    public GameObject GetHighlightObject()
    {
        if (highlightObject == null)
        {
            highlightObject = Instantiate(
                Resources.Load<GameObject>("Prefabs/Map/HighlightPrefab"),
                Vector3.zero,
                Quaternion.identity
            );
        }
        return highlightObject;
    }
    public void DestroyHighlightObject() { if (highlightObject != null) { Destroy(highlightObject); } }

    private HashSet<Vector2> farmTiles = new HashSet<Vector2>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreatePlayer()
    {
        if (GameObject.FindGameObjectWithTag("Player") == null)
        {
            GameObject playerObject = Instantiate(
                Resources.Load<GameObject>("Prefabs/PlayerPrefab"),
                new Vector3(0, 10, 0),
                Quaternion.identity
            );
            playerObject.name = "PlayerObject";
            Camera.main.transform.SetParent(playerObject.transform.Find("CameraObject"));
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
    }

    public void CreateBaseMap()
    {   
        Vector2 coordsOffset = new Vector2(
            (float)GameManager.instance.GetMapBaseWidth() / 2f * -1f,
            (float)GameManager.instance.GetMapBaseWidth() / 2f
        );

        // Create farm tiles.
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
                newTile.SetTileCoords(new Vector2(xIndex, zIndex));
                newTileObject.transform.SetParent(GameObject.FindGameObjectWithTag("FarmTiles").transform);
                farmTiles.Add(new Vector2(xIndex + coordsOffset.x, (zIndex * -1) + coordsOffset.y));

                // Create wall tile if on an edge piece.
                if ((xIndex == 0 || xIndex == GameManager.instance.GetMapBaseWidth() - 1) && (zIndex == 0 || zIndex == GameManager.instance.GetMapBaseWidth() - 1))
                {
                    GameObject newWallObject = Instantiate(
                        Resources.Load<GameObject>("Prefabs/Map/Wall_L_Prefab"),
                        new Vector3(xIndex + coordsOffset.x, 0, (zIndex * -1) + coordsOffset.y),
                        Quaternion.identity
                    );
                    newWallObject.name = "Wall_(" + xIndex + ", " + zIndex + ")";
                    newWallObject.transform.SetParent(GameObject.FindGameObjectWithTag("FarmTiles").transform);
                    PlacementWall newWall = newWallObject.AddComponent<PlacementWall>();
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
                    newTile.SetPlacement(newWall);
                }
                else if ((xIndex == 0 || xIndex == GameManager.instance.GetMapBaseWidth() - 1) || (zIndex == 0 || zIndex == GameManager.instance.GetMapBaseWidth() - 1))
                {
                    GameObject newWallObject = Instantiate(
                        Resources.Load<GameObject>("Prefabs/Map/Wall_Straight_Prefab"),
                        new Vector3(xIndex + coordsOffset.x, 0, (zIndex * -1) + coordsOffset.y),
                        Quaternion.identity
                    );
                    newWallObject.name = "Wall_(" + xIndex + ", " + zIndex + ")";
                    newWallObject.transform.SetParent(GameObject.FindGameObjectWithTag("FarmTiles").transform);
                    PlacementWall newWall = newWallObject.AddComponent<PlacementWall>();
                    if (zIndex == 0 || zIndex == GameManager.instance.GetMapBaseWidth() - 1)
                    {
                        newWallObject.transform.rotation = Quaternion.Euler(0, 90, 0);
                    }
                    newTile.SetPlacement(newWall);
                }
                else
                {
                    // Create an overgrowth object.
                    GameObject newOvergrowthObject = Instantiate(
                        Resources.Load<GameObject>("Prefabs/Map/OvergrowthPrefab"),
                        new Vector3(xIndex + coordsOffset.x, 0, (zIndex * -1) + coordsOffset.y),
                        Quaternion.identity
                    );
                    newOvergrowthObject.name = "Overgrowth_(" + xIndex + ", " + zIndex + ")";
                    newOvergrowthObject.transform.SetParent(GameObject.FindGameObjectWithTag("FarmTiles").transform);
                    PlacementOvergrowth newOvergrowth = newOvergrowthObject.AddComponent<PlacementOvergrowth>();
                    newTile.SetPlacement(newOvergrowth);

                    float verticalScale = Random.Range(0.25f, GameManager.instance.GetMapOvergrowthMaxHeight());
                    float horizontalScale = Random.Range(GameManager.instance.GetMapOvergrowthScaleVariation() * -1, GameManager.instance.GetMapOvergrowthScaleVariation());
                    newOvergrowthObject.transform.localScale = new Vector3(1 + horizontalScale, verticalScale, 1 + horizontalScale);

                    float variationX = Random.Range(GameManager.instance.GetMapOvergrowthLocationVariation() * -1, GameManager.instance.GetMapOvergrowthLocationVariation());
                    float variationZ = Random.Range(GameManager.instance.GetMapOvergrowthLocationVariation() * -1, GameManager.instance.GetMapOvergrowthLocationVariation());
                    newOvergrowthObject.transform.position = new Vector3(newOvergrowthObject.transform.position.x + variationX, newOvergrowthObject.transform.position.y, newOvergrowthObject.transform.position.z + variationZ);

                    float rotationVariation = Random.Range(-360, 360);
                    newOvergrowthObject.transform.rotation = Quaternion.Euler(0, rotationVariation, 0);
                }
            }
        }

        // Create filler tiles.
        Vector2 fillerOffset = new Vector2(((float)GameManager.instance.GetMapBaseWidth() + GameManager.instance.GetMapTreeRingOffset() + GameManager.instance.GetMapFillerRingOffset()) / 2f * -1, ((float)GameManager.instance.GetMapBaseWidth() + GameManager.instance.GetMapTreeRingOffset() + GameManager.instance.GetMapFillerRingOffset()) / 2f);
        float totalWidth = GameManager.instance.GetMapBaseWidth() + GameManager.instance.GetMapTreeRingOffset() + GameManager.instance.GetMapFillerRingOffset();
        for (int xIndex = 0; xIndex < GameManager.instance.GetMapBaseWidth() + GameManager.instance.GetMapTreeRingOffset() + GameManager.instance.GetMapFillerRingOffset(); xIndex++)
        {
            for (int zIndex = 0; zIndex < GameManager.instance.GetMapBaseWidth() + GameManager.instance.GetMapTreeRingOffset() + GameManager.instance.GetMapFillerRingOffset(); zIndex++)
            {
                // Skip farm tile spaces.
                if (farmTiles.Contains(new Vector2(xIndex + fillerOffset.x, (zIndex * -1) + fillerOffset.y))) { continue; }

                if (((xIndex < totalWidth / 4) || (xIndex > totalWidth - (totalWidth / 4)))
                    || ((zIndex < totalWidth / 4) || (zIndex > totalWidth - (totalWidth / 4)))
                )
                {
                    // if (((xIndex + zIndex) % 3 == 2) || ((xIndex == 0 || xIndex == totalWidth - 1) || (zIndex == 0 || zIndex == totalWidth - 1)))
                    if (Random.Range(0, 7) == 0 || ((xIndex == 0 || xIndex == totalWidth - 1) || (zIndex == 0 || zIndex == totalWidth - 1)))
                    {
                        GameObject newTreeObject = Instantiate(
                            Resources.Load<GameObject>("Prefabs/Map/TreePrefab"),
                            new Vector3(xIndex + fillerOffset.x, 0, (zIndex * -1) + fillerOffset.y),
                            Quaternion.identity
                        );
                        float scale = Random.Range(1 - GameManager.instance.GetMapTreeScaleVariance(), 1 + GameManager.instance.GetMapTreeScaleVariance());
                        newTreeObject.transform.localScale = new Vector3(1, scale, 1);
                        newTreeObject.transform.localRotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
                        newTreeObject.transform.SetParent(GameObject.FindGameObjectWithTag("Trees").transform);
                    }
                }

                // Place filler tile down.
                GameObject newTileObject = Instantiate(
                    Resources.Load<GameObject>("Prefabs/Map/ExteriorMapTilePrefab"),
                    new Vector3(xIndex + fillerOffset.x, 0, (zIndex * -1) + fillerOffset.y),
                    Quaternion.identity
                );
                newTileObject.transform.SetParent(GameObject.FindGameObjectWithTag("FillerTiles").transform);
            }
        }
    }
}
