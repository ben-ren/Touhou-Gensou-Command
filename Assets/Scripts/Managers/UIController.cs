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

    public void ExecuteTurn()
    {
        // Start player token movement
        DrawPathGenerator[] paths = FindObjectsByType<DrawPathGenerator>(FindObjectsSortMode.None);

        foreach (var p in paths)
        {
            p.StartTokenMovement();
        }

        // Start enemy tokens
        AI_EnemyToken[] enemies = FindObjectsByType<AI_EnemyToken>(FindObjectsSortMode.None);

        foreach (var enemy in enemies)
        {
            enemy.startMovement = true;
        }
    }

    public float GetPause() => pause.ReadValue<float>();
}
