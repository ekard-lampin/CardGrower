using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    void Awake() { instance = this; }

    [SerializeField]
    private Dialogue[] dialogues;
    private Dictionary<DialogueId, Dialogue> dialogueLibrary = new Dictionary<DialogueId, Dialogue>();
    public Dialogue GetDialogueByDialogueId(DialogueId dialogueId)
    {
        return dialogueLibrary[dialogueId];
    }

    [SerializeField]
    private int dialogueIndex = 0;

    [SerializeField]
    private DialogueId activeDialogue = DialogueId.None;

    void Start()
    {
        foreach (Dialogue dialogue in dialogues) { dialogueLibrary.Add(dialogue.GetDialogueId(), dialogue); }
    }

    public void ProgressDialogue()
    {
        dialogueIndex++;
        if (dialogueIndex == GetDialogueByDialogueId(activeDialogue).GetDialogueSteps().Length)
        {
            dialogueIndex = 0;
            if (activeDialogue.ToString().ToLower().Contains("tutorial")) { GameManager.instance.SetPlayerViewState(PlayerViewState.Game); }
            if (!OpeningCutsceneState.None.Equals(CutsceneManager.instance.GetOpeningCutsceneState())) {
                GameManager.instance.SetPlayerViewState(PlayerViewState.OpeningCutscene);
                CutsceneManager.instance.UpdateCutsceneDialogue();
            }
            activeDialogue = DialogueId.None;
            ViewManager.instance.DestroyOpenView();
            return;
        }

        ViewManager.instance.OpenDialogueView(GetDialogueByDialogueId(activeDialogue).GetDialogueSteps()[dialogueIndex]);
    }

    public void SkipDialogue()
    {
        dialogueIndex = GetDialogueByDialogueId(activeDialogue).GetDialogueSteps().Length - 1;
        ProgressDialogue();
    }

    public void StartDialogue(DialogueId dialogueId)
    {
        dialogueIndex = 0;
        activeDialogue = dialogueId;
        ViewManager.instance.OpenDialogueView(GetDialogueByDialogueId(activeDialogue).GetDialogueSteps()[dialogueIndex]);
        // AudioManager.instance.PlayWindowOpen();
    }
}