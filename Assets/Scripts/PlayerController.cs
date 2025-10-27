using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]   
    public InputActionAsset inputActionAsset;
    private InputAction _look;
    private InputAction _move;
    private InputAction _boost;
    private Rigidbody rb;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float lookSensitivity = 2f;

    private Vector3 moveDirection;
    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private Quaternion smoothRotation;
    private float rotationY;
    private float rotationX;

    void Start()
    {
        initialRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
        _look = inputActionAsset.FindAction("Look");
        _move = inputActionAsset.FindAction("Move");
        _boost = inputActionAsset.FindAction("Boost");
        rb.MoveRotation(initialRotation);
    }

    void FixedUpdate()
    {
        PlayerMovement();
        PlayerRotation();   
    }

    private void PlayerMovement()
    {
        Vector2 moveAmount = _move.ReadValue<Vector2>();
        float boostAmount = _boost.ReadValue<float>();
        moveDirection = new Vector3(moveAmount.x, moveAmount.y, boostAmount).normalized;
        rb.MovePosition(transform.position + transform.rotation * moveDirection * moveSpeed * Time.fixedDeltaTime);
    }

    //Handles player rotation
    private void PlayerRotation()
    {
        //input
        Vector2 lookAmount = _look.ReadValue<Vector2>();

        // rotation angles
        rotationY += lookAmount.x * lookSensitivity * Time.fixedDeltaTime;
        rotationX += lookAmount.y * lookSensitivity * Time.fixedDeltaTime;

        RotationRangeClamp(); //Clamp player rotation range to within cameras bounds
        
        //compute target rotation
        targetRotation = Quaternion.Euler(rotationX, rotationY, 0f);
        smoothRotation = Quaternion.Slerp(rb.rotation, targetRotation, lookSensitivity * Time.fixedDeltaTime);
        rb.MoveRotation(smoothRotation);
    }

    //clamps rotation range to within camera FOV.
    void RotationRangeClamp()
    {
        Camera cam = Camera.main;
        if(cam != null)
        {
            //Derive clamp values from camera fov
            float halfFOV = Camera.main.fieldOfView;

            // Get camera's current orientation in world space
            Vector3 camEuler = cam.transform.eulerAngles;
            if (camEuler.x > 180f) camEuler.x -= 360f;
            if (camEuler.y > 180f) camEuler.y -= 360f;

            // Clamp player rotation so it stays within the camera's visible FOV cone
            rotationX = Mathf.Clamp(rotationX, camEuler.x - halfFOV, camEuler.x + halfFOV);
            rotationY = Mathf.Clamp(rotationY, camEuler.y - halfFOV, camEuler.y + halfFOV);   
        }
    }
}
