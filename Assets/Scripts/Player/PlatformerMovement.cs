using UnityEngine;
using UnityEngine.InputSystem;

public class PlatformerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float acceleration = 50f;
    [SerializeField] private float deceleration = 50f;
    [SerializeField] private float airControlMultiplier = 1f;
    
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private int maxAirJumps = 1;
    [SerializeField] private float jumpBufferTime = 0.2f;
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float gravityModifier = 75f;
    [SerializeField] private float maxFallModifier=-40f;
    
    [Header("Wall Jump Settings")]
    [SerializeField] private float wallJumpForce = 18f;
    [SerializeField] private float wallJumpHorizontalForce = 12f;
    [SerializeField] private LayerMask wallJumpLayerMask = -1;
    [SerializeField] private float wallDetectionDistance = 0.6f;
    
    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayerMask = -1;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    
    // Components
    private Rigidbody2D rb;
    private GrappleMovement grappleMovement;
    
    // Input
    private Vector2 moveInput;
    private bool jumpPressed;
    private float lastJumpPressTime;
    private bool jumpReleased;
    
    // State
    private bool isGrounded;
    private bool wasGroundedLastFrame;
    private float timeSinceGrounded;
    private int airJumpCount;
    private bool isOnWall;
    private bool isOnWallLeft;
    private bool isOnWallRight;
    private float lastWallJumpTime;
    private float wallJumpCooldown = 0.2f;


    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        grappleMovement = GetComponent<GrappleMovement>();
        
        // Ensure we have a Rigidbody2D
        if (rb == null)
        {
            Debug.LogError("PlatformerMovement requires a Rigidbody2D component!");
        }
    }
    
    void Update()
    {
        HandleInput();
    }
    
    void FixedUpdate()
    {
        CheckGround();
        CheckWall();
        HandleMovement();
        HandleJump();
        UpdateState();
    }
    
    void HandleInput()
    {
        // Get keyboard input
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;
        
        // Horizontal movement (A/D or Left/Right arrows)
        moveInput.x = 0f;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
        {
            moveInput.x = -1f;
        }
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
        {
            moveInput.x = 1f;
        }
        
        // Jump input (W or Space or Up arrow)
        bool jumpButtonPressed = keyboard.wKey.wasPressedThisFrame || 
                                 keyboard.spaceKey.wasPressedThisFrame || 
                                 keyboard.upArrowKey.wasPressedThisFrame;
        //Jump input released
        bool jumpButtonReleased = keyboard.wKey.wasReleasedThisFrame || 
                                 keyboard.spaceKey.wasReleasedThisFrame || 
                                 keyboard.upArrowKey.wasReleasedThisFrame;
        
        if (jumpButtonPressed)
        {
            jumpPressed = true;
            lastJumpPressTime = Time.time;
        }
        if (jumpButtonReleased)
        {
            jumpReleased = true;
        }
    }
    
    void HandleMovement()
    {
        // Don't apply horizontal movement if grappling (let grapple script handle it)
        if (grappleMovement != null && grappleMovement.IsGrappling())
        {
            return;
        }
        
        float targetVelocity = moveInput.x * moveSpeed;
        float currentVelocity = rb.linearVelocity.x;
        float velocityDifference = targetVelocity - currentVelocity;
        
        // Apply acceleration/deceleration
        float accelerationRate = Mathf.Abs(targetVelocity) > 0.01f ? acceleration : deceleration;
        
        if (!isGrounded)
        {

            accelerationRate *= airControlMultiplier/(Mathf.Abs(velocityDifference*0.10f)+1);

            /* new system, as the speed increases instead of pulling back to the target velocity the player slowly loses the ability 
            to accelerate forward but still maintains a lot of their ability to decelerate. This eventually tapers off to completely kill 
            acceleration  in the direction you are moving, basically a whole new movement system
            */
            float velocityModifier=accelerationRate*2;//final output 
            if (currentVelocity*targetVelocity<0)
            {
                //turning against velocity
                if (Mathf.Abs(currentVelocity)>moveSpeed)
                {
                    velocityModifier*=0.75f;
                }else if (Mathf.Abs(currentVelocity)>moveSpeed*2)
                {
                    velocityModifier*=0.50f;
                }else if (Mathf.Abs(currentVelocity) > moveSpeed * 3)
                {
                    velocityModifier*=0.25f;
                }
            }else if (currentVelocity * targetVelocity > 0)
            {
                //turning with velocity
                if (Mathf.Abs(currentVelocity)>moveSpeed)
                {
                    velocityModifier*=0.25f;
                }else if (Mathf.Abs(currentVelocity)>moveSpeed*2)
                {
                    velocityModifier*=0.10f;
                }else if (Mathf.Abs(currentVelocity) > moveSpeed * 3)
                {
                    velocityModifier*=0.5f;
                }
            }
            float force=targetVelocity*velocityModifier;
            rb.AddForce(new Vector2(force, 0f));
        }
        else
        {
            float force = velocityDifference * accelerationRate;
            rb.AddForce(new Vector2(force, 0f));
        }
         
        
        
        
        //faster fall down for snappier controls
        if (jumpReleased && rb.linearVelocity.y > 0 && airJumpCount == 0)
        {
            rb.AddForce(Vector2.up * Physics2D.gravity.y * (gravityModifier) * rb.mass); 
        }
        if (rb.linearVelocity.y < 0f)
        {
            //clamp downward velocity to avoid excessive speed
            rb.AddForce(Vector2.up * Physics2D.gravity.y * (1.5f) * rb.mass);
            if (rb.linearVelocity.y < maxFallModifier){
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxFallModifier);
            }
            
        }
        if (jumpReleased){
            jumpReleased=false;
        }



    }
    
    void HandleJump()
    {

        // Check if we should jump
        bool canJump = false;
        
        // Ground jump with coyote time
        if (isGrounded || (timeSinceGrounded < coyoteTime && wasGroundedLastFrame))
        {
            canJump = true;
            airJumpCount = 0; // Reset air jump count when grounded
        }
        // Wall jump
        else if (isOnWall && Time.time - lastWallJumpTime > wallJumpCooldown)
        {
            canJump = true;
        }
        // Air jump (double/triple jump)
        else if (!isGrounded && airJumpCount < maxAirJumps)
        {
            canJump = true;
        }
        
        // Check jump buffer
        bool jumpBufferActive = Time.time - lastJumpPressTime < jumpBufferTime;
        
        if (jumpPressed && canJump && jumpBufferActive)
        {
            PerformJump();
        }
        
        // Reset jump input
        if (jumpPressed)
        {
            jumpPressed = false;
        }
    }
    
    void PerformJump()
    {
        if (isOnWall)
        {
            // Wall jump
            Vector2 jumpDirection = new Vector2();
            
            // Jump away from the wall
            if (isOnWallLeft)
            {
                jumpDirection = new Vector2(1f, 1f).normalized; // Jump right
            }
            else if (isOnWallRight)
            {
                jumpDirection = new Vector2(-1f, 1f).normalized; // Jump left
            }
            
            // Apply wall jump force
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // Reset vertical velocity
            rb.AddForce(jumpDirection * wallJumpForce, ForceMode2D.Impulse);
            
            lastWallJumpTime = Time.time;
            

            Debug.Log("Wall jump!");
        }
        else
        {
            // Regular jump or air jump
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // Reset vertical velocity for consistent jump height
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            
            if (!isGrounded)
            {
                airJumpCount++;
                Debug.Log($"Air jump {airJumpCount}/{maxAirJumps}");
            }
        }
        
        jumpPressed = false;
        lastJumpPressTime = 0f; // Reset jump buffer
    }
    
    void CheckGround()
    {
        wasGroundedLastFrame = isGrounded;
        
        // Use box cast for more reliable ground detection
        Vector2 checkPosition = (Vector2)transform.position + Vector2.down * groundCheckDistance;
        RaycastHit2D hit = Physics2D.BoxCast(
            checkPosition,
            groundCheckSize,
            0f,
            Vector2.down,
            0.05f,
            groundLayerMask
        );
        
        isGrounded = hit.collider != null && rb.linearVelocity.y <= 0.1f;
        
        if (isGrounded)
        {
            timeSinceGrounded = 0f;
        }
        else
        {
            timeSinceGrounded += Time.fixedDeltaTime;
        }
    }
    
    void CheckWall()
    {
        // Check for walls on both sides
        Vector2 checkPosition = transform.position;
        
        // Check left wall
        RaycastHit2D hitLeft = Physics2D.Raycast(
            checkPosition,
            Vector2.left,
            wallDetectionDistance,
            wallJumpLayerMask
        );
        
        // Check right wall
        RaycastHit2D hitRight = Physics2D.Raycast(
            checkPosition,
            Vector2.right,
            wallDetectionDistance,
            wallJumpLayerMask
        );
        
        isOnWallLeft = hitLeft.collider != null;
        isOnWallRight = hitRight.collider != null;
        isOnWall = isOnWallLeft || isOnWallRight;
    }
    
    void UpdateState()
    {
        // Reset air jump count when grounded
        if (isGrounded && wasGroundedLastFrame == false)
        {
            airJumpCount = 0;
        }
    }
    
    // Public methods for external access
    public bool IsGrounded()
    {
        return isGrounded;
    }
    
    public bool IsOnWall()
    {
        return isOnWall;
    }
    
    public int GetRemainingAirJumps()
    {
        return maxAirJumps - airJumpCount;
    }
    
    // Debug visualization
    void OnDrawGizmos()
    {
        // Draw ground check
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector2 checkPosition = (Vector2)transform.position + Vector2.down * groundCheckDistance;
        Gizmos.DrawWireCube(checkPosition, groundCheckSize);
        
        // Draw wall checks
        Gizmos.color = isOnWallLeft ? Color.green : Color.yellow;
        Gizmos.DrawRay(transform.position, Vector2.left * wallDetectionDistance);
        
        Gizmos.color = isOnWallRight ? Color.green : Color.yellow;
        Gizmos.DrawRay(transform.position, Vector2.right * wallDetectionDistance);
    }
}

