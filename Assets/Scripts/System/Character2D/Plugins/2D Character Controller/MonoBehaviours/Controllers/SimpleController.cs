using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharacterController2D
{

//inherits from Controller class and uses InputContainer to provide simple left and right input to follow target
public class SimpleController : Controller
{
    InputContainer m_inputList;


    [SerializeField]
    Transform m_target;

    [SerializeField]
    float m_followDistance;



    // Start is called before the first frame update
    void Start()
    {
        m_inputList = new InputContainer(1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(m_target != null)
        {
            if(Mathf.Abs(Mathf.Abs(transform.position.x) - Mathf.Abs(m_target.position.x)) > m_followDistance)
            {
                if(m_target.position.x > transform.position.x)
                {
                    //go right
                    m_inputList.SetInputData(InputAction.Right, true);
                    m_inputList.SetInputData(InputAction.Left, false);
                }
                else
                {
                    //left
                    m_inputList.SetInputData(InputAction.Left, true);
                    m_inputList.SetInputData(InputAction.Right, false);
                }
            }
            else
            {
                m_inputList.SetInputData(InputAction.Right, false);
                m_inputList.SetInputData(InputAction.Left, false);
            }
        }
    }


    public override InputData GetInputData(InputAction inputValue)
    {
        return m_inputList.GetData(inputValue);
    }
}

}