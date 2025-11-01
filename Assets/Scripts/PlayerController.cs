using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Inputs")]   
    public InputActionAsset inputActionAsset;
    private InputAction _look;
    private InputAction _move;
    private InputAction _boost;
    private InputAction _brake;
    InputDevice device;
    private Rigidbody rb;
    [Header("Movement Settings")]
    [SerializeField] private float panSpeed = 2f;
    [SerializeField] private float thrust = 5f;
    [SerializeField] private float boostSpeed = 10f;
    [SerializeField] private float brakeSpeed = 1f;
    private float oldThrust;

    [Header("Aim Controls")]  
    [SerializeField] private float lookSensitivity = 2f;
    [SerializeField] private float joystickSensitivity = 50f;
    [SerializeField] private bool inverted;
    [SerializeField] private bool onRailsMode;
    
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
        _brake = inputActionAsset.FindAction("Brake");

        _move.Enable();
        _look.Enable();
        _boost.Enable();
        _brake.Enable();

        rb.MoveRotation(initialRotation);
        oldThrust = thrust;
    }

    void FixedUpdate()
    {
        float boostAmount = _boost.ReadValue<float>();
        float brakeAmount = _brake.ReadValue<float>();

        device = _look.activeControl?.device;

        PlayerRotation();

        if (onRailsMode)
        {
            PlayerMovement();
        }
        else
        {
            ThrustHandler(boostAmount, brakeAmount);
        }
    }

    //Used to increase speed.
    //Can be fed other booleans & newThrust values to dynamically change speed.
    private void ThrustHandler(float boostAmount, float brakeAmount)
    {
        if (boostAmount > 0)
        {
            thrust = boostSpeed;
            Debug.Log("BOOST! | " + thrust);
        }
        else if (brakeAmount > 0)
        {
            thrust = brakeSpeed;
            Debug.Log("BRAKE! | " + thrust);
        }
        else
        {
            thrust = oldThrust;
            Debug.Log("RESET SPEED | " + thrust);
        }

        moveDirection = new Vector3(moveDirection.x, moveDirection.y, 1);
        rb.MovePosition(transform.position + transform.rotation * moveDirection * thrust * Time.fixedDeltaTime);
    }

    //
    private void PlayerMovement()
    {
        Vector2 moveAmount = _move.ReadValue<Vector2>();
        
        moveDirection = new Vector3(moveAmount.x, moveAmount.y, 1).normalized;
        rb.MovePosition(transform.position + transform.rotation * moveDirection * panSpeed * Time.fixedDeltaTime);
    }

    //Handles player rotation
    private void PlayerRotation()
    {
        //input
        Vector2 lookAmount = _look.ReadValue<Vector2>();
        if (!inverted) lookAmount.y *= -1f;

        // Check if input is joystick (roughly, if values are between -1 and 1)
        if (device is Gamepad || device is Joystick)
        {
            lookAmount *= joystickSensitivity; // joystick multiplier — tweak until it feels right
        }

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
        if (cam != null)
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
    
    public void ToggleInvertLook() => inverted = !inverted;
}
