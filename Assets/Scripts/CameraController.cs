using UnityEngine;
using UnityEngine.Rendering;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform player;
    Vector3 cameraTargetPosition;
    [SerializeField] float smoothing = 5f; // smoothing

    private Vector3 offset;

    void Start()
    {
        // Store initial offset from player
        offset = transform.position - player.position;
        cameraTargetPosition = player.position + offset;
    }

    void LateUpdate()
    {
        UpdateCameraTargetPosition();
        //Slerp towards cameraTarget y-axis position
        transform.position = Vector3.Slerp(transform.position, cameraTargetPosition, smoothing * Time.deltaTime);
        //LookAt player
        transform.LookAt(player.position);
    }
    
    void UpdateCameraTargetPosition()
    {
         // Rotate the offset by the player's rotation around Y-axis
        Quaternion rotationY = Quaternion.Euler(0f, player.rotation.eulerAngles.y, 0f);
        cameraTargetPosition = player.position + rotationY * offset;
    }
}
