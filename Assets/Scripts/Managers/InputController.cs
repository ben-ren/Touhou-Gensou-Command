using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    public static InputController instance { get; private set;}

    [Header("Input Settings")]
    [SerializeField] private InputActionAsset inputActionAsset;

    private InputAction move;
    private InputAction look;
    private InputAction boost;
    private InputAction brake;
    private InputAction fire;
    private InputAction fire2;
    private InputAction bomb;
    private InputAction ui_click;
    InputDevice ActiveDevice;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);   // ← persists across scene loads

        var map = inputActionAsset.FindActionMap("Player");
        var map2 = inputActionAsset.FindActionMap("UI");
        move = map.FindAction("Move");
        look = map.FindAction("Look");
        boost = map.FindAction("Boost");
        brake = map.FindAction("Brake");
        fire = map.FindAction("PrimaryFire");
        fire2 = map.FindAction("SecondaryFire");
        bomb = map.FindAction("Bomb");
        ui_click = map2.FindAction("Click");
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
        fire2.Enable();
        bomb.Enable();
        ui_click.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        look.Disable();
        boost.Disable();
        brake.Disable();
        fire.Disable();
        fire2.Disable();
        bomb.Disable();
        ui_click.Disable();
    }

    public void UpdateActiveDevice() => ActiveDevice = look?.activeControl?.device;

    // --- GetValue() Style Accessors ---
    public Vector2 GetMove() => move.ReadValue<Vector2>();
    public Vector2 GetLook() => look.ReadValue<Vector2>();
    public float GetBoost() => boost.ReadValue<float>();
    public float GetBrake() => brake.ReadValue<float>();
    public float GetFire() => fire.ReadValue<float>();
    public float GetFire2() => fire2.ReadValue<float>();
    public float GetBomb() => bomb.ReadValue<float>();
    public float GetClickState() => ui_click.ReadValue<float>();
    public InputDevice GetActiveDevice() => ActiveDevice;
}
