using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Input Settings")]
    public InputActionAsset input;
    private InputAction _move;
    private InputAction _look;

    [Header("Movement Settings")]
    public float thrust = 5f;
    public float moveSpeed = 2f;
    public float rotateSpeed = 150f;

    [Header("Idle Correction Settings")]
    [SerializeField] private float correctionSpeed = 15f;

    [Header("Control Settings")]
    [SerializeField] private bool invertLook = false; // toggle for inverted look

    private Rigidbody rb;
    private Vector2 moveAmount;
    private Vector2 lookAmount;
    private Vector3 moveDirection;
    private Vector3 defaultRotation;

    private Vector2 lastLookAmount = Vector2.zero;
    private bool isCorrecting = false;

    // ----------------------------------------------------------

    private void OnEnable()
    {
        input.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        input.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        _move = InputSystem.actions.FindAction("Move");
        _look = InputSystem.actions.FindAction("Look");
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveDirection = Vector3.forward;
        defaultRotation = transform.rotation.eulerAngles;
    }

    private void FixedUpdate()
    {
        moveAmount = _move.ReadValue<Vector2>();
        lookAmount = _look.ReadValue<Vector2>();

        // Apply inversion toggle
        if (invertLook)
            lookAmount.y *= -1f;
        else
            lookAmount.y *= 1f;

        PlayerMovement(moveAmount);
        PlayerRotation(lookAmount);
        IdleChecker();
    }

    // ----------------------------------------------------------

    private void PlayerMovement(Vector2 moveAxis)
    {
        moveDirection = new Vector3(moveAxis.x * moveSpeed, moveAxis.y * moveSpeed, thrust);
        rb.MovePosition(rb.position + (transform.rotation * moveDirection) * Time.fixedDeltaTime);
    }

    private void PlayerRotation(Vector2 rotateAxis)
    {
        Vector3 newRot = new Vector3(-rotateAxis.y, rotateAxis.x, 0) * rotateSpeed * Time.fixedDeltaTime;
        Quaternion deltaRot = Quaternion.Euler(newRot);

        rb.MoveRotation(Quaternion.RotateTowards(
            rb.rotation,
            rb.rotation * deltaRot,
            rotateSpeed * Time.fixedDeltaTime
        ));
    }

    private void IdleChecker()
    {
        // Detect if the mouse (look input) has stopped moving
        if (lookAmount == Vector2.zero && lastLookAmount == Vector2.zero)
        {
            isCorrecting = true;
        }
        else
        {
            isCorrecting = false;
        }

        lastLookAmount = lookAmount;

        if (isCorrecting)
            CorrectAngle();
    }

    private void CorrectAngle()
    {
        Vector3 currentRotation = rb.rotation.eulerAngles;

        // Gradually return Z (roll) toward the default level
        float correctedZ = Mathf.LerpAngle(currentRotation.z, defaultRotation.z, correctionSpeed * Time.fixedDeltaTime);

        Vector3 targetEuler = new Vector3(currentRotation.x, currentRotation.y, correctedZ);
        Quaternion targetRotation = Quaternion.Euler(targetEuler);

        rb.MoveRotation(targetRotation);
    }

    // ----------------------------------------------------------

    // Public method to toggle inversion at runtime
    public void ToggleInvertLook()
    {
        invertLook = !invertLook;
    }
}
