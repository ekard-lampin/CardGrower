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

    [Header("Player Settings")]
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
