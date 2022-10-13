namespace VD.Platformer
{
    using UnityEngine;
    using Character2D;

    public class PlayerController : CharacterManager
    {
        PlayerInput input;

        protected override void Awake()
        {
            base.Awake();

            input = GetComponent<PlayerInput>();
        }
    }
}
