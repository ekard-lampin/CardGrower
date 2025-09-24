using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    void Awake() { instance = this; }

    [Header("Map Settings")]
    [SerializeField]
    private int mapBaseWidth;
    public int GetMapBaseWidth() { return mapBaseWidth; } public void SetMapBaseWidth(int mapBaseWidth) { this.mapBaseWidth = mapBaseWidth; }

    [SerializeField]
    private float mapOvergrowthLocationVariation;
    public float GetMapOvergrowthLocationVariation() { return mapOvergrowthLocationVariation; } public void SetMapOvergrowthLocationVariation(float mapOvergrowthLocationVariation) { this.mapOvergrowthLocationVariation = mapOvergrowthLocationVariation; }

    [SerializeField]
    private float mapOvergrowthMaxHeight;
    public float GetMapOvergrowthMaxHeight() { return mapOvergrowthMaxHeight; } public void SetMapOvergrowthMaxHeight(float mapOvergrowthMaxHeight) { this.mapOvergrowthMaxHeight = mapOvergrowthMaxHeight; }

    [SerializeField]
    private float mapOvergrowthScaleVariation;
    public float GetMapOvergrowthScaleVariation() { return mapOvergrowthScaleVariation; } public void SetMapOvergrowthScaleVariation(float mapOvergrowthScaleVariation) { this.mapOvergrowthScaleVariation = mapOvergrowthScaleVariation; }

    [Header("Player Settings")]
    [SerializeField]
    private float playerHeightMinimum;
    public float GetPlayerHeightMinimum() { return playerHeightMinimum; } public void SetPlayerHeightMinimum(float playerHeightMinimum) { this.playerHeightMinimum = playerHeightMinimum; }

    [SerializeField]
    private float playerLooksSensitivity;
    public float GetPlayerLookSensitivity() { return playerLooksSensitivity; } public void SetPlayerLookSensitivity(float playerLooksSensitivity) { this.playerLooksSensitivity = playerLooksSensitivity; }

    [SerializeField]
    private float playerMoveSpeed;
    public float GetPlayerMoveSpeed() { return playerMoveSpeed; } public void SetPlayerMoveSpeed(float playerMoveSpeed) { this.playerMoveSpeed = playerMoveSpeed; }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        MapManager.instance.CreateBaseMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
