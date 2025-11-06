using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private InputActionAsset inputActionAsset;

    private InputAction move;
    private InputAction look;
    private InputAction boost;
    private InputAction brake;
    private InputAction fire;
    InputDevice ActiveDevice;

    private void Awake()
    {
        var map = inputActionAsset.FindActionMap("Player");
        move = map.FindAction("Move");
        look = map.FindAction("Look");
        boost = map.FindAction("Boost");
        brake = map.FindAction("Brake");
        fire = map.FindAction("PrimaryFire");
    }

    private void Update()
    {
        UpdateActiveDevice();
    }

    private void OnEnable()
    {
        move.Enable();
        look.Enable();
        boost.Enable();
        brake.Enable();
        fire.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        look.Disable();
        boost.Disable();
        brake.Disable();
        fire.Disable();
    }

    public void UpdateActiveDevice() => ActiveDevice = look?.activeControl?.device;

    // --- GetValue() Style Accessors ---
    public Vector2 GetMove() => move.ReadValue<Vector2>();
    public Vector2 GetLook() => look.ReadValue<Vector2>();
    public float GetBoost() => boost.ReadValue<float>();
    public float GetBrake() => brake.ReadValue<float>();
    public float GetFire() => fire.ReadValue<float>();
    public InputDevice GetActiveDevice() => ActiveDevice;
}
