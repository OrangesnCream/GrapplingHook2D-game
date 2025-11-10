using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject player;

    private Vector2 playerVelocity;
    private Vector2 cameraVelocity; //velocity vector the camera uses to dampen the camera movement
    void Start()
    {
        if (player == null)
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Player not found");
        }
        playerVelocity = player.GetComponent<Rigidbody2D>().linearVelocity;
        cameraVelocity=playerVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        playerVelocity = player.GetComponent<Rigidbody2D>().linearVelocity;
        MoveCamera();

    }
    //Move camera using the players movement vector so that the players can always see ahead of them in the direction of their movement
    void MoveCamera()
    {
        Vector3 newPosition;
        cameraVelocity = Vector2.Lerp(cameraVelocity, playerVelocity, Time.deltaTime * .5f);
        newPosition = (Vector3)player.transform.position + (Vector3)cameraVelocity*.12f;
        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
    }
}
