using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager instance;
    void Awake() { instance = this; }

    [SerializeField]
    private OpeningCutsceneState openingCutsceneState = OpeningCutsceneState.None;
    public OpeningCutsceneState GetOpeningCutsceneState() { return openingCutsceneState; }
    public void StartOpeningCutscene() { openingCutsceneState = OpeningCutsceneState.OpeningPan; }

    [Header("Cutscene Variables")]
    [SerializeField]
    private bool cutsceneStateInitialized = false;
    [SerializeField]
    private float cutsceneActionTimer = 0;
    [SerializeField]
    private float cameraTrackSpeed;
    [SerializeField]
    private float cameraZoomRate;

    [Header("Opening Custcene Variables")]
    [SerializeField]
    private float openingPanDuration;
    [SerializeField]
    private Vector3 openingPanCameraStart;
    [SerializeField]
    private Vector3 openingPanCameraTarget;
    [SerializeField]
    private float firstStallDuration;
    [SerializeField]
    private float portalOpenDuration;
    [SerializeField]
    private Vector3 portalLocation;
    [SerializeField]
    private Vector3 portalRotation;
    [SerializeField]
    private float portalRotationRate;
    [SerializeField]
    private float portalGrowthDelay;
    [SerializeField]
    private float portalFocusDelay;
    [SerializeField]
    private float portalZoomDelay;
    [SerializeField]
    private float droneEntryDuration;
    [SerializeField]
    private Vector3 droneRotation;
    [SerializeField]
    private Vector3 droneStartLocation;
    [SerializeField]
    private Vector3 droneTargetLocation;
    [SerializeField]
    private float droneGrowthDelay;
    [SerializeField]
    private Vector3 droneEntryCameraOffset;
    [SerializeField]
    private float droneBobSpeed;
    [SerializeField]
    private float droneWobbleSpeed;
    [SerializeField]
    private float secondStallDuration;

    [Header("GameObject References")]
    [SerializeField]
    private GameObject portalObject;
    [SerializeField]
    private GameObject droneObject;

    private float bobRatio;
    private float zWobbleRatio;
    
    void Update()
    {
        ProcessOpeningCutscene();
    }

    private void ProcessOpeningCutscene()
    {
        // if (!PlayerViewState.OpeningCutscene.Equals(GameManager.instance.GetPlayerViewState())) { return; }
        if (OpeningCutsceneState.None.Equals(openingCutsceneState)) { return; }

        switch (openingCutsceneState)
        {
            case OpeningCutsceneState.OpeningPan:
                if (!cutsceneStateInitialized)
                {
                    cutsceneStateInitialized = true;
                    MapManager.instance.CreateBaseMap();
                    Camera.main.transform.position = openingPanCameraStart;
                    Debug.Log("Starting opening pan.");
                }

                cutsceneActionTimer += Time.deltaTime;

                float ratio = Mathf.SmoothStep(0, 1, Mathf.Clamp(cutsceneActionTimer / openingPanDuration, 0, 1));
                Camera.main.transform.position = Vector3.Lerp(openingPanCameraStart, openingPanCameraTarget, ratio);

                if (cutsceneActionTimer < openingPanDuration) { return; }

                cutsceneActionTimer = 0;
                cutsceneStateInitialized = false;
                openingCutsceneState = OpeningCutsceneState.FirstStall;
                Debug.Log("Opening pan complete.");
                break;
            case OpeningCutsceneState.FirstStall:
                if (!cutsceneStateInitialized)
                {
                    cutsceneStateInitialized = true;
                    Debug.Log("Started: first stall");
                }

                cutsceneActionTimer += Time.deltaTime;

                if (cutsceneActionTimer < firstStallDuration) { return; }

                cutsceneActionTimer = 0;
                cutsceneStateInitialized = false;
                openingCutsceneState = OpeningCutsceneState.FirstDialogue;
                Debug.Log("Completed: first stall");
                break;
            case OpeningCutsceneState.FirstDialogue:
                if (!cutsceneStateInitialized)
                {
                    cutsceneStateInitialized = true;
                    DialogueManager.instance.StartDialogue(DialogueId.OpeningCutsceneFirstDialogue);
                    Debug.Log("Started: first dialogue");
                }

                if (cutsceneActionTimer == 0) { return; }

                cutsceneActionTimer = 0;
                cutsceneStateInitialized = false;
                openingCutsceneState = OpeningCutsceneState.PortalOpen;
                Debug.Log("Completed: first dialogue");
                break;
            case OpeningCutsceneState.PortalOpen:
                if (!cutsceneStateInitialized)
                {
                    cutsceneStateInitialized = true;
                    portalObject = Instantiate(
                        Resources.Load<GameObject>("Prefabs/PortalPrefab"),
                        portalLocation,
                        Quaternion.Euler(portalRotation)
                    );
                    portalObject.transform.localScale = Vector3.zero;
                    Debug.Log("Started: portal open");
                }

                cutsceneActionTimer += Time.deltaTime;

                // Portal rotation.
                PortalRotation();

                // Portal growth.
                if (cutsceneActionTimer > portalGrowthDelay)
                {
                    float growthTimer = Mathf.Clamp(cutsceneActionTimer - portalGrowthDelay, 0, 1);
                    float growthRatio = growthTimer / 1f;
                    portalObject.transform.localScale = Vector3.one * growthRatio;
                }

                // Camera tracking.
                if (cutsceneActionTimer > portalFocusDelay)
                {
                    Vector3 direction = portalObject.transform.position - Camera.main.transform.position;
                    Quaternion lookRotation = Quaternion.LookRotation(direction, Camera.main.transform.up);
                    Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, lookRotation, cameraTrackSpeed * Time.deltaTime);
                }

                // Camera zoom.
                if (cutsceneActionTimer > portalZoomDelay)
                {
                    float zoomTimer = Mathf.SmoothStep(0, 1, Mathf.Clamp(cutsceneActionTimer - portalZoomDelay, 0, 1));
                    float zoomAmount = 20 * zoomTimer / 1f;
                    Camera.main.fieldOfView = 60 - zoomAmount;
                }

                if (cutsceneActionTimer < portalOpenDuration) { return; }

                cutsceneActionTimer = 0;
                cutsceneStateInitialized = false;
                openingCutsceneState = OpeningCutsceneState.DroneEntry;
                Debug.Log("Completed: portal open");
                break;
            case OpeningCutsceneState.DroneEntry:
                if (!cutsceneStateInitialized)
                {
                    cutsceneStateInitialized = true;
                    droneObject = Instantiate(
                        Resources.Load<GameObject>("Prefabs/DronePrefab"),
                        droneStartLocation,
                        Quaternion.Euler(droneRotation)
                    );
                    droneObject.transform.localScale = Vector3.zero;
                    Camera.main.transform.position = droneObject.transform.position + droneEntryCameraOffset;
                    Camera.main.transform.rotation = Quaternion.identity;
                    Camera.main.fieldOfView = 60;
                    Debug.Log("Started: drone entry");
                }

                cutsceneActionTimer += Time.deltaTime;

                // Portal rotation.
                PortalRotation();

                // Ship descension.
                float descensionRatio = Mathf.SmoothStep(0, 1, Mathf.Clamp(cutsceneActionTimer / droneEntryDuration, 0, 1));
                droneObject.transform.position = Vector3.Lerp(droneStartLocation, droneTargetLocation, descensionRatio);

                // Camera ship tracking.
                Camera.main.transform.position = droneObject.transform.position + droneEntryCameraOffset;

                // Ship wobble.
                DroneMovement();

                // Ship growth.
                if (cutsceneActionTimer > droneGrowthDelay) { droneObject.transform.localScale = Vector3.one; }

                if (cutsceneActionTimer < droneEntryDuration) { return; }

                cutsceneActionTimer = 0;
                cutsceneStateInitialized = false;
                openingCutsceneState = OpeningCutsceneState.SecondStall;
                Debug.Log("Completed: drone entry");
                break;
            case OpeningCutsceneState.SecondStall:
                if (!cutsceneStateInitialized)
                {
                    cutsceneStateInitialized = true;
                    Debug.Log("Started: second stall");
                }

                cutsceneActionTimer += Time.deltaTime;

                // Portal rotation.
                PortalRotation();
                float portalScale = 1 - Mathf.Clamp(cutsceneActionTimer / 1, 0, 1);
                portalObject.transform.localScale = Vector3.one * portalScale;

                // Ship wobble.
                DroneMovement();

                if (cutsceneActionTimer < secondStallDuration) { return; }

                cutsceneActionTimer = 0;
                cutsceneStateInitialized = false;
                Destroy(portalObject);
                openingCutsceneState = OpeningCutsceneState.SecondDialogue;
                Debug.Log("Completed: second stall");
                break;
            case OpeningCutsceneState.SecondDialogue:
                if (!cutsceneStateInitialized)
                {
                    cutsceneStateInitialized = true;
                    DialogueManager.instance.StartDialogue(DialogueId.OpeningCutsceneSecondDialogue);
                    Debug.Log("Started: second dialogue");
                }

                // Ship wobble.
                bobRatio = 0.2f * Mathf.SmoothStep(0, 1, Mathf.PingPong(Time.time * droneBobSpeed, 1));
                droneObject.transform.Find("Mesh").transform.localPosition = new Vector3(
                    0,
                    -0.1f + bobRatio,
                    0
                );
                zWobbleRatio = 10f * Mathf.SmoothStep(0, 1, Mathf.PingPong(Time.time * droneWobbleSpeed, 1));
                droneObject.transform.Find("Mesh").transform.localRotation = Quaternion.Euler(new Vector3(
                    droneObject.transform.Find("Mesh").transform.localRotation.eulerAngles.x,
                    droneObject.transform.Find("Mesh").transform.localRotation.eulerAngles.y,
                    -5f + zWobbleRatio
                ));

                if (cutsceneActionTimer == 0) { return; }

                cutsceneActionTimer = 0;
                cutsceneStateInitialized = false;
                openingCutsceneState = OpeningCutsceneState.None;
                Debug.Log("Completed: second dialogue");
                Destroy(droneObject);
                GameManager.instance.StartGame();
                break;
            default:
                break;
        }
    }

    public void UpdateCutsceneDialogue()
    {
        cutsceneActionTimer++;
    }

    private void DroneMovement()
    {
        // Ship wobble.
        bobRatio = 0.2f * Mathf.SmoothStep(0, 1, Mathf.PingPong(Time.time * droneBobSpeed, 1));
        droneObject.transform.Find("Mesh").transform.localPosition = new Vector3(
            0,
            -0.1f + bobRatio,
            0
        );
        zWobbleRatio = 10f * Mathf.SmoothStep(0, 1, Mathf.PingPong(Time.time * droneWobbleSpeed, 1));
        droneObject.transform.Find("Mesh").transform.localRotation = Quaternion.Euler(new Vector3(
            droneObject.transform.Find("Mesh").transform.localRotation.eulerAngles.x,
            droneObject.transform.Find("Mesh").transform.localRotation.eulerAngles.y,
            -5f + zWobbleRatio
        ));
    }
    
    private void PortalRotation()
    {
        portalObject.transform.rotation = Quaternion.Euler(new Vector3(
            portalObject.transform.rotation.eulerAngles.x,
            portalObject.transform.rotation.eulerAngles.y + (portalRotationRate * Time.deltaTime),
            portalObject.transform.rotation.eulerAngles.z
        ));
    }
}