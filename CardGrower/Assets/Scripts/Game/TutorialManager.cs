using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;
    void Awake() { instance = this; }
    
    [SerializeField]
    private TutorialState tutorialState;
    public TutorialState GetTutorialState() { return tutorialState; }
    public void SetTutorialState(TutorialState tutorialState) { this.tutorialState = tutorialState; }

    [SerializeField]
    private HashSet<TutorialState> tutorialFlags = new HashSet<TutorialState>();
    public bool IsTutorialFlagSet(TutorialState tutorialState) { return tutorialFlags.Contains(tutorialState); }
    public void AddTutorialFlag(TutorialState tutorialState) { tutorialFlags.Add(tutorialState); }
    public void ResetTutorialFlags() { tutorialFlags = new HashSet<TutorialState>(); }

    [SerializeField]
    private bool tutorialMessageShown = false;

    [SerializeField]
    private float trackedAction = 0;
    public void UpdateTrackedLooking(float looking)
    {
        if (TutorialState.Look.Equals(tutorialState))
        {
            trackedAction += looking;
        }
    }
    public void UpdateTrackedMovement(float movement)
    {
        if (TutorialState.Move.Equals(tutorialState))
        {
            trackedAction += movement;
        }
    }
    public void UpdateTrackedToolShopAction()
    {
        if (TutorialState.ToolShop.Equals(tutorialState))
        {
            trackedAction++;
        }
    }
    public void UpdateTrackedTillAction()
    {
        if (TutorialState.ToolDeck.Equals(tutorialState))
        {
            trackedAction++;
        }
    }
    public void UpdateTrackedSeedShopAction()
    {
        if (TutorialState.SeedShop.Equals(tutorialState))
        {
            trackedAction++;
        }
    }
    public void UpdateTrackedPlantingAction()
    {
        if (TutorialState.SeedDeck.Equals(tutorialState))
        {
            trackedAction++;
        }
    }
    public void UpdateTrackedBoosterShopAction()
    {
        if (TutorialState.BoosterShop.Equals(tutorialState))
        {
            trackedAction++;
        }
    }
    public void UpdateTrackedBoosterUseAction()
    {
        if (TutorialState.BoosterDeck.Equals(tutorialState))
        {
            trackedAction++;
        }
    }
    public void UpdateTrackedGrowingAction()
    {
        if (TutorialState.Growing.Equals(tutorialState))
        {
            trackedAction++;
        }
    }
    public void UpdateTrackedHarvestAction()
    {
        if (TutorialState.Harvest.Equals(tutorialState))
        {
            trackedAction++;
        }
    }
    public void UpdateTrackedSellingAction()
    {
        if (TutorialState.Selling.Equals(tutorialState))
        {
            trackedAction++;
        }
    }

    void Start()
    {
        int[] tutorialStates = (int[])System.Enum.GetValues(typeof(TutorialState));
        for (int stateIndex = TutorialState.None.Equals(tutorialState) ? tutorialStates.Length - 1 : (int)tutorialState; stateIndex > 0; stateIndex--)
        {
            Debug.Log("Adding " + ((TutorialState)tutorialStates[stateIndex]).ToString() + " to tutorial flags.");
            tutorialFlags.Add((TutorialState)tutorialStates[stateIndex]);
        }
    }

    void Update()
    {
        HandleTutorialState();
    }

    private void HandleTutorialState()
    {
        if (!PlayerViewState.Game.Equals(GameManager.instance.GetPlayerViewState())) { return; }

        switch (tutorialState)
        {
            case TutorialState.Look:
                if (!tutorialMessageShown)
                {
                    tutorialMessageShown = true;
                    DialogueManager.instance.StartDialogue(DialogueId.TutorialLook);
                    tutorialFlags.Add(TutorialState.Look);
                }

                if (trackedAction > 100)
                {
                    tutorialMessageShown = false;
                    trackedAction = 0;
                    tutorialState = TutorialState.Move;
                }
                break;
            case TutorialState.Move:
                if (!tutorialMessageShown)
                {
                    tutorialMessageShown = true;
                    DialogueManager.instance.StartDialogue(DialogueId.TutorialMove);
                    tutorialFlags.Add(TutorialState.Move);
                }

                if (trackedAction > 500)
                {
                    tutorialMessageShown = false;
                    trackedAction = 0;
                    tutorialState = TutorialState.ToolShop;
                }
                break;
            case TutorialState.ToolShop:
                if (!tutorialMessageShown)
                {
                    tutorialMessageShown = true;
                    DialogueManager.instance.StartDialogue(DialogueId.TutorialTool);
                    tutorialFlags.Add(TutorialState.ToolShop);
                    if (GameObject.FindGameObjectWithTag("Player") != null)
                    {
                        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoneyManager>().AddMoney(GameManager.instance.GetToolPackPrice());
                    }
                }

                if (trackedAction > 0)
                {
                    tutorialMessageShown = false;
                    trackedAction = 0;
                    tutorialState = TutorialState.ToolDeck;
                }
                break;
            case TutorialState.ToolDeck:
                if (!tutorialMessageShown)
                {
                    tutorialMessageShown = true;
                    DialogueManager.instance.StartDialogue(DialogueId.TutorialTill);
                    tutorialFlags.Add(TutorialState.ToolDeck);
                }

                if (trackedAction > 0)
                {
                    tutorialMessageShown = false;
                    trackedAction = 0;
                    tutorialState = TutorialState.SeedShop;
                }
                break;
            case TutorialState.SeedShop:
                if (!tutorialMessageShown)
                {
                    tutorialMessageShown = true;
                    DialogueManager.instance.StartDialogue(DialogueId.TutorialSeed);
                    tutorialFlags.Add(TutorialState.SeedShop);
                    if (GameObject.FindGameObjectWithTag("Player") != null)
                    {
                        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoneyManager>().AddMoney(GameManager.instance.GetSeedPackPrice());
                    }
                }

                if (trackedAction > 0)
                {
                    tutorialMessageShown = false;
                    trackedAction = 0;
                    tutorialState = TutorialState.SeedDeck;
                }
                break;
            case TutorialState.SeedDeck:
                if (!tutorialMessageShown)
                {
                    tutorialMessageShown = true;
                    DialogueManager.instance.StartDialogue(DialogueId.TutorialPlanting);
                    tutorialFlags.Add(TutorialState.SeedDeck);
                }

                if (trackedAction > 0)
                {
                    tutorialMessageShown = false;
                    trackedAction = 0;
                    tutorialState = TutorialState.BoosterShop;
                }
                break;
            case TutorialState.BoosterShop:
                if (!tutorialMessageShown)
                {
                    tutorialMessageShown = true;
                    DialogueManager.instance.StartDialogue(DialogueId.TutorialBoosterShop);
                    tutorialFlags.Add(TutorialState.BoosterShop);
                    if (GameObject.FindGameObjectWithTag("Player") != null)
                    {
                        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMoneyManager>().AddMoney(GameManager.instance.GetBoosterPackPrice());
                    }
                }

                if (trackedAction > 0)
                {
                    tutorialMessageShown = false;
                    trackedAction = 0;
                    tutorialState = TutorialState.BoosterDeck;
                }
                break;
            case TutorialState.BoosterDeck:
                if (!tutorialMessageShown)
                {
                    tutorialMessageShown = true;
                    DialogueManager.instance.StartDialogue(DialogueId.TutorialBoosting);
                    tutorialFlags.Add(TutorialState.BoosterDeck);
                }

                if (trackedAction > 0)
                {
                    tutorialMessageShown = false;
                    trackedAction = 0;
                    tutorialState = TutorialState.Growing;
                }
                break;
            case TutorialState.Growing:
                if (!tutorialMessageShown)
                {
                    tutorialMessageShown = true;
                    DialogueManager.instance.StartDialogue(DialogueId.TutorialGrowing);
                    tutorialFlags.Add(TutorialState.Growing);
                }

                if (trackedAction > 0)
                {
                    tutorialMessageShown = false;
                    trackedAction = 0;
                    tutorialState = TutorialState.Harvest;
                }
                break;
            case TutorialState.Harvest:
                if (!tutorialMessageShown)
                {
                    tutorialMessageShown = true;
                    DialogueManager.instance.StartDialogue(DialogueId.TutorialHarvest);
                    tutorialFlags.Add(TutorialState.Harvest);
                }

                if (trackedAction > 0)
                {
                    tutorialMessageShown = false;
                    trackedAction = 0;
                    tutorialState = TutorialState.Selling;
                }
                break;
            case TutorialState.Selling:
                if (!tutorialMessageShown)
                {
                    tutorialMessageShown = true;
                    DialogueManager.instance.StartDialogue(DialogueId.TutorialSelling);
                    tutorialFlags.Add(TutorialState.Selling);
                }

                if (trackedAction > 0)
                {
                    tutorialMessageShown = false;
                    trackedAction = 0;
                    tutorialState = TutorialState.Done;
                }
                break;
            case TutorialState.Done:
                if (!tutorialMessageShown)
                {
                    tutorialMessageShown = true;
                    DialogueManager.instance.StartDialogue(DialogueId.TutorialDone);
                    tutorialFlags.Add(TutorialState.Done);
                }

                tutorialMessageShown = false;
                trackedAction = 0;
                tutorialState = TutorialState.None;
                break;
            default:
                break;
        }
    }
}