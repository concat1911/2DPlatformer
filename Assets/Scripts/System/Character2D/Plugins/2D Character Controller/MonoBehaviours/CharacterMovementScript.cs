using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/////////
//To swap to different input method get rid of m_controller and replace all calls to m_controller in HandleInput() with new input method
// input mostly handled in HandleInput()
/////////
//To add new state
// add info to PlayerMoveInfo and MovementConstraints in CharacterMoveData file
// add possible needed input into InputAction enum in InputData file
// implement into CharacterMovementScript so that it is set in HandleInput function
// add movement function if needed. Similar to how Jump or Dash ar functions
// implement into PlayerAnimation script or wherever animation is handled if anim is needed
// use anywhere else it is needed
/////////

/////////
//recommended to turn off raycast detect object it starts in Edit -> Project Settings -> physics2d -> queries start in colliders(disable) if wanting character to work properly while also setting ground layer to layer the character is on
// can ignore this if character is on seperate layer from walkable objects
////////

namespace CharacterController2D
{ 

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class CharacterMovementScript : MonoBehaviour, ICharacterMovement
{
    //info structs
    PlayerMovementInfo m_moveInfo;
    MovementConstraints m_constraints;

    //utility classes
    Controller m_controller;
    MoveComponent m_moveComponent;
    CollisionCheckComponent m_collisionCheck;

    
    [Header("Collision Config")]
    [Tooltip("The PhysicsMaterial2d of the object with desired friction and bounciness for object. Used when other objects outside of the Ground layer collide with character. A friction of 0 is set when moving and a friction of 100 is set when standing still.")]
    [SerializeField]
    PhysicsMaterial2D m_defaultFriction;
    [SerializeField]
    LayerMask m_groundLayers;
    [Tooltip("Offset of collider when crouching. Setting to 0 will prevent size changes to collider")]
    [SerializeField]
    Vector2 m_crouchColliderOffset = new Vector2(0f, 0f);
    [Tooltip("Size of collider when crouching. Setting to 0 will prevent size changes to collider")]
    [SerializeField]
    Vector2 m_crouchColliderSize = new Vector2(1f, 1f);
    [Tooltip("This is the offset from the center of the characters transform that will be used to determing if the character is at the top of a wall or can still climb")]
    [SerializeField]
    float m_ledgeCheckOffset = 0.6f;
    [Tooltip("Recommended that the ground check does not exceed the horizontale size of collider or wall checks may behave incorrectly.\nSet with Gizmos on to see.")]
    [SerializeField]
    float m_groundCheckSize = 0.45f;
    [SerializeField]
    float m_groundCheckHeadOffset = 0.6f;
    [SerializeField]
    float m_groundCheckFootOffset = 0.6f;
    [Tooltip("If ground check should use a square collider or circular collider")]
    [SerializeField]
    bool m_useSquareCheck;


    [Header("General Movement")]
    [Tooltip("The speed the character will normally move at with basic left and right input")]
    [SerializeField]
    float m_defaultSpeed = 10f;
    [Tooltip("The speed the chatacter will move when running.\nIf speed is set to 0 then running will be disabled and DefaultSpeed will be used.")]
    [SerializeField]
    float m_runSpeed = 0f;
    [SerializeField]
    float m_crouchedMoveSpeed = 0f;
    [Tooltip("Smothing applied to acceleration")]
    [SerializeField]
    [Range(0, 0.3f)] float m_movementSmoothing = 0.042f; 
    [Tooltip("The smallest angle before ground is considered a slope")]
    [SerializeField]
    float m_slopeThreshold = 5f;
    [Tooltip("The largest angle the character can walk on")]
    [SerializeField]
    float m_walkableSlope = 50f; 
    [Tooltip("When the character is considered to be on a slope.\n1 - just one side needs to be touching the slope.\n2 - side and middle\n3 - entirely on slope\nThis is directly linked to the 3 rays used to check the slope under the player. The number indicates the number of rays toouching an unwalkable slope.")]
    [SerializeField]
    [Range(1, 3)] int m_isOnSlopeCheckSetting = 1;


    [Header("Dodge stats")]
    [Tooltip("Will search these layers for objects with tags to ignore collision for and will allow the character to move through them when dodging.")]
    [SerializeField]
    LayerMask m_layerToCheckWhenDodging;
    [Tooltip("Tags on objects that character will move through Only when dodging.")]
    [SerializeField]
    string m_tagsOfObjectsToIgnoreCollisionFor;
    [SerializeField]
    float m_dodgeForce = 10f;
    [Tooltip("Time in seconds that dodge lasts")]
    [SerializeField]
    float m_dodgeDuritation = 0.3f; 
    [Tooltip("Time in seconds before another dodge can be done")]
    [SerializeField]
    float m_dodgeCooldown = 0.5f; 
    [SerializeField]
    bool m_stopMomentumAfterDodge;
    [SerializeField]
    bool m_canDodgeInAir = true;
    [Tooltip("Power multiplier for dodges done in air from 0% to 200%")]
    [SerializeField]
    [Range(0, 2f)]float m_airDodgeSpeedPercent = 0.5f;
    [SerializeField]
    bool m_stopMomentumAfterDodgeInAir; 


    [Header("Jump stats")]
    [SerializeField]
    int m_maxJumps = 2;
    [Tooltip("Normal jump power")]
    [SerializeField]
    float m_jumpForce = 10f;
    [Tooltip("Amount of controll character has in air from 0% to 200%")]
    [SerializeField]
    [Range(0, 2f)]float m_airControl = 0.5f; 
    [Tooltip("How jumping on slopes is handled.\n0 - can not jump while sliding\n1 - can only jump once if sliding\n2 - can jump as many times as available (will not refill jumps to max)\n3 - can jump off of slopes and regains jumps as if landing on ground")]
    [SerializeField]
    [Range(0, 3)]int m_slidingJumpSetting = 0;
    [Tooltip("If should use ascending and decending gravity")]
    [SerializeField]
    bool m_useCustomGravity;
    [Tooltip("Gravity scale used when jumping")]
    [SerializeField]
    float m_ascendingGravity;
    [Tooltip("Gravity scale used when falling")]
    [SerializeField]
    float m_descendingGravity;

    
    [Header("Wall Sliding stats")]
    [Tooltip("How fast character slids down a wall. This value is multiplied by the Y velocity.\nSetting to 0 will cause Wall grabs to happen automatically but prevent wall sliding.")]
    [SerializeField]
    float m_wallSlideSpeed = 0.8f;
    [Tooltip("The speed the character can move up walls. This value sets the Y velocity. Setting to 0 will not allow wall climbing")]
    [SerializeField]
    float m_wallClimbSpeed = 0f;
    [Tooltip("Jump power when sliding on a wall and jumping with an upward input")]
    [SerializeField]
    float m_upwardsWallJumpForce = 15f; 
    [Tooltip("Jump power when sliding on a wall and jumping with a downward input")]
    [SerializeField]
    float m_downwardsWallJumpForce = 5f;
    [Tooltip("Jump power when sliding on a wall and jumping with a horizontal input")]
    [SerializeField]
    float m_horizontalWallJumpsForce = 10f;
 
    
    PhysicsMaterial2D m_noFrictionMat;
    PhysicsMaterial2D m_fullFrictionMat;

    Vector2 m_standingColliderOffset;
    Vector2 m_standingColliderSize;

    bool m_ControllerEnabled;
    bool m_trueIsGrounded;
    bool m_isFacingRight;
    bool m_tryToStand;
    bool m_delayGroundCheck;
    bool m_frictionOverride;

    int m_jumpCount;
    bool m_usedSlideJump;

    float m_normalGravity;
    
    float m_slopeCheckDist = 1f;
    float m_wallCheckDist = 0.7f;

    float m_groundCheckDelay;

    float m_dodgeCooldownTime;
    float m_activeDodgeEndTime;

    float m_trueWallSlideSpeed;
    bool m_atTopOfWall;



    void Start()
    {
        Rigidbody2D rigidBody = gameObject.GetComponent<Rigidbody2D>();
        Collider2D collider = gameObject.GetComponent<Collider2D>();

        if(!m_defaultFriction)
            m_defaultFriction = new PhysicsMaterial2D();

        collider.sharedMaterial = m_defaultFriction;
        m_fullFrictionMat = new PhysicsMaterial2D();
        m_fullFrictionMat.friction = 100;
        m_fullFrictionMat.bounciness = m_defaultFriction.bounciness;
        m_noFrictionMat = new PhysicsMaterial2D();
        m_noFrictionMat.friction = 0;
        m_noFrictionMat.bounciness = m_defaultFriction.bounciness;

        m_controller = gameObject.GetComponent<Controller>();

        m_moveComponent = new MoveComponent(rigidBody);
        m_collisionCheck = new CollisionCheckComponent(rigidBody, collider, m_groundLayers);

        m_moveInfo = new PlayerMovementInfo(0f, false, true, false, false, false, false);
        m_constraints = new MovementConstraints(true);

        m_standingColliderOffset = m_collisionCheck.GetCollider2DOffset();
        m_standingColliderSize = m_collisionCheck.GetCollider2DSize();

        m_normalGravity = rigidBody.gravityScale;

        m_isFacingRight = true;
        m_trueWallSlideSpeed = m_wallSlideSpeed;

        if(m_controller)
            m_ControllerEnabled = true;
        else
            m_ControllerEnabled = false;
    }

    void Update()
    {
        //set isGrounded if groundCheckDelay is done
        if(Time.time >= m_groundCheckDelay)
        {
            m_moveInfo.isGrounded = m_trueIsGrounded;
            m_delayGroundCheck = false;
        }  

        if(m_moveInfo.isGrounded)
        {
            if(m_collisionCheck.IsOnSLope() && m_collisionCheck.GetGroundSlope() > m_walkableSlope)
            {
                m_moveInfo.isSliding = true;
                if(m_constraints.canFlipTransform)
                {
                    SetFaceDirection(m_moveComponent.GetVelocity().x);
                }
                
                if(m_slidingJumpSetting == 0)
                    m_constraints.canJump = false;
                else if(m_slidingJumpSetting == 1 && !m_usedSlideJump)
                {
                    m_jumpCount = 1;
                    m_usedSlideJump = true;
                }    
                else if(m_slidingJumpSetting == 3)
                    m_jumpCount = m_maxJumps;
            }
            else
            {   
                m_moveInfo.isSliding = false;
            
                if(m_slidingJumpSetting == 0)
                {
                    m_constraints.canJump = true;
                    m_jumpCount = m_maxJumps;
                }
                else if(m_slidingJumpSetting == 1)
                {
                    m_jumpCount = m_maxJumps;
                    m_usedSlideJump = false;   
                }
                else if(m_slidingJumpSetting == 2 || m_slidingJumpSetting == 3)
                    m_jumpCount = m_maxJumps;
            }


            if(!m_trueIsGrounded && !m_delayGroundCheck)
            {
                //short delay befor setting isGrounded to false
                m_groundCheckDelay = Time.time + 0.1f;
                m_delayGroundCheck = true;
            }
        }
        else
        {
            if(m_moveInfo.isSliding)
                m_moveInfo.isSliding = false;
        }



        if(!m_moveInfo.isGrounded && m_moveComponent.GetVelocity().y <= 0f)
            m_moveInfo.hitApex = true;
        else
            m_moveInfo.hitApex = false;


        if(m_moveInfo.isDodging)
        {
            if(m_dodgeDuritation != 0f)
            {
                if(Time.time >= m_activeDodgeEndTime)
                {
                    EndDodge();

                    if((m_stopMomentumAfterDodge && m_moveInfo.isGrounded) || (m_stopMomentumAfterDodgeInAir && !m_moveInfo.isGrounded))
                        m_moveComponent.SetVelocity(new Vector2(0,0));
                }
            }
        }

        if(m_moveInfo.hitApex || m_moveInfo.isGrounded)
            m_moveInfo.isJumping = false; 


        HandleInput();

        if(m_constraints.canFlipTransform == true)
        {
            if((!m_moveInfo.isNearWall || (m_moveInfo.isNearWall && m_moveInfo.isGrounded)) && !m_moveInfo.isSliding)
                SetFaceDirection(m_moveInfo.directionNormal);
        }    
            

    }

    void FixedUpdate() 
    {
        m_trueIsGrounded = m_collisionCheck.CheckLower(m_groundCheckSize, m_groundCheckFootOffset, m_useSquareCheck);

        if(m_trueIsGrounded)
        {
            bool tempSlope = m_collisionCheck.IsOnSLope();
            m_collisionCheck.UpdateSlopeCheck(m_slopeCheckDist, m_slopeThreshold, m_isOnSlopeCheckSetting);
        
            //adjust velocity to prevent flying off edges with slopes
            if(tempSlope == false && m_collisionCheck.IsOnSLope() == true)//onto slope
            {
                //down hill
                if((m_collisionCheck.IsSlopePositive() && m_moveInfo.directionNormal < 0) || (!m_collisionCheck.IsSlopePositive() && m_moveInfo.directionNormal > 0))
                {
                    //down hill force adjust
                    Vector2 tempVel = new Vector2(m_moveComponent.GetVelocity().x * 0.2f, -1);
                    tempVel = (Vector2)(Vector3.ProjectOnPlane(tempVel, m_collisionCheck.GetGroundNorm()));
                    m_moveComponent.SetVelocity(tempVel);
                }
            }
            else if(tempSlope == true && m_collisionCheck.IsOnSLope() == false)//off of slope
            {
                Vector2 tempVel = (Vector2)(Vector3.ProjectOnPlane(m_moveComponent.GetVelocity() * 0.5f, m_collisionCheck.GetGroundNorm()));
                m_moveComponent.SetVelocity(tempVel);
            }
        }

        //dodge collision. Allow player to dodge through other players
        if(m_moveInfo.isDodging)
            m_collisionCheck.IgnoreCollidersInRange(gameObject, m_layerToCheckWhenDodging, 3f, m_tagsOfObjectsToIgnoreCollisionFor);
        else
            m_collisionCheck.ReenableIgnoredColliders(gameObject, m_layerToCheckWhenDodging);

        if(m_tryToStand)
        {
            if(TryToStand())
                m_tryToStand = false;
        }


        WallCheck();

        UpdateGravity();
        UpdateFrictionMat();
        UpdateVelocity();

        //prevent from lossing all momentum on slope and getting stuck
        if(m_moveInfo.isSliding)
        {
            if((m_collisionCheck.IsSlopePositive() && !m_isFacingRight && m_moveComponent.GetVelocity().magnitude <= 1f)
                || (!m_collisionCheck.IsSlopePositive() && m_isFacingRight && m_moveComponent.GetVelocity().magnitude <= 1f))
                AddForwardForce(new Vector2(1f, 0f), ForceMode2D.Impulse);
        }
    }


    #region InterfaceFunctions
    //IMove//////////////////////////////
    public void AddForce(Vector2 force, ForceMode2D mode = ForceMode2D.Force)
    {
        if(m_collisionCheck.IsOnSLope())
            m_moveComponent.AddForce((Vector2)(Vector3.ProjectOnPlane(force, m_collisionCheck.GetGroundNorm())), mode);
        else
            m_moveComponent.AddForce(force, mode);

        //show force
        //Debug.DrawRay(m_collisionCheck.GetGroundInfo().point, m_collisionCheck.GetGroundPerpendicular(), Color.cyan);
        //Debug.DrawRay(m_collisionCheck.GetGroundInfo().point, force, Color.magenta);
    }

    public void AddForwardForce(Vector2 force, ForceMode2D mode = ForceMode2D.Force)
    {
        if(!m_isFacingRight)
            force.x *= -1f;

        if(m_collisionCheck.IsOnSLope())
            force = (Vector2)(Vector3.ProjectOnPlane(force, m_collisionCheck.GetGroundNorm()));
        
        AddForce(force, mode);
    }

    public void SetVelocity(Vector2 vector)
    {
        m_moveComponent.SetVelocity(vector);
    }

    public Vector2 GetVelocity()
    {
        return m_moveComponent.GetVelocity();
    }

    //ICharacterMovement//////////////////
    public PlayerMovementInfo GetMovementInfo()
    {
        return m_moveInfo;
    }

    public void SetMovementConstraints(MovementConstraints constraints)
    {
        m_constraints = constraints;
    }

    public MovementConstraints GetMovementConstraints()
    {
        return m_constraints;
    }

    public void CanControlMovement(bool canControl)
    {
        m_ControllerEnabled = canControl;
    }
    #endregion


    #region UpdateFunctions
    void HandleInput()
    {
        if(m_controller == null || !m_ControllerEnabled)
            return;

        if(!m_moveInfo.isDodging)
        {
            //directional input///
            if(m_constraints.canMove)
            {
                m_moveInfo.directionNormal = m_controller.GetHorizontalAxis();  
                if(m_moveInfo.isGrounded)  
                    m_frictionOverride = false;        


                //set if running///
                if(m_constraints.canRun && m_runSpeed > 0f && m_moveInfo.isGrounded)
                {
                    m_moveInfo.isRunning = m_controller.GetInputData(InputAction.Run).isDown;
                }
            }
            else
                m_moveInfo.directionNormal = 0f;

            
            //crouch input///
            if(m_constraints.canCrouch && m_moveInfo.isGrounded)
                Crouch(m_controller.GetInputData(InputAction.Crouch).isDown);

            //Wall grab///
            if(m_constraints.canGrabWall)
            {
                if(!m_moveInfo.isGrounded && m_moveInfo.isNearWall)
                {
                    if(!m_moveInfo.isClimbingWall && !m_moveInfo.isJumping)
                    {
                        if(m_wallSlideSpeed == 0f)
                            m_moveInfo.isGrabingWall = true;
                        else if(m_controller.GetInputData(InputAction.WallGrab).isDown)
                        {
                            m_moveInfo.isGrabingWall = true;
                            m_trueWallSlideSpeed = 0f;
                        }
                        else if(m_wallSlideSpeed != 0f && !m_controller.GetInputData(InputAction.WallGrab).isDown)
                        {
                            m_moveInfo.isGrabingWall = false;
                            m_trueWallSlideSpeed = m_wallSlideSpeed;
                        }
                    }
                    else
                    {
                        m_moveInfo.isGrabingWall = false;
                    }
                    
                }
                else
                {
                    m_moveInfo.isGrabingWall = false;
                    m_trueWallSlideSpeed = m_wallSlideSpeed;
                }
            }

            //wall climb///
            if(m_constraints.canClimbWall)
            {
                if(m_moveInfo.isNearWall && !m_moveInfo.isJumping && m_wallClimbSpeed > 0)
                {
                    if(m_controller.GetInputData(InputAction.Up).isDown)
                    {
                        if(!m_atTopOfWall)
                        {
                            m_moveInfo.isClimbingWall = true;
                            m_trueWallSlideSpeed = 0;
                            
                            if(m_moveInfo.isGrounded)
                            {
                                m_moveInfo.isGrounded = false;
                                m_trueIsGrounded = false;
                            }
                        }
                        else
                        {
                            m_moveInfo.isClimbingWall = false;
                            m_moveInfo.isGrabingWall = true;
                            m_trueWallSlideSpeed = 0f;
                        }
                        
                    }
                    else
                    {
                        if(!m_moveInfo.isGrabingWall)
                        {
                            m_moveInfo.isClimbingWall = false;
                            m_trueWallSlideSpeed = m_wallSlideSpeed;
                        }
                    }
                }
                else
                {
                    m_moveInfo.isClimbingWall = false;
                    m_trueWallSlideSpeed = m_wallSlideSpeed;
                }
            }


            //jump input///
            if(m_constraints.canJump) 
            {
                if(m_controller.GetInputData(InputAction.Jump).isDown && m_controller.GetInputData(InputAction.Jump).isNew)
                {
                    if(m_moveInfo.isNearWall && !m_moveInfo.isGrounded)
                    { 
                        WallJump(m_controller.GetVerticalAxis());
                    }
                    else if(m_jumpCount > 0 )
                    {
                        if(m_moveInfo.isCrouching)
                        {
                            TryToStand();
                            NormalJump();
                        }
                        else
                            NormalJump();
                    }    
                }
            }
        }

        //dash input///
        if(m_constraints.canDodge)
        {
            if(m_moveInfo.isGrounded || (!m_moveInfo.isGrounded && m_canDodgeInAir))
            {
                if(m_controller.GetInputData(InputAction.Dash).isDown && m_controller.GetInputData(InputAction.Dash).isNew)
                {
                    if(!(!m_moveInfo.isGrounded && m_moveInfo.isNearWall) && m_dodgeForce != 0f && m_collisionCheck.GetGroundSlope() <= m_walkableSlope)
                    {
                        if(Time.time >= m_dodgeCooldownTime && m_constraints.canDodge)//check dash delay
                        {
                            m_dodgeCooldownTime = Time.time + m_dodgeCooldown;
                            m_activeDodgeEndTime = Time.time + m_dodgeDuritation;
                            Dash(m_controller.GetHorizontalAxis());
                        }    
                    }
                }

            }
        }

    }

    void UpdateGravity()
    {
        //set gravity higher if moveing on ground but not dodging or on unwalkable slope
        //this helps with walking over corners
        if(m_moveInfo.isGrounded && m_trueIsGrounded && m_moveInfo.directionNormal != 0f && m_collisionCheck.GetGroundSlope() <= m_walkableSlope)
        {   
            //if move speed is greater than 0
            if(!(m_moveInfo.isCrouching && m_crouchedMoveSpeed == 0f) && !(!m_moveInfo.isCrouching && m_defaultSpeed == 0f) && !m_moveInfo.isDodging)
                m_collisionCheck.GetRigidbody2D().gravityScale = 10f;
        }
        else if((!m_moveInfo.isGrounded && m_moveInfo.isNearWall) && m_moveInfo.hitApex && m_trueWallSlideSpeed == 0f)
        {
            //if wall sliding and wallSlideSpeed is 0 then turn off gravity
            m_collisionCheck.GetRigidbody2D().gravityScale = 0f;
        }
        else if(m_useCustomGravity && (!m_moveInfo.isGrounded && !m_moveInfo.isNearWall))
        {
            //if using custom gravity for ascending and descending and also not wall sliding
            if(m_moveInfo.hitApex)
                m_collisionCheck.GetRigidbody2D().gravityScale = m_descendingGravity;
            else
                m_collisionCheck.GetRigidbody2D().gravityScale = m_ascendingGravity;
        }
        else
            m_collisionCheck.GetRigidbody2D().gravityScale = m_normalGravity;
    }

    void UpdateFrictionMat()
    {
        //full friction used to be stationary on a slope
        //no friction for walking or in air
        if(m_frictionOverride)
        {
            if(m_defaultFriction)
                m_collisionCheck.GetCollider2D().sharedMaterial = m_defaultFriction; 
            else
                m_collisionCheck.GetCollider2D().sharedMaterial = m_noFrictionMat;

        }
        else if(m_moveInfo.isGrounded && m_collisionCheck.GetGroundSlope() <= m_walkableSlope)
        {
            //if dodging or having a move speed faster than 0
            if(m_moveInfo.isDodging || (m_moveInfo.directionNormal != 0f && !(m_moveInfo.isCrouching && m_crouchedMoveSpeed == 0f) && !(!m_moveInfo.isCrouching && m_defaultSpeed == 0f)))
                m_collisionCheck.GetCollider2D().sharedMaterial = m_noFrictionMat;   
            else
                m_collisionCheck.GetCollider2D().sharedMaterial = m_fullFrictionMat;    
        }
        else
        {
            m_collisionCheck.GetCollider2D().sharedMaterial = m_noFrictionMat;   
        }
    }

    void UpdateVelocity()
    {
        if(m_moveInfo.isDodging == true || m_constraints.canMove == false || !m_ControllerEnabled)
        return;

        if(m_moveInfo.isGrounded && !m_moveInfo.isClimbingWall)
        {  
            float currentSpeed = 0f;
        
            if(m_moveInfo.isCrouching)
                currentSpeed = m_crouchedMoveSpeed;
            else
            {
                if(m_moveInfo.isRunning)
                    currentSpeed = m_runSpeed;
                else
                    currentSpeed = m_defaultSpeed;
            }

            //set velocity based on move speed, input direction and slope
            if(m_collisionCheck.IsOnSLope() && m_collisionCheck.GetGroundSlope() <= m_walkableSlope)
            {
                m_moveComponent.SmoothSetVelocity((m_moveInfo.directionNormal * currentSpeed) * -m_collisionCheck.GetGroundPerpendicular(), m_movementSmoothing);
            }
            else if(!m_collisionCheck.IsOnSLope())
            {
                m_moveComponent.SmoothSetVelocity(m_moveInfo.directionNormal * currentSpeed, m_moveComponent.GetVelocity().y, m_movementSmoothing);
            }
        }
        else
        {
            Vector2 targetVel;

            //wall sliding
            //near a wall in air and not ascending
            if(m_constraints.canWallSlide && m_moveInfo.isNearWall && (m_moveInfo.hitApex || m_moveComponent.GetVelocity().y == 0f))
            {
                if(m_moveInfo.isClimbingWall)
                    targetVel = new Vector2(m_moveComponent.GetVelocity().x, m_wallClimbSpeed);
                else
                    targetVel = new Vector2(m_moveComponent.GetVelocity().x, m_moveComponent.GetVelocity().y * m_trueWallSlideSpeed);  

                //little bit of force twords wall to keep character close
                if(m_isFacingRight)
                    m_moveComponent.AddForce(Vector2.right * 2f);
                else
                    m_moveComponent.AddForce(Vector2.right * -2f);
            }
            else
            {
                if(m_moveInfo.directionNormal == 0f)
                    targetVel = new Vector2(m_moveComponent.GetVelocity().x, m_moveComponent.GetVelocity().y);//in air with no air controll or input
                else
                    targetVel = new Vector2(m_moveComponent.GetVelocity().x + (m_moveInfo.directionNormal * (m_defaultSpeed * m_airControl)), m_moveComponent.GetVelocity().y);//in air with control
            }

            //clamp X speed
            if(targetVel.x > m_defaultSpeed)
                targetVel.x = m_defaultSpeed;
            if(targetVel.x < m_defaultSpeed * -1f)
                targetVel.x = m_defaultSpeed * -1f;
            
            m_moveComponent.SmoothSetVelocity(targetVel, m_movementSmoothing);
        }     
    }
    #endregion


    #region ActionFunctions
    void Crouch(bool crouch)
    {
        if(crouch && m_crouchColliderSize.x != 0f && m_crouchColliderSize.y != 0f)
        {
            SetColliderSize(m_crouchColliderSize, m_crouchColliderOffset);
            m_moveInfo.isCrouching = true;
        }
        else if(crouch)
        {
            m_moveInfo.isCrouching = true;
        }
        else
        {
            if(m_moveInfo.isCrouching)
            {
                m_tryToStand = true;
            }
        }
    }

    void NormalJump()
    {
        if(m_jumpForce == 0f)
        return;


        m_jumpCount -= 1;
        m_moveInfo.isJumping = true; 
        m_moveInfo.isGrounded = false;
        m_groundCheckDelay = Time.time + 0.05f;//delay ground check after a jump to prevent the frame when begining to jump but still close to ground

        m_moveComponent.SetVelocity(m_moveComponent.GetVelocity().x, m_jumpForce);
    }


    void WallJump(float verticalDir)
    {
        Vector2 move;
        bool hasPower = false;

        if(verticalDir > 0f)//up
        {
            if(m_isFacingRight)
            {
                move = new Vector2(m_moveComponent.GetVelocity().x + 1f, m_upwardsWallJumpForce);
            }
            else
            {
                move = new Vector2(m_moveComponent.GetVelocity().x - 1f, m_upwardsWallJumpForce);
            }

            if(m_upwardsWallJumpForce != 0f)
                hasPower = true;
            
        }
        else if(verticalDir < 0f)//down
        {
            if(m_isFacingRight)
            {
                SetFaceDirection(-1f);
                move = new Vector2(-2f, m_downwardsWallJumpForce);
            }
            else
            {
                SetFaceDirection(1f);
                move = new Vector2(2f, m_downwardsWallJumpForce);
            }

            if(m_downwardsWallJumpForce != 0f)
                hasPower = true;
        }
        else //neutral
        {
            if(m_moveInfo.directionNormal != 0f)
            {
                move = new Vector2(m_horizontalWallJumpsForce * m_moveInfo.directionNormal, m_horizontalWallJumpsForce);

                if(m_horizontalWallJumpsForce != 0f)
                    hasPower = true;
            }
            else //no direction 
            {
                move = new Vector2(m_moveComponent.GetVelocity().x, m_jumpForce);

                if(m_jumpForce != 0f)
                    hasPower = true;
            }
        }

        if(move != Vector2.zero && hasPower)
        {
            m_moveComponent.SetVelocity(move);

            //m_jumpCount -= 1;//uncomment to make walljumps take a jump stock
            m_moveInfo.isJumping = true; 
            m_moveInfo.isGrounded = false;
            m_groundCheckDelay = Time.time + 0.05f;//delay ground check for short time
        }

    }

    void Dash(float direction)
    {
        if(direction == 0f)
        {
            if(m_isFacingRight)
                direction = 1f;
            else
                direction = -1f;
        }

        SetFaceDirection(direction);

        Vector2 force = Vector2.zero;
        Vector2 vel = m_moveComponent.GetVelocity();


        if(m_moveInfo.isGrounded)
        { 
            if(m_collisionCheck.IsOnSLope())
            {
                force = (m_dodgeForce * direction) * -m_collisionCheck.GetGroundPerpendicular();
            }
            else
                force = new Vector2(m_dodgeForce * direction, vel.y + 1); 

            
            //dampen momentum befor dash
            m_moveComponent.SetVelocity(vel * 0.35f);
        }
        else
        {
            //dampen vertical momentum if dodging while jumping
            if(!m_moveInfo.hitApex)
                force = new Vector2((m_dodgeForce * direction), -2);
            else
                force = new Vector2((m_dodgeForce * direction), 0);
            

            //kills directional momentum if dodging in the opposite direction (run left + dodge right)
            if(vel.x > 0 && direction < 0 || vel.x < 0 && direction > 0)
            {
                force.x += vel.x * -1;
            }
            else
            {
                //reggulate air dodge speed based on momentum (keeps dodge more constant if moving slow or fast in air)
                if(vel.x > m_defaultSpeed/2f || vel.x < -(m_defaultSpeed/2f))
                {
                    force.x = force.x * m_airDodgeSpeedPercent;
                }
                else
                {
                    force.x = force.x * 0.9f;
                }
            }
        }
        
        m_moveComponent.AddForce(force, ForceMode2D.Impulse);
        m_moveInfo.isDodging = true;

        SetColliderSize(m_crouchColliderSize, m_crouchColliderOffset);
    }
    #endregion


    #region UtilityFunctions
    void EndDodge()
    {
        if(m_moveInfo.isDodging)
        {
            m_moveInfo.isDodging = false;
            TryToStand();
        }
    }

    bool TryToStand()
    {
        if(m_collisionCheck.CheckUpper(m_groundCheckSize, m_groundCheckHeadOffset, m_standingColliderSize, m_standingColliderOffset, m_useSquareCheck)) 
        {
            m_moveInfo.isCrouching = true;
            return false;
        }
        else
        {
            m_moveInfo.isCrouching = false;
            SetColliderSize(m_standingColliderSize, m_standingColliderOffset);
            return true;
        }
    }
 
    void WallCheck()
    {
        if(m_constraints.canWallSlide)
        {
            /* //Use this if having issues with character being in a layer marked in ground layers or if character has many attached colliders
                //switch with bellow 4 lines
            m_moveInfo.isNearWall = false;
            RaycastHit2D[] tempRaycastResults = new RaycastHit2D[5];
            ContactFilter2D tempFilter = new ContactFilter2D();
            tempFilter.SetLayerMask(m_groundLayers);


            if(m_isFacingRight)
                Physics2D.Raycast(transform.position, transform.right, tempFilter, tempRaycastResults, m_wallCheckDist);
            else
                Physics2D.Raycast(transform.position, transform.right * -1, tempFilter, tempRaycastResults, m_wallCheckDist);

            foreach(RaycastHit2D rayHit in tempRaycastResults)
            {
                if(rayHit && rayHit.collider.gameObject != gameObject)
                    m_moveInfo.isNearWall = true;
            }*/   

            ////
            if(m_isFacingRight)
                m_moveInfo.isNearWall = Physics2D.Raycast(transform.position, transform.right, m_wallCheckDist, m_groundLayers);
            else
                m_moveInfo.isNearWall = Physics2D.Raycast(transform.position, transform.right * -1, m_wallCheckDist, m_groundLayers);
            ////

            //sets when character will stop climbing walls
            if(m_moveInfo.isNearWall)
            {
                if(m_isFacingRight)
                    m_atTopOfWall = !Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + m_ledgeCheckOffset), transform.right, m_wallCheckDist, m_groundLayers);
                else
                    m_atTopOfWall = !Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + m_ledgeCheckOffset), transform.right * -1, m_wallCheckDist, m_groundLayers);
            }
        }
        else
        {
            m_moveInfo.isNearWall = false;
        } 
    }
 
 
    void SetFaceDirection(float direction)
    {
        Vector3 scale = transform.localScale;

        if(direction > 0 && !m_isFacingRight)
        {
            m_isFacingRight = true;

            if(scale.x < 0)
                scale.x *= -1;
        }
        else if(direction < 0 && m_isFacingRight)
        {
            m_isFacingRight = false;

            if(scale.x > 0)
                scale.x *= -1;
        }

        transform.localScale = scale;
    }

    void SetColliderSize(Vector2 size, Vector2 offset)
    {
        if(size.x != 0f && size.y != 0f)
        {
            m_collisionCheck.SetCollider2DSize(size);
            m_collisionCheck.SetCollider2DOffset(offset);
        }
    }
    #endregion


    void OnCollisionEnter2D(Collision2D collision)
    {
        //if object is not in ground layer
        if(!((m_groundLayers.value & (1 << collision.gameObject.layer)) > 0))
        {
            m_frictionOverride = true;    
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.layer != m_groundLayers)
        {
            m_frictionOverride = false;            
        }
    }

    void OnDrawGizmosSelected() 
    {
        //ground check
        if(m_collisionCheck != null && m_ControllerEnabled)
        {
            if(m_useSquareCheck)
            {
                Gizmos.color = Color.grey;
                Gizmos.DrawWireCube(m_collisionCheck.GetUpperCheckPosition(m_groundCheckSize, m_groundCheckHeadOffset), new Vector3(m_groundCheckSize, m_groundCheckSize, 0));    
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(m_collisionCheck.GetLowerCheckPosition(m_groundCheckSize, m_groundCheckFootOffset), new Vector3(m_groundCheckSize, m_groundCheckSize, 0));   
            }
            else
            {
                Gizmos.color = Color.grey;
                Gizmos.DrawWireSphere(m_collisionCheck.GetUpperCheckPosition(m_groundCheckSize, m_groundCheckHeadOffset), m_groundCheckSize);    
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(m_collisionCheck.GetLowerCheckPosition(m_groundCheckSize, m_groundCheckFootOffset), m_groundCheckSize);            
            }
            
        }

        //wall check
        Gizmos.color = Color.red;
        Vector2 tempPos = new Vector2(transform.position.x, transform.position.y + m_ledgeCheckOffset);

        if(m_isFacingRight)
        {
            Gizmos.DrawLine(tempPos, new Vector3(tempPos.x + m_wallCheckDist, tempPos.y, 0));
            Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + m_wallCheckDist, transform.position.y, transform.position.z));
        }
        else
        {
            Gizmos.DrawLine(tempPos, new Vector3(tempPos.x - m_wallCheckDist, tempPos.y, 0));
            Gizmos.DrawLine(transform.position, new Vector3(transform.position.x - m_wallCheckDist, transform.position.y, transform.position.z));
        }
    }
}

}