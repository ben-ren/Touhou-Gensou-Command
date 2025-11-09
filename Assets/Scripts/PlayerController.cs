using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputController IC;
    private GrazeAbility ability;
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
        rb.MoveRotation(initialRotation);
        oldThrust = thrust;
        
        if(IC == null)
        {
            IC = GetComponent<InputController>();
        }
    }

    void FixedUpdate()
    {
        PlayerRotation();
        TriggerAbility();
        if (onRailsMode)
        {
            PlayerMovement();
        }
        else
        {
            ThrustHandler(IC.GetBoost(), IC.GetBrake());
        }
    }

    //Used to increase speed.
    //Can be fed other booleans & newThrust values to dynamically change speed.
    private void ThrustHandler(float boostAmount, float brakeAmount)
    {
        if (boostAmount > 0)
        {
            thrust = boostSpeed;
            //Debug.Log("BOOST! | " + thrust);
        }
        else if (brakeAmount > 0)
        {
            thrust = brakeSpeed;
            //Debug.Log("BRAKE! | " + thrust);
        }
        else
        {
            thrust = oldThrust;
            //Debug.Log("RESET SPEED | " + thrust);
        }

        moveDirection = new Vector3(moveDirection.x, moveDirection.y, 1);
        rb.MovePosition(transform.position + transform.rotation * moveDirection * thrust * Time.fixedDeltaTime);
    }

    //
    private void PlayerMovement()
    {
        Vector2 moveAmount = IC.GetMove();
        
        moveDirection = new Vector3(moveAmount.x, moveAmount.y, 1).normalized;
        rb.MovePosition(transform.position + transform.rotation * moveDirection * panSpeed * Time.fixedDeltaTime);
    }

    //Handles player rotation
    private void PlayerRotation()
    {
        //input
        Vector2 lookAmount = IC.GetLook();
        if (!inverted) lookAmount.y *= -1f;

        // Check if input is joystick (roughly, if values are between -1 and 1)
        if (IC.GetActiveDevice() is Gamepad || IC.GetActiveDevice() is Joystick)
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
    
    private void TriggerAbility()
    {
        if (!TryGetComponent<GrazeAbility>(out ability)) return;
        ability.ActivateAbility(IC.GetFire2() > 0f);
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
