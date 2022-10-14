using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharacterController2D
{

//listens to controller for input then moves using MoveComponent
[RequireComponent(typeof(Rigidbody2D))]
public class SimpleMove : MonoBehaviour
{

    [SerializeField]
    Controller m_controller;
    [SerializeField]
    float m_speed;

    MoveComponent m_moveComponent;



    // Start is called before the first frame update
    void Start()
    {
        Rigidbody2D rigidBody = gameObject.GetComponent<Rigidbody2D>();
        m_moveComponent = new MoveComponent(rigidBody);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(m_controller)
        {    
            //check if Right input is down
            if(m_controller.GetInputData(InputAction.Right).isDown)
            {
                m_moveComponent.SmoothSetVelocity(Vector2.right * m_speed, 0.042f);
            }

            //check if Left input is down
            if(m_controller.GetInputData(InputAction.Left).isDown)
            {
                m_moveComponent.SmoothSetVelocity(Vector2.left * m_speed, 0.042f);
            }
        }
    }

}

}