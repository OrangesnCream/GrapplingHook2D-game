using UnityEngine;

public class Grapplevisual : MonoBehaviour
{
    [Header("Visual Settings")]
    public Color grappleLineColor = Color.white;
    public float lineWidth = 0.1f;
    public Material grappleMaterial;
    
    private LineRenderer line;
    private GrappleMovement grappleMovement;
    private Vector3 playerPosition;
    private Vector3 grapplePoint;
    private bool isGrappling = false;
    
    void Start()
    {
        line = gameObject.GetComponent<LineRenderer>();
        grappleMovement = GetComponent<GrappleMovement>();
        
        SetupLineRenderer();
    }

    void Update()
    {
        UpdateGrappleVisual();
    }
    
    void SetupLineRenderer()
    {
        if (line == null)
        {
            line = gameObject.AddComponent<LineRenderer>();
        }
        
        line.material = grappleMaterial != null ? grappleMaterial : new Material(Shader.Find("Sprites/Default"));
        line.material.color = grappleLineColor;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = 2;
        line.sortingOrder = 10; // Ensure it renders on top
        line.enabled = false;
    }
    
    void UpdateGrappleVisual()
    {
        if (grappleMovement != null)
        {
            isGrappling = grappleMovement.IsGrappling();
            
            if (isGrappling)
            {
                playerPosition = transform.position;
                grapplePoint = grappleMovement.GetGrapplePoint();
                
                line.enabled = true;
                line.SetPosition(0, playerPosition);
                line.SetPosition(1, grapplePoint);
                
                // Optional: Add some visual effects
                UpdateVisualEffects();
            }
            else
            {
                line.enabled = false;
            }
        }
    }
    
    void UpdateVisualEffects()
    {
        // Add subtle animation to the grapple line
        float time = Time.time;
        float wave = Mathf.Sin(time * 5f) * 0.02f;
        
        // Create a slight wave effect in the line
        Vector3 midPoint = Vector3.Lerp(playerPosition, grapplePoint, 0.5f);
        Vector3 perpendicular = Vector3.Cross(grapplePoint - playerPosition, Vector3.forward).normalized;
        midPoint += perpendicular * wave;
        
        // For more complex visual effects, you could use more line positions
        // For now, we'll keep it simple with just the two endpoints
    }
    
    // Public method to update grapple point externally
    public void SetGrapplePoint(Vector3 point)
    {
        grapplePoint = point;
    }
    
    // Method to change line color dynamically
    public void SetLineColor(Color color)
    {
        grappleLineColor = color;
        if (line != null)
        {
            line.material.color = color;
        }
    }
}
