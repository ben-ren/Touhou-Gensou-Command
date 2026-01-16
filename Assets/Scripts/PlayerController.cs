using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Inputs")]
    private InputController IC;
    private GrazeAbility ability;
    private Rigidbody rb;
    private SettingsData settings;

    [Header("Movement Settings")]
    [SerializeField] private float panSpeed = 2f;
    [SerializeField] private float thrust = 5f;
    [SerializeField] private float boostSpeed = 10f;
    [SerializeField] private float brakeSpeed = 1f;
    private float oldThrust;

    [Header("Aim Controls")]  
    [SerializeField] private float mouseSensitivity = 40f;
    [SerializeField] private float gamepadSensitivity = 50f;
    [SerializeField] private bool inverted;
    [SerializeField] private bool onRailsMode;
    
    [HideInInspector] public bool rotationOverride = false;
    private float lookSensitivity;
    private float joystickSensitivity;

    private Vector3 moveDirection;
    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private Quaternion smoothRotation;
    private Vector2 lastLookInput;
    private float rotationY;
    private float rotationX;

    void Start()
    {
        initialRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
        rb.MoveRotation(initialRotation);
        oldThrust = thrust;
        
        IC = InputController.instance;

        settings = SettingsStorage.Load();

        ApplySettings();
    }

    void FixedUpdate()
    {
        HandleMovement();
        TriggerAbility();
        HandleRotation();
    }

    //=============================================
    //              Helper functions
    //=============================================

    //Player Movement logic
    void HandleMovement()
    {
        if (!onRailsMode)
            ThrustHandler(IC.GetBoost(), IC.GetBrake());
        else
            PlayerMovement();
    }

    //Player rotation logic
    void HandleRotation()
    {
        if (rotationOverride) return;
        PlayerRotation();
    }

    //=============================================
    //              Control settings load
    //=============================================

    private void ApplySettings()
    {
        lookSensitivity = settings.mouseSensitivity * 20f;

        joystickSensitivity = settings.joystickSensitivity * 20f;
    }

    public void RefreshSettings()
    {
        settings = SettingsStorage.Load();
        ApplySettings();

        // Force rotation math to immediately use new sensitivity
        lastLookInput = Vector2.zero;
    }

    //=============================================
    //              Entity Movement
    //=============================================

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
        rb.MovePosition(rb.position + rb.rotation * moveDirection * thrust * Time.fixedDeltaTime);
    }

    //
    private void PlayerMovement()
    {
        Vector2 moveAmount = IC.GetMove();
        
        moveDirection = new Vector3(moveAmount.x, moveAmount.y, 1).normalized;
        rb.MovePosition(rb.position + rb.rotation * moveDirection * panSpeed * Time.fixedDeltaTime);
    }

    private void TriggerAbility()
    {
        if (!TryGetComponent<GrazeAbility>(out ability)) return;
        ability.ActivateAbility(IC.GetFire2() > 0f);
    }

    //=============================================
    //              Player Rotation
    //=============================================
    private void PlayerRotation()
    {
        Vector2 lookAmount = IC.GetLook();

        if (!inverted) lookAmount.y *= -1f;

        switch (settings.controlSchemeIndex)
        {
            case 0: // Relative Input (old method)
            default:
                RotateRelativeInput(lookAmount);
                return;
            case 1: // Cursor Follow
                RotateTowardsCursor();
                return;
        }
    }

    //Default rotation method
    private void RotateRelativeInput(Vector2 lookAmount)
    {
        // Check if input is joystick
        if (IC.GetActiveDevice() is Gamepad || IC.GetActiveDevice() is Joystick)
            lookAmount *= joystickSensitivity;

        // rotation angles
        rotationY += lookAmount.x * lookSensitivity * Time.fixedDeltaTime;
        rotationX += lookAmount.y * lookSensitivity * Time.fixedDeltaTime;

        RotationRangeClamp();

        targetRotation = Quaternion.Euler(rotationX, rotationY, 0f);
        smoothRotation = Quaternion.Slerp(rb.rotation, targetRotation, lookSensitivity * Time.fixedDeltaTime);
        rb.MoveRotation(smoothRotation);
    }

    private void RotateTowardsCursor()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        // 1 Get mouse position in screen space
        Vector2 mousePos = Mouse.current.position.ReadValue();

        // 2 Create a ray from the camera through the mouse position
        Ray ray = cam.ScreenPointToRay(mousePos);

        // 3 set fixed distance along ray (adjust as needed)
        float rayDistance = 40f;

        // 3 Choose a fixed distance along the ray
        Vector3 targetPoint = ray.origin + ray.direction * rayDistance;

        // 4 Compute direction from player to target point
        Vector3 direction = (targetPoint - rb.position).normalized;

        // 5 Compute target rotation for player
        targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        // 6 Smoothly rotate player towards target
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, lookSensitivity * Time.fixedDeltaTime));

        // 7 Update internal rotation values so relative input still works
        Vector3 euler = targetRotation.eulerAngles;
        rotationY = euler.y;
        rotationX = euler.x;
    }




    //=============================================
    //              Camera Logic
    //=============================================

    void RotationRangeClamp()
    {
        if (rotationOverride) return;

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

    //=============================================
    //              Collision
    //=============================================

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("boundary") || collision.gameObject.CompareTag("Level_geometry"))
        {
            Vector3 wallNormal = collision.contacts[0].normal;

            // Reflect player forward vector across wall normal
            Vector3 reflectedDir = Vector3.Reflect(transform.forward, wallNormal).normalized;

            // Compute target yaw
            float targetY = Mathf.Atan2(reflectedDir.x, reflectedDir.z) * Mathf.Rad2Deg;

            // Keep pitch the same
            float targetX = rotationX;

            // Apply player rotation smoothly via Slerp coroutine
            Quaternion targetRot = Quaternion.Euler(targetX, targetY, 0f);
            StartCoroutine(SlerpRotation(rb.rotation, targetRot, 0.3f));

            // Update internal rotation values
            rotationY = targetY;
            rotationX = targetX;
        }
    }

    //=============================================
    //              Timed automove logic
    //=============================================

    private IEnumerator SlerpRotation(Quaternion from, Quaternion to, float duration)
    {
        rotationOverride = true;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.fixedDeltaTime / duration;
            rb.MoveRotation(Quaternion.Slerp(from, to, t));
            yield return new WaitForFixedUpdate();
        }

        rb.MoveRotation(to);
        rotationOverride = false;
    }
}