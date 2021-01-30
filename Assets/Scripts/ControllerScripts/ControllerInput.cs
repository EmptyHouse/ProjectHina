using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InputMapping", menuName = "ScriptableObjects/Controller", order = 1)]
public class ControllerInput : ScriptableObject
{
    [SerializeField]
    private ButtonInputNode[] ButtonInputNodes = new ButtonInputNode[1];
    [SerializeField]
    private AxisInputNode[] AxisInputNodes = new AxisInputNode[1];

    private struct ButtonInputNode
    {
        public string ButtonName;
        public KeyCode PrimaryButton;
        public KeyCode AlternativeButton;
    }

    private struct AxisInputNode
    {
        public string AxisName;
    }
}
