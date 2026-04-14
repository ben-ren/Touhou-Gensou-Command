using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]public Transform player;
    [SerializeField] float smoothing = 10f; // smoothing

    Vector3 cameraTargetPosition;
    private Vector3 offset;
    private bool initialized;

    void Start()
    {
        // Store initial offset from player
        offset = transform.position - player.position;
        offset.y = 1.5f;
        cameraTargetPosition = player.position + offset;
    }

    void LateUpdate()
    {
        if (player == null) return;

        if (!initialized) Initialize();

        UpdateCameraTargetPosition();
        //Slerp towards cameraTarget y-axis position
        transform.position = Vector3.Slerp(transform.position, cameraTargetPosition, smoothing * Time.deltaTime);
        //LookAt player
        transform.LookAt(player.position);
    }

    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;
        initialized = false;
    }

    void Initialize()
    {
        offset = transform.position - player.position;
        offset.y = 1.5f;

        cameraTargetPosition = player.position + offset;
        initialized = true;
    }
    
    void UpdateCameraTargetPosition()
    {
         // Rotate the offset by the player's rotation around Y-axis
        Quaternion rotationY = Quaternion.Euler(0f, player.rotation.eulerAngles.y, 0f);
        cameraTargetPosition = player.position + rotationY * offset;
    }
}
