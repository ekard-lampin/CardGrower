using System;
using UnityEngine;

[Serializable]
public class Dialogue
{
    [SerializeField]
    private DialogueId dialogueId;
    public DialogueId GetDialogueId() { return dialogueId; }

    [SerializeField]
    private DialogueStep[] dialogueSteps;
    public DialogueStep[] GetDialogueSteps() { return dialogueSteps; }
}