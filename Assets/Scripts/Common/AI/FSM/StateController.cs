namespace VD.FSM
{
    using System.Collections.Generic;
    using System;

    public class StateController
    {
        Dictionary<string, IState> states = new Dictionary<string, IState>();

        public IState prevState { get; private set; }
        public IState curState { get; private set; }

        public event Action<IState> OnChangeState;

        public StateController()
        {
            states = new Dictionary<string, IState>();
        }

        public StateController(IState[] stateList)
        {
            for (int i = 0; i < stateList.Length; i++)
            {
                AddState(stateList[i]);
            }
        }

        public void PerformStateUpdate()
        {
            if (curState == null) return;
            curState?.StateUpdate();
        }

        public void PerformStateFixedUpdate()
        {
            if (curState == null) return;
            curState?.StateFixedUpdate();
        }

        public void PerformStateLateUpdate()
        {
            if (curState == null) return;
            curState?.StateLateUpdate();
        }

        public void AddState(IState newState)
        {
            if (!states.ContainsKey(newState.stateName))
            {
                states.Add(newState.stateName, newState);
            
            } else {

                UnityEngine.Debug.LogError("State " + newState.stateName + " is already registered.");
            }
        }

        public IState GetState(string name)
        {
            if (states.ContainsKey(name))
            {
                return states[name];
            }

            return null;
        }

        /// <summary>
        /// Change State by state name string
        /// </summary>
        /// <param name="stateName"></param>
        public void ChangeState(string stateName)
        {
            if (!states.ContainsKey(stateName))
            {
                UnityEngine.Debug.LogError(stateName + " is not registered | not exist in Dict.");
                return;
            }

            IState newState = states[stateName];

            ChangeState(newState);
        }

        /// <summary>
        /// Change State by state reference
        /// </summary>
        /// <param name="stateName"></param>
        public void ChangeState(IState newState)
        {
            if (newState == null) return;
            if (newState == curState) return;

            if (!states.ContainsValue(newState))
            {
                UnityEngine.Debug.LogError(newState.stateName + " is not registered | not exist in Dict.");
                return;
            }

            if( curState != null ) curState.IsStateActive = false;

            prevState = curState;
            curState = newState;

            prevState?.StateExit();
            if (prevState != null) prevState.OnDeactive?.Invoke();

            curState?.StateEnter();
            curState.IsStateActive = true;
            curState.OnActive?.Invoke();


            OnChangeState?.Invoke(newState);
        }

        public bool IsLastStateEqual( string state )
        {
            if (prevState == null) return false;

            return prevState == states[state];
        }
    }
}
