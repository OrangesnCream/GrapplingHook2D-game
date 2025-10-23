using UnityEngine;

public class GrappleMovement : MonoBehaviour
{
    [Header("Grapple Settings")]
    public float grappleRange = 10f;
    public float grappleSpeed = 20f;
    public float swingForce = 15f;
    public float pullForce = 25f;
    public LayerMask grappleLayerMask = -1;
    
    [Header("Physics Settings")]
    public float maxGrappleLength = 8f;
    public float minGrappleLength = 2f;
    public float momentumMultiplier = 1.5f;
    public float airDrag = 0.98f;
    
    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse0;
    public KeyCode releaseKey = KeyCode.Mouse1;
    
    // Private variables
    private Rigidbody2D rb;
    private bool isGrappling = false;
    private Vector2 grapplePoint;
    private Vector2 grappleDirection;
    private float currentGrappleLength;
    private Vector2 lastVelocity;
    private Camera cam;
    
    // Components
    private LineRenderer grappleLine;
    private Grapplevisual grappleVisual;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        
        // Get or create grapple line renderer
        grappleLine = GetComponent<LineRenderer>();
        if (grappleLine == null)
        {
            grappleLine = gameObject.AddComponent<LineRenderer>();
        }
        
        grappleVisual = GetComponent<Grapplevisual>();
        
        SetupGrappleLine();
    }
    
    void Update()
    {
        HandleInput();
        UpdateGrapplePhysics();
        UpdateGrappleVisual();
    }
    
    void HandleInput()
    {
        if (Input.GetKeyDown(grappleKey) && !isGrappling)
        {
            TryGrapple();
        }
        
        if (Input.GetKeyDown(releaseKey) && isGrappling)
        {
            ReleaseGrapple();
        }
        
        // Allow extending/retracting grapple length
        if (isGrappling)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                AdjustGrappleLength(scroll * 2f);
            }
        }
    }
    
    void TryGrapple()
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - (Vector2)transform.position).normalized;
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, grappleRange, grappleLayerMask);
        
        if (hit.collider != null)
        {
            StartGrapple(hit.point);
        }
    }
    
    void StartGrapple(Vector2 point)
    {
        isGrappling = true;
        grapplePoint = point;
        grappleDirection = (grapplePoint - (Vector2)transform.position).normalized;
        currentGrappleLength = Vector2.Distance(transform.position, grapplePoint);
        
        // Clamp grapple length
        currentGrappleLength = Mathf.Clamp(currentGrappleLength, minGrappleLength, maxGrappleLength);
        
        // Store initial velocity for momentum calculation
        lastVelocity = rb.linearVelocity;
        
        Debug.Log($"Grapple attached to {grapplePoint}");
    }
    
    void ReleaseGrapple()
    {
        if (!isGrappling) return;
        
        // Apply momentum boost when releasing
        Vector2 releaseDirection = (Vector2)transform.position - grapplePoint;
        releaseDirection = releaseDirection.normalized;
        
        // Add momentum based on current velocity
        Vector2 momentumBoost = rb.linearVelocity * momentumMultiplier;
        rb.linearVelocity = momentumBoost;
        
        isGrappling = false;
        grappleLine.enabled = false;
        
        Debug.Log("Grapple released with momentum boost");
    }
    
    void UpdateGrapplePhysics()
    {
        if (!isGrappling) return;
        
        Vector2 playerPos = transform.position;
        Vector2 toGrapplePoint = grapplePoint - playerPos;
        float distanceToGrapple = toGrapplePoint.magnitude;
        
        // Constrain player to grapple length
        if (distanceToGrapple > currentGrappleLength)
        {
            Vector2 constrainedPos = grapplePoint - toGrapplePoint.normalized * currentGrappleLength;
            transform.position = constrainedPos;
            
            // Adjust velocity to maintain constraint
            Vector2 velocityDirection = rb.linearVelocity.normalized;
            Vector2 tangentDirection = Vector2.Perpendicular(toGrapplePoint.normalized);
            
            // Project velocity onto tangent (swing direction)
            float tangentVelocity = Vector2.Dot(rb.linearVelocity, tangentDirection);
            rb.linearVelocity = tangentDirection * tangentVelocity;
        }
        
        // Apply swing forces
        ApplySwingForces();
        
        // Apply pull forces if player is too far
        if (distanceToGrapple > currentGrappleLength * 0.8f)
        {
            ApplyPullForces();
        }
        
        // Apply air resistance
        rb.linearVelocity *= airDrag;
        
        // Store velocity for next frame
        lastVelocity = rb.linearVelocity;
    }
    
    void ApplySwingForces()
    {
        Vector2 toGrapplePoint = grapplePoint - (Vector2)transform.position;
        Vector2 tangentDirection = Vector2.Perpendicular(toGrapplePoint.normalized);
        
        // Calculate swing force based on gravity and current velocity
        Vector2 gravityForce = Physics2D.gravity * rb.mass;
        Vector2 swingForceVector = Vector2.Dot(gravityForce, tangentDirection) * tangentDirection * swingForce;
        
        rb.AddForce(swingForceVector);
    }
    
    void ApplyPullForces()
    {
        Vector2 toGrapplePoint = grapplePoint - (Vector2)transform.position;
        Vector2 pullDirection = toGrapplePoint.normalized;
        
        // Apply pull force towards grapple point
        Vector2 pullForceVector = pullDirection * pullForce;
        rb.AddForce(pullForceVector);
    }
    
    void AdjustGrappleLength(float adjustment)
    {
        currentGrappleLength += adjustment;
        currentGrappleLength = Mathf.Clamp(currentGrappleLength, minGrappleLength, maxGrappleLength);
    }
    
    void UpdateGrappleVisual()
    {
        if (isGrappling)
        {
            grappleLine.enabled = true;
            grappleLine.SetPosition(0, transform.position);
            grappleLine.SetPosition(1, grapplePoint);
            
            // Update grapple visual if it exists
            if (grappleVisual != null)
            {
                // The grapple visual will handle its own updates
            }
        }
        else
        {
            grappleLine.enabled = false;
        }
    }
    
    void SetupGrappleLine()
    {
        grappleLine.material = new Material(Shader.Find("Sprites/Default"));
        grappleLine.material.color = Color.white;
        grappleLine.startWidth = 0.1f;
        grappleLine.endWidth = 0.1f;
        grappleLine.positionCount = 2;
        grappleLine.enabled = false;
    }
    
    // Public methods for external access
    public bool IsGrappling()
    {
        return isGrappling;
    }
    
    public Vector2 GetGrapplePoint()
    {
        return grapplePoint;
    }
    
    public float GetGrappleLength()
    {
        return currentGrappleLength;
    }
    
    // Debug visualization
    void OnDrawGizmos()
    {
        if (isGrappling)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(grapplePoint, 0.5f);
            Gizmos.DrawLine(transform.position, grapplePoint);
        }
        
        // Draw grapple range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, grappleRange);
    }
}
