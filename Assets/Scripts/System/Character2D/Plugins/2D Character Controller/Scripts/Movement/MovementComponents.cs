using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharacterController2D
{ 

//simple class for easy movement of a Rigidbody2D
public class MoveComponent 
{
    Rigidbody2D m_rigidBody;

    Vector3 m_vel;

    bool m_disabled;



    public MoveComponent(Rigidbody2D rigidbody)
    {
        m_rigidBody = rigidbody;
        m_vel = Vector3.zero;
    }


    public void SetDisabled(bool disable)
    {
        m_disabled = disable;
    }
    public bool IsDisabled()
    {
        return m_disabled;
    }
    public void SetRigidBody(Rigidbody2D rigidbody)
    {
        m_rigidBody = rigidbody;
    }

    public Rigidbody2D GetRigidbody2D()
    {
        return m_rigidBody;
    }


    public void SmoothSetVelocity(float xVelocity, float yVelocity, float smoothing)
    {
        if(m_disabled)
            return;

        SmoothSetVelocity(new Vector2(xVelocity, yVelocity), smoothing);
    }
    public void SmoothSetVelocity(Vector2 velocity, float smoothing)
    {
        if(m_disabled)
            return;

        Vector3 targetVel = velocity;
        m_rigidBody.velocity = Vector3.SmoothDamp(m_rigidBody.velocity, targetVel, ref m_vel, smoothing);
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        if(m_disabled)
            return;

        m_rigidBody.velocity = new Vector3(xVelocity, yVelocity);
    }
    public void SetVelocity(Vector2 velocity)
    {
        if(m_disabled)
            return;

        SetVelocity(velocity.x, velocity.y);
    }

    public Vector3 GetVelocity()
    {
        return m_rigidBody.velocity;
    }


    public void AddForce(Vector2 force, ForceMode2D mode = ForceMode2D.Force)
    {
        if(m_disabled)
            return;

        m_rigidBody.AddForce(force, mode);
    }
}

}