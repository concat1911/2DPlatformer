using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharacterController2D
{ 

[CreateAssetMenu(fileName = "ScriptableInputMap", menuName = "ScriptableObjects/ScriptableInputMap")]
public class ScriptableInputMap : ScriptableObject
{
    [System.Serializable]
    public struct InputInfo
    {
            [Header("Button Data")]
            [Tooltip("Button to be pressed that activates action")]
            public KeyCode m_button;
            
            [Tooltip("OPTIONAL button modifier\nIf set this button will need to be pressed at the same time as button to activate action")]
            public KeyCode m_buttonModifier;

            [Tooltip("If this input will be toggled each press. Will set input as held down on first press then set as up the next.\nInput is set to being held down right away and only uses Action, NOT HoldAction")]
            public bool m_toggleable;

            [Tooltip("If the Button should be checked for being held down")]
            public bool m_holdable;
            
            [Header("Action Data")]
            [Tooltip("The action to set when button is pressed")]
            public InputAction m_action;

            [Tooltip("OPTIONAL secondary action\nIf button is Holdable and held then this action will be set when button is held and first action will be set when button is pressed but not held")]
            public InputAction m_holdAction;
    }

    [SerializeField]
    public List<InputInfo> m_InputInfoList;
}

}