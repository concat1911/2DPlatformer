namespace VD.Character2D
{
    using UnityEngine;

    public class Move : CharacterState
    {
        public override void Initialize(CharacterManager _controller)
        {
            base.Initialize(_controller);

            stateName = CharaState.Move;
        }


    }
}
