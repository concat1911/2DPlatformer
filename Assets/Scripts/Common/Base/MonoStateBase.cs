namespace ML
{
    using UnityEngine;

    public abstract class MonoStateBase<T> : MonoBehaviour
    {
        public T CurrentState { get; private set; }
        public T PrevState { get; private set; }

        public System.Action OnStateChange  { get; private set; }
        public System.Action OnStateEnter   { get; private set; }
        public System.Action OnStateExit    { get; private set; }

        protected virtual void Update()
        {
            StateUpdate();
        }

        protected virtual void FixedUpdate()
        {
            StateFixedUpdate();
        }

        protected virtual void LateUpdate()
        {
            StateLateUpdate();
        }

        protected virtual void StateEnter()
        {
            OnStateEnter?.Invoke();
        }

        protected virtual void StateUpdate()
        {

        }

        protected virtual void StateFixedUpdate()
        {

        }

        protected virtual void StateLateUpdate()
        {

        }

        protected virtual void StateExit()
        {
            OnStateExit?.Invoke();
        }

        protected virtual bool ChangeState(T newState, bool forceReload = false)
        {
            if (CompareState(CurrentState, PrevState) && !forceReload) return false;

            StateExit();

            PrevState = CurrentState;
            CurrentState = newState;

            StateEnter();

            OnStateChange?.Invoke();

            return true;
        }

        protected virtual bool CompareState(T stateA, T stateB)
        {
            // Need overrite this
            return false;
        }
    }
}
