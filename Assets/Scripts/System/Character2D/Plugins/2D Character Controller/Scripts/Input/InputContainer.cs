using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


namespace CharacterController2D
{ 

//holds data about InputActions for controllers to use
public class InputContainer
{
    public class InputTimeDataPair
    {
        public float time;
        public InputData data;

        public InputTimeDataPair(float t, InputData d)
        {
            time = t;
            data = d;
        }
    }


    Dictionary<InputAction, InputTimeDataPair> m_inputDic;
    List<InputAction> m_newInputs;
    InputAction[] m_inputString;
    bool m_hasNewInput;
    int m_numInputsToSave;
    int m_currentInputStringEmptyIndex;
    float m_inputStringSaveTime;
    float m_lastInputTime;


    public InputContainer()
        :this(0, 0.0f)
    {

    }
    public InputContainer(int numInputsToStore, float inputStringSaveTime)
    {
        if(numInputsToStore <= 0)
            numInputsToStore = 0;

        if(inputStringSaveTime < 0)
            inputStringSaveTime = 0;

        m_inputDic = new Dictionary<InputAction, InputTimeDataPair>();
        m_newInputs = new List<InputAction>();
        m_inputString = new InputAction[numInputsToStore];
        m_numInputsToSave = numInputsToStore;
        m_inputStringSaveTime = inputStringSaveTime;

        foreach (InputAction input in System.Enum.GetValues(typeof(InputAction)) )
        {
            m_inputDic.Add(input, new InputTimeDataPair(Mathf.Infinity, new InputData(input)) );
        }
    }

    public void Update()
    {
        SetNewInputsOld();
        UpdateInputString();
    }

    public bool HasNewInput()
    {
        return m_hasNewInput;
    }

    public float GetInputTime(InputAction inputType)
    {
        return m_inputDic[inputType].time;
    }
    
    public void SetInputTime(InputAction inputType, float time)
    {
        m_inputDic[inputType].time = time;
    }

    
    public void SetInputData(InputAction inputType, bool down, bool held = false)
    {
        if(!m_inputDic[inputType].data.isDown && down == true)
        {
            m_newInputs.Add(inputType);
            m_inputDic[inputType].data.isNew = down;
            m_inputDic[inputType].time = Time.time;
            m_hasNewInput = true;
            AddInputToInputString(inputType);
        }

        if(down)
            m_lastInputTime = Time.time;
        else
            m_inputDic[inputType].time = Mathf.Infinity;
        

        m_inputDic[inputType].data.isDown = down;
        m_inputDic[inputType].data.isHeld = held;
    }
    public InputData GetData(InputAction inputType)
    {
        return m_inputDic[inputType].data;
    }

    public InputData GetLastInput()
    {
        int index = GetNewestInputIndex();
        if(m_inputString[index] == InputAction.None)
        {
            return null;
        }

        return m_inputDic[m_inputString[index]].data;
    }
    public float GetLastInputTime()
    {
        int index = GetNewestInputIndex();
        if(m_inputString[index] == InputAction.None)
        {
            return 0f;
        }

        return m_inputDic[m_inputString[index]].time;
    }
    public InputAction[] GetInputString()
    {
        return m_inputString;
    }

    public bool InputStringContains(string inputActions)
    {
        if(inputActions != null && m_inputString.Length > 0)
        {
            if(m_inputString.Length > 1)
            {
                string actions = String.Join("", m_inputString);
                if(actions.Contains(inputActions))
                    return true;
                else
                    return false;
            }
            else if(m_inputString[0].ToString() == inputActions)
                return true;
            else
                return false;
        }
        else
            return false;
         
    }

    public void SetInputLock(InputAction inputType, bool locked)
    {
        m_inputDic[inputType].data.inputLocked = locked;
    }
    public void SetInputLock(bool locked)
    {
        foreach (InputAction key in m_inputDic.Keys)
        {
            m_inputDic[key].data.inputLocked = locked;
        }
    }

    public bool GetInputLock(InputAction inputType)
    {
        return m_inputDic[inputType].data.inputLocked;
    }

    void SetNewInputsOld()
    {
        if(m_newInputs.Count != 0)
        {
            for (int i = 0; i < m_newInputs.Count; i++)
            {
                m_inputDic[m_newInputs[i]].data.isNew = false;
                m_hasNewInput = false;
            }
        }
    }

    void UpdateInputString()
    {
        if(Time.time >= m_lastInputTime + m_inputStringSaveTime && m_inputString[0] != InputAction.None)
        {
            Array.Clear(m_inputString, 0, m_inputString.Length);
            m_currentInputStringEmptyIndex = 0;
        }
    }

    int GetNewestInputIndex()
    {
        if(m_currentInputStringEmptyIndex <= m_inputString.Length - 1)
        {
            return m_currentInputStringEmptyIndex - 1;
        }
        else
        {
            return m_inputString.Length - 1;
        }
    }

    void AddInputToInputString(InputAction inputAction)
    {
        if(m_inputDic[inputAction].data.isNew)
        {
            if(m_currentInputStringEmptyIndex < m_inputString.Length - 1)
            {
                m_inputString[m_currentInputStringEmptyIndex] = inputAction;
                ++m_currentInputStringEmptyIndex;
            }
            else
            {
                if(m_currentInputStringEmptyIndex == m_inputString.Length - 1)
                {
                    m_inputString[m_currentInputStringEmptyIndex] = inputAction;
                    ++m_currentInputStringEmptyIndex;
                }
                else
                {
                    Array.Copy(m_inputString, 1, m_inputString, 0, m_inputString.Length - 1);
                    m_inputString[m_inputString.Length - 1] = inputAction;
                }
            }
        }
    }
}

}