using UnityEngine;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour
{
    public static UIController instance { get; private set;}

    [Header("Input Settings")]
    [SerializeField] private InputActionAsset inputActionAsset;
    private InputAction pause;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);   // ← persists across scene loads

        var map = inputActionAsset.FindActionMap("UI");
        pause = map.FindAction("StartButton");
    }

    void Update()
    {
        
    }

    private void OnEnable()
    {
        pause.Enable();
    }

    private void OnDisable()
    {
        pause.Disable();
    }

    public float GetPause() => pause.ReadValue<float>();
}
