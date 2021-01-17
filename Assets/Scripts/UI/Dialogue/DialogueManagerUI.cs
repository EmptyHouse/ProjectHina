using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManagerUI : MonoBehaviour
{
    #region main variables
    private DialogueSentenceNode CurrentDialogueSentence;
    #endregion main variables

    #region mononbehaviour methods
    private void Update()
    {
        
    }
    #endregion monobehaivour methods
    public void StartDialogue(Dialogue Dialogue)
    {

    }

    private void DisplayNextSentence()
    {

    }
    private class DialogueSentenceNode
    {
        public DialogueSentence Sentence;
        public DialogueSentenceNode NextNode;
    }
}
