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

    [Header("GameObject References")]
    [SerializeField]
    private GameObject portalObject;
    
    void Update()
    {
        ProcessOpeningCutscene();
    }

    private void ProcessOpeningCutscene()
    {
        if (!PlayerViewState.OpeningCutscene.Equals(GameManager.instance.GetPlayerViewState())) { return; }

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

                portalObject.transform.rotation = Quaternion.Euler(new Vector3(
                    portalObject.transform.rotation.eulerAngles.x,
                    portalObject.transform.rotation.eulerAngles.y + portalRotationRate,
                    portalObject.transform.rotation.eulerAngles.z
                ));

                Vector3 direction = portalObject.transform.position - Camera.main.transform.position;
                Quaternion lookRotation = Quaternion.LookRotation(direction, Camera.main.transform.up);
                Camera.main.transform.rotation = Quaternion.Slerp(Camera.main.transform.rotation, lookRotation, cameraTrackSpeed * Time.deltaTime);

                if (cutsceneActionTimer < portalOpenDuration) { return; }

                cutsceneActionTimer = 0;
                cutsceneStateInitialized = false;
                openingCutsceneState = OpeningCutsceneState.DroneEntry;
                Debug.Log("Completed: portal open");
                break;
            default:
                break;
        }
    }
    
    public void UpdateCutsceneDialogue()
    {
        cutsceneActionTimer++;
    }
}