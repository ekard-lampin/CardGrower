using System;
using UnityEngine;

[Serializable]
public class DialogueStep
{
    [SerializeField]
    private string text;
    public string GetText() { return text; }

    [SerializeField]
    private bool italicized;
    public bool IsItalicized() { return italicized; }
}