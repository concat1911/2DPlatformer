namespace VD.Platformer
{
    using UnityEngine;

    public class PlayerInput : MonoBehaviour
    {
        public float HMove { get; private set; }
        public bool  Jump { get; private set; }

        private void Update()
        {
            HMove = Input.GetAxis("Horizontal");
            Jump = Input.GetKey(KeyCode.Space);
        }
    }
}
