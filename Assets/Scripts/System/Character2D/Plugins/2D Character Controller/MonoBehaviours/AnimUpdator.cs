using UnityEngine;

namespace CharacterController2D
{
    public class AnimUpdator : MonoBehaviour
    {
        [SerializeField]
        CharacterMovementScript m_moveScript;

        [SerializeField]
        Animator m_animator;

        PlayerMovementInfo moveInfo;


        // Update is called once per frame
        void Update()
        {
            moveInfo = m_moveScript.GetMovementInfo();


            m_animator.SetFloat(CharaAnim.SpeedHashed, Mathf.Abs(moveInfo.directionNormal));//left right directional input
            m_animator.SetBool(CharaAnim.HitApexHashed, moveInfo.hitApex);//if apex of jump was hit
            m_animator.SetBool(CharaAnim.IsGroundedHashed, moveInfo.isGrounded);//if on ground
            m_animator.SetBool(CharaAnim.IsCrouchingHashed, moveInfo.isCrouching);//crouching
            m_animator.SetBool(CharaAnim.IsSlidingHashed, moveInfo.isSliding);//sliding down a slope
            m_animator.SetBool(CharaAnim.IsDodgingHashed, moveInfo.isDodging);//dodging
            m_animator.SetBool(CharaAnim.IsJumpingHashed, moveInfo.isJumping);//true if ascending and jump wa called, can be used to see if falling or jumping
            m_animator.SetBool(CharaAnim.IsNearWallHashed, moveInfo.isNearWall);//if near a wall, used with "isGrounded" to find if wall sliding
        }
    }

    public static class CharaAnim
    {
        public static readonly int SpeedHashed       = Animator.StringToHash("Speed");
        public static readonly int HitApexHashed     = Animator.StringToHash("HitApex");
        public static readonly int IsGroundedHashed  = Animator.StringToHash("IsGrounded");
        public static readonly int IsCrouchingHashed = Animator.StringToHash("IsCrouching");
        public static readonly int IsSlidingHashed = Animator.StringToHash("IsSliding");
        public static readonly int IsDodgingHashed = Animator.StringToHash("IsDodging");
        public static readonly int IsJumpingHashed = Animator.StringToHash("IsJumping");
        public static readonly int IsNearWallHashed = Animator.StringToHash("IsNearWall");
    }
}