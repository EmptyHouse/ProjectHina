using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EHDebugUIScreen : MonoBehaviour
{
    public Text DebugText;
    private List<DebugTextNode> DebugTextNodeList = new List<DebugTextNode>();

    public Text FrameCounterText;
    private int FramesPassedSinceLastUpdate;
    private float FrameTimePassedSinceLastUpdate;
    private const int FRAMES_PER_UPDATE = 10;

    #region monobehaviour methods
    private void Awake()
    {
        if (Application.isEditor)
        {
            EHDebug.DebugUIScreenInstance = this;
            UpdateDebugText();
        }
        else
        {
            this.gameObject.SetActive(false);//Turn off UI if we are in a game
        }
    }

    private void Update()
    {
        UpdateFrameCounter();
    }

    private void OnDestroy()
    {
        if (EHDebug.DebugUIScreenInstance == this)
        {
            EHDebug.DebugUIScreenInstance = null;
        }
    }

    #endregion monobehaviour methods

    #region debug message methods
    public void DebugAddMessage(string Message, float TimeToDisplay, Color ColorToDisplayMessage)
    {
        Message = "<color=#" + ColorUtility.ToHtmlStringRGBA(ColorToDisplayMessage) + ">:" + Message + "</color>";
        DebugTextNode TextNode = new DebugTextNode(Message, TimeToDisplay);
        DebugTextNodeList.Add(TextNode);
        StartCoroutine(RemoveMessageAfterTime(TextNode));

        UpdateDebugText();
    }

    private void DebugRemoveMessage(DebugTextNode TextNodeToRemove)
    {
        //We should be able to assume that the list contains the node is passed in
        DebugTextNodeList.Remove(TextNodeToRemove);

        UpdateDebugText();
    }

    private void UpdateDebugText()
    {
        string DebugMessage = "";

        for (int i = DebugTextNodeList.Count - 1; i >= 1; --i)
        {
            DebugMessage += DebugTextNodeList[i].DebugMessage + '\n';
        }
        if (DebugTextNodeList.Count >= 1)
        {
            DebugMessage += DebugTextNodeList[0].DebugMessage;
        }
        DebugText.text = DebugMessage;
    }

    private IEnumerator RemoveMessageAfterTime(DebugTextNode TextNode)
    {
        while (TextNode.TimeRemaining >= 0)
        {
            TextNode.TimeRemaining -= Time.deltaTime;
            yield return null;
        }
        DebugRemoveMessage(TextNode);
    }


    private class DebugTextNode
    {
        public string DebugMessage;
        public float TimeRemaining;

        public DebugTextNode(string DebugMessage, float TimeRemaining)
        {
            this.DebugMessage = DebugMessage;
            this.TimeRemaining = TimeRemaining;
        }
    }
    #endregion Debug Message Methods

    #region frame counter methods
    private void UpdateFrameCounter()
    {
        ++FramesPassedSinceLastUpdate;
        FrameTimePassedSinceLastUpdate += Time.unscaledDeltaTime;

        if (FramesPassedSinceLastUpdate >= FRAMES_PER_UPDATE)
        {
            FrameCounterText.text = (FramesPassedSinceLastUpdate / FrameTimePassedSinceLastUpdate).ToString("0.00") + " FPS";
            FramesPassedSinceLastUpdate = 0;
            FrameTimePassedSinceLastUpdate = 0;
        }
    }
    #endregion frame counter methods
}
