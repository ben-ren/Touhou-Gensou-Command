using UnityEngine;
using UnityEngine.InputSystem;

//Used to store useful functions for later
public class InputHandler : MonoBehaviour
{
    public InputActionAsset inputActionAsset;
    private InputAction _look;
    private InputAction _move;

    public PlayerController player;
    public GameObject crosshair;

    private Vector3 offset;
    private PlayerRoot root;

    private Rigidbody rb;
    public float followSpeed = 5f;
    public float rotateSpeed = 5f;
    private bool isCorrecting = false;
    private Vector3 defaultRotation;
    private Vector2 lastLookAmount = Vector2.zero;
    Vector2 lookAmount;
    public float correctionSpeed = 15f;
    public bool invertLook = false;


    void Start()
    {
        _look = inputActionAsset.FindAction("Look");
        _move = inputActionAsset.FindAction("Move");
        _look.Enable();

        offset = transform.position - player.transform.position;

        root = FindFirstObjectByType<PlayerRoot>();
        if (root == null)
            Debug.LogError("No PlayerRoot found in the scene!");
    }

    void Update()
    {
        Vector2 moveAmount = _move.ReadValue<Vector2>();
        lookAmount = _look.ReadValue<Vector2>();
        if (root.GetOnRailsMode())
        {

        }
        else
        {
            FreeMovementMode();
        }
    }

    void FreeMovementMode()
    {
        Quaternion yRotation = Quaternion.Euler(0f, player.transform.eulerAngles.y, 0f);
        Vector3 targetPos = player.transform.position + yRotation * offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);

        Vector3 direction = crosshair.transform.position - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
        }
    }

    private void MouseControl()
    {
        Vector3 screenPos = Mouse.current.position.ReadValue();
        screenPos.z = offset.magnitude;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        transform.position = worldPos;
    }

     private void IdleChecker(Vector2 lookAmount)
    {
        if (lookAmount == Vector2.zero && lastLookAmount == Vector2.zero)
            isCorrecting = true;
        else
            isCorrecting = false;

        lastLookAmount = lookAmount;

        if (isCorrecting)
            CorrectAngle();
    }

    private void CorrectAngle()
    {
        Vector3 current = rb.rotation.eulerAngles;
        float correctedZ = Mathf.LerpAngle(current.z, defaultRotation.z, correctionSpeed * Time.fixedDeltaTime);
        Quaternion target = Quaternion.Euler(current.x, current.y, correctedZ);
        rb.MoveRotation(target);
    }

    void GetMousePointOnSphere(Ray ray)
    {
        int layerMask = 1 << 10; // Layer 10 bitmask
        float maxDistance = 500f; // adjust as needed
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask))
        {
            Vector3 target = hit.point;
        }
    }

    void DebugRay(Ray ray)
    {
        // Draw a long visible line in the Scene view (yellow, 500 units)
        Debug.DrawRay(ray.origin, ray.direction * 500f, Color.yellow);

        // Optional: test hit
        if (Physics.Raycast(ray, out RaycastHit hit, 500f))
        {
            Debug.Log($"Hit {hit.collider.name} at {hit.point}");
            Debug.DrawLine(ray.origin, hit.point, Color.green); // draw green to hit point
        }
    }

    void MoveCrosshair()
    {
        if (invertLook) lookAmount.y *= -1f;
        lookAmount = _look.ReadValue<Vector2>();
        transform.RotateAround(player.transform.position, Vector3.up, lookAmount.x * rotateSpeed * Time.deltaTime);
        transform.RotateAround(player.transform.position, Vector3.left, lookAmount.y * rotateSpeed * Time.deltaTime);
        //clamp crosshair movement to within camera bounds
    }
    
    public void ToggleInvertLook() => invertLook = !invertLook;
}
