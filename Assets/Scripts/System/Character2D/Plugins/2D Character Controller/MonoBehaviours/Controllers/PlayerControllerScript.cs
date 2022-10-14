using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharacterController2D
{ 

//processes input from user and stores data in InputContainer
public class PlayerControllerScript : Controller
{
    [Tooltip("The length of time a button needs to be held down, in seconds, till it is set as held")]
    [SerializeField]
    float m_inputHoldDelay = 0.2f;

    [Tooltip("The number of previous inputs to save")]
    [SerializeField]
    int m_numInputsToSave = 2;

    [Tooltip("The length of time, in seconds, till saved inputs should be cleared. Timer reset after each input.")]
    [SerializeField]
    float m_inputStringSaveTime = 0.5f;

    [SerializeField]
    ScriptableInputMap m_inputMap;


    InputContainer m_inputContainer; 
    List<InputAction> m_inputsToSetFalse;
    bool m_lock;
    

    public void Awake()
    {
        m_inputContainer = new InputContainer(m_numInputsToSave, m_inputStringSaveTime);
        m_inputsToSetFalse = new List<InputAction>();
    }

    public void Update()
    {
        m_inputContainer.Update();
        
        if(m_inputsToSetFalse.Count > 0)
        {
            foreach(InputAction action in m_inputsToSetFalse)
                m_inputContainer.SetInputData(action, false);

            m_inputsToSetFalse.Clear();
        }


        foreach(ScriptableInputMap.InputInfo inputInfo in m_inputMap.m_InputInfoList)
        {
            if(UsesModifier(inputInfo.m_buttonModifier))
            {
                if(inputInfo.m_toggleable)
                {
                    ToggleInput(inputInfo.m_button, inputInfo.m_action);
                }
                else if(inputInfo.m_holdable)
                {
                    if(inputInfo.m_holdAction == InputAction.None)
                    {
                        CheckInputHeld(inputInfo.m_button, inputInfo.m_action);
                    }
                    else
                    {
                        CheckInputHeldToggle(inputInfo.m_button, inputInfo.m_action, inputInfo.m_holdAction);
                    }
                }
                else
                {
                    InputCheck(inputInfo.m_button, inputInfo.m_action);                
                }
            }
        }
    
    
        if(m_inputContainer.HasNewInput())//uncomment code for debug logs
        {
            //shows all inputs saved////
            //InputAction[] data = m_inputContainer.GetInputString();
            //string result = string.Join(" ", data);
            //Debug.Log(result);

            //shows last input////
            //Debug.Log(m_inputContainer.GetLastInput().type.ToString() + " " + m_inputContainer.GetLastInput().isHeld + " " + m_inputContainer.GetLastInput().isDown + " " + m_inputContainer.GetLastInputTime());
            
            //defines a series of inputs as a "combo" then prints if those inputs were given in that order////
            //need to be storing at least as many inputs as the length of the combo////
            //string combo = InputAction.Up.ToString() + InputAction.Up.ToString() + InputAction.Down.ToString();
            //Debug.Log(m_inputContainer.InputStringContains(combo));
        }
    }


    //overriden functions from Controller base class
    #region GettersAndSetters
    public override InputData GetInputData(InputAction inputValue)
    {
        return m_inputContainer.GetData(inputValue);
    }

    public override float GetInputTime(InputAction inputValue)
    {
        return m_inputContainer.GetInputTime(inputValue);
    }

    public override float GetHorizontalAxis()
    {
        float result = 0f;
        if(m_inputContainer.GetData(InputAction.Right).isDown)
            result = 1f;
        else if(m_inputContainer.GetData(InputAction.Left).isDown)
            result = -1f;

        return result;
    }

    public override float GetVerticalAxis()
    {
        float result = 0f;
        if(m_inputContainer.GetData(InputAction.Up).isDown)
            result = 1f;
        else if(m_inputContainer.GetData(InputAction.Down).isDown)
            result = -1f;

        return result;
    }

    public override InputData GetLastInputData()
    {
        return m_inputContainer.GetLastInput();
    }

    public override float GetLastInputTime()
    {
        return m_inputContainer.GetLastInputTime();
    }

    public override InputAction[] GetInputString()
    {
        return m_inputContainer.GetInputString();
    }
    public override bool InputStringContains(string inputActions)
    {
        return m_inputContainer.InputStringContains(inputActions);
    }

    public override void SetInputLock(InputAction inputType, bool locked)
    {
        m_inputContainer.SetInputLock(inputType, locked);
    }
    public override void SetInputLock(bool locked)
    {
        m_inputContainer.SetInputLock(locked);
    }

    public override bool GetInputLock(InputAction inputType)
    {
        return m_inputContainer.GetInputLock(inputType);
    }
    #endregion


    #region InputFunctionality
    bool UsesModifier(KeyCode inputModifier)
    {
        if(inputModifier != KeyCode.None)
        {
            if(Input.GetKey(inputModifier))
            {
                return true;
            }
        }
        else
        {
            return true;
        }

        return false;
    }

    void InputCheck(KeyCode inputKey, InputAction inputType)
    {
        if(!m_inputContainer.GetInputLock(inputType))
        {
            if(Input.GetKeyDown(inputKey))
            {
                m_inputContainer.SetInputData(inputType, true);
            }
            else if(Input.GetKeyUp(inputKey))
            {
                m_inputContainer.SetInputData(inputType, false);
            }
        }
    }

    //fires input as held if down longer that inputHoldDelay
    //fires input as normal if released before delay then set to false next frame
    void CheckInputHeld(KeyCode inputKey, InputAction inputType)
    {
        if(m_inputContainer.GetInputLock(inputType))
            return;

        if(!m_inputContainer.GetData(inputType).isDown && Input.GetKey(inputKey))
        {
            if(Time.time >= m_inputContainer.GetInputTime(inputType) + m_inputHoldDelay)
            {
                m_inputContainer.SetInputData(inputType, true, true);
                return;
            }
        }

        if(Input.GetKeyDown(inputKey))
        {
            if(!m_inputContainer.GetData(inputType).isDown)
            {
                m_inputContainer.SetInputTime(inputType, Time.time);
            }
        }

        if(Input.GetKeyUp(inputKey))
        {
            if(Time.time < m_inputContainer.GetInputTime(inputType) + m_inputHoldDelay)
            {
                m_inputsToSetFalse.Add(inputType);
                m_inputContainer.SetInputData(inputType, true);
            }
            else
                m_inputContainer.SetInputData(inputType, false);
        }
    }
    
    //fires first input if input tapped, second if held
    //fires after heldDelay or keyUp. if key is down after heldDelay time then input is considerd held
    void CheckInputHeldToggle(KeyCode inputKey, InputAction inputType1, InputAction inputType2)
    {
        if(m_inputContainer.GetInputLock(inputType1) || m_inputContainer.GetInputLock(inputType2))
            return;

        //if input is held up to or past the delay it is held and second input is used
        if(!m_inputContainer.GetData(inputType2).isDown && Input.GetKey(inputKey))
        {
            if(Time.time >= m_inputContainer.GetInputTime(inputType1) + m_inputHoldDelay)
            {
                m_inputContainer.SetInputData(inputType2, true, true);
                return;
            }
        }

        //start input time
        if(Input.GetKeyDown(inputKey))
        {
            if(!m_inputContainer.GetData(inputType1).isDown)
            {
                m_inputContainer.SetInputTime(inputType1, Time.time);
            }
        }

        //if key is released before held delay is hit it is tapped and first input is used
        if(Input.GetKeyUp(inputKey))
        {
            if(Time.time < m_inputContainer.GetInputTime(inputType1) + m_inputHoldDelay)
            {
                m_inputsToSetFalse.Add(inputType1);
                m_inputContainer.SetInputData(inputType1, true);
            }
            else
                m_inputContainer.SetInputData(inputType1, false);

            m_inputContainer.SetInputData(inputType2, false);
        }
    }

    void ToggleInput(KeyCode inputKey, InputAction inputType)
    {
        if(m_inputContainer.GetInputLock(inputType))
            return;

        if(Input.GetKeyDown(inputKey))
        {
            if(!m_inputContainer.GetData(inputType).isDown)
            {
                m_inputContainer.SetInputData(inputType, true, true);
            }
            else
            {
                m_inputContainer.SetInputData(inputType, false);
            }
        }

    }
    #endregion
    
}

}