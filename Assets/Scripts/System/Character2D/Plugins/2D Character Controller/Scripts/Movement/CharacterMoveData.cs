using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CharacterController2D
{ 

public enum MovementState//used for attacks and a key for combos////////////////////////
{
    None
    ,Grounded
    ,Airborn
}

//does not need to be Serializable but allows for debugging in editor
[System.Serializable]
public struct PlayerMovementInfo
{
    public float directionNormal;//horizontal input direction
    
    public bool hitApex;//if apex of jump was hit, same as if falling or ascending 
    public bool isGrounded;//if on ground
    public bool isRunning;
    public bool isCrouching;
    public bool isSliding;//if sliding down a slope
    public bool isDodging;
    public bool isJumping;//if jumping. true if jump function was fired and if ascending 
    public bool isNearWall;
    public bool isGrabingWall;
    public bool isClimbingWall;


    public PlayerMovementInfo(float norm = 0f, bool apex = false, bool onGround = false, bool running = false, bool crouch = false, bool sliding = false, bool dash = false, bool jump = false, bool nearWall = false)
    {
        directionNormal = norm;
        hitApex = apex;
        isGrounded = onGround;
        isRunning = running;
        isCrouching = crouch;
        isSliding = sliding;
        isDodging = dash;
        isJumping = jump;
        isNearWall = nearWall;
        isGrabingWall = false;
        isClimbingWall = false;
    }
    public PlayerMovementInfo(bool enableAll)
    {
        directionNormal = 0f;
        hitApex = enableAll;
        isGrounded = enableAll;
        isRunning = enableAll;
        isCrouching = enableAll;
        isSliding = enableAll;
        isDodging = enableAll;
        isJumping = enableAll;
        isNearWall = enableAll;
        isGrabingWall = enableAll;
        isClimbingWall = enableAll;
    }

    public MovementState GetMovementState()
    {
        if(isGrounded)
        {
            return MovementState.Grounded;
        }
        else
            return MovementState.Airborn;
    }

    public bool IsEqual(PlayerMovementInfo other)
    {
        if(directionNormal == other.directionNormal && hitApex == other.hitApex && isGrounded == other.isGrounded && isRunning == other.isRunning &&
            isCrouching == other.isCrouching && isSliding == other.isSliding && isDodging == other.isDodging && isJumping == other.isJumping && 
            isNearWall == other.isNearWall && isGrabingWall == other.isGrabingWall && isClimbingWall == other.isClimbingWall)
            return true;
        else
            return false;
    }
}

//used to restrict movement
[System.Serializable]
public struct MovementConstraints
{
    public bool canFlipTransform;
    public bool canCrouch;
    public bool canMove;
    public bool canRun;
    public bool canJump;
    public bool canDodge;
    public bool canWallSlide;
    public bool canGrabWall;
    public bool canClimbWall;
    
    
    public MovementConstraints(bool flipTrans, bool crouch, bool move, bool run, bool jump, bool dodge, bool wallSlide, bool grabWall, bool climbWall)
    {
        canFlipTransform = flipTrans;
        canCrouch = crouch;
        canMove = move;
        canRun = run;
        canJump = jump;
        canDodge = dodge;
        canWallSlide = wallSlide;
        canGrabWall = grabWall;
        canClimbWall = climbWall;
    }
    public MovementConstraints(bool enableAll)
    {
        canFlipTransform = enableAll;
        canCrouch = enableAll;
        canMove = enableAll;
        canRun = enableAll;
        canJump = enableAll;
        canDodge = enableAll;
        canWallSlide = enableAll;
        canGrabWall = enableAll;
        canClimbWall = enableAll;
    }
}

}