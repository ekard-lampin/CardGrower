using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    private Vector3 movementInput;
    private GameObject cameraParent;

    private Vector2 playerRotation = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        cameraParent = transform.Find("CameraObject").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        ReadPlayerInput();
        HandlePlayerMouseMovement();
    }

    void FixedUpdate()
    {
        HandlePlayerInput();
    }

    private void HandlePlayerInput()
    {
        if (!PlayerViewState.Game.Equals(GameManager.instance.GetPlayerViewState())) { return; }

        Vector3 newPosition = transform.position + (transform.forward * movementInput.x + transform.up * movementInput.y + transform.right * movementInput.z) * GameManager.instance.GetPlayerMoveSpeed() * Time.deltaTime;
        // if (!IsNewPositionInBounds(newPosition)) { return; }
        newPosition.x = Mathf.Clamp(newPosition.x, ((float)GameManager.instance.GetMapBaseWidth()) / 2f * -1, ((float)GameManager.instance.GetMapBaseWidth()) / 2f);
        newPosition.y = Mathf.Clamp(newPosition.y, GameManager.instance.GetPlayerHeightMinimum(), 10);
        newPosition.z = Mathf.Clamp(newPosition.z, ((float)GameManager.instance.GetMapBaseWidth()) / 2f * -1, ((float)GameManager.instance.GetMapBaseWidth()) / 2f);

        transform.position = newPosition;
    }

    private void HandlePlayerMouseMovement()
    {
        if (!PlayerViewState.Game.Equals(GameManager.instance.GetPlayerViewState())) { return; }

        Vector2 mouseMovement = InputManager.instance.GetMouseMovementInput();
        mouseMovement.y *= -1;
        playerRotation += (mouseMovement * GameManager.instance.GetPlayerLookSensitivity());
        playerRotation.y = Mathf.Clamp(playerRotation.y, -90, 90);

        transform.rotation = Quaternion.Euler(0, playerRotation.x, 0);
        cameraParent.transform.localRotation = Quaternion.Euler(playerRotation.y, 0, 0);
    }

    private void ReadPlayerInput()
    {
        movementInput = InputManager.instance.GetMovementInput();
    }
}
