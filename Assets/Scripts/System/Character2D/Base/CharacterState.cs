namespace VD.Character2D
{
    using UnityEngine;

    public class CharacterState : StateMono
    {
        CharacterManager controller;

        public virtual void Initialize(CharacterManager _controller)
        {
            controller = _controller;
        }
    }

    public static class CharaState
    {
        public const string Idle = "Idle";
        public const string Move = "Move";
        public const string Jump = "Jump";
    }

    public static class CharaAnim
    {
        public static readonly int IdleHashed = Animator.StringToHash(CharaState.Idle);
        public static readonly int MoveHashed = Animator.StringToHash(CharaState.Move);
        public static readonly int JumpHashed = Animator.StringToHash(CharaState.Jump);
    }
}
