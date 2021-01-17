using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : ScriptableObject
{
    public DialogueSentence[] DialogueSentenceList = new DialogueSentence[0];
}

[System.Serializable]
public class DialogueSentence
{
    [Tooltip("The name of the character that is speaking this sentence")]
    public string CharacterName;

    [TextArea]
    public string DialogueText;
}