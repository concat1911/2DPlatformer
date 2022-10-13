namespace VD.Character2D
{
    using UnityEngine;
    using FSM;

    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class CharacterManager : MonoBehaviour
    {
        public Rigidbody2D m_rigidbody { get; protected set; }
        protected StateController stateController;

        protected virtual void Awake()
        {
            m_rigidbody = GetComponent<Rigidbody2D>();

            CharacterState[] states = GetComponentsInChildren<CharacterState>();

            stateController = new StateController(states);
        }

        protected virtual void Update()
        {
            stateController.PerformStateUpdate();
        }

        protected virtual void FixedUpdate()
        {
            stateController.PerformStateFixedUpdate();
        }
    }
}
