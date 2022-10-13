using UnityEngine;

public interface IState
{
    public string stateName { get; set; }
    public bool IsStateActive { get; set; }

    public System.Action OnActive { get; set; }
    public System.Action OnDeactive { get; set; }

    void StateEnter();
    void StateUpdate();
    void StateFixedUpdate();
    void StateLateUpdate();
    void StateExit();
}

public abstract class State : IState
{
    public string stateName { get; set; }
    public bool IsStateActive { get; set; }

    public System.Action OnActive { get; set; }
    public System.Action OnDeactive { get; set; }

    public virtual void StateEnter()
    {

    }

    public virtual void StateExit()
    {

    }

    public virtual void StateFixedUpdate()
    {

    }

    public virtual void StateLateUpdate()
    {

    }

    public virtual void StateUpdate()
    {

    }
}

public abstract class StateMono : MonoBehaviour, IState
{
    public string stateName { get; set; }
    public bool IsStateActive { get; set; }

    public System.Action OnActive { get; set; }
    public System.Action OnDeactive { get; set; }

    public virtual void StateEnter()
    {

    }

    public virtual void StateExit()
    {

    }

    public virtual void StateFixedUpdate()
    {

    }

    public virtual void StateLateUpdate()
    {

    }

    public virtual void StateUpdate()
    {

    }
}

