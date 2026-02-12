using UnityEngine;
using UnityEngine.InputSystem;

public class CursorFollowPoint : MonoBehaviour
{
    [SerializeField] private float cursorPadding = 0.0f; 
    [SerializeField] private float leftInset = 2f;
    
    private InputController IC;
    private SettingsData settings;
    private Rigidbody2D rb2d;
    Camera cam;
    private Vector3 mousePos;
    private float joystickSensitivity;

    void Start()
    {
        IC = InputController.instance;
        rb2d = GetComponent<Rigidbody2D>();
        settings = SettingsStorage.Load();
        cam = Camera.main;
        joystickSensitivity = settings.joystickSensitivity * 20f;
    }

    void FixedUpdate()
    {
        switch (settings.controlSchemeIndex)
        {
            case 0:
                JoystickMovesCursor();
                return;
            case 1:
            default:
                FollowMouseCursor();
                return;
        }
    }

    //follow cursor position
    void FollowMouseCursor()
    {
        Vector3 mouse = Input.mousePosition;
        mouse.z = -cam.transform.position.z;

        mousePos = cam.ScreenToWorldPoint(mouse);

        rb2d.MovePosition(ClampToCameraBounds(mousePos));
    }

    void JoystickMovesCursor()
    {
        Vector2 input = IC.GetMove();

        Vector2 delta = input * joystickSensitivity * Time.fixedDeltaTime;

        Vector2 newPos = rb2d.position + delta;

        rb2d.MovePosition( ClampToCameraBounds(newPos));
    }

    Vector3 ClampToCameraBounds(Vector3 position)
    {
        float cameraHeight = cam.orthographicSize * 2f;
        float cameraWidth  = cameraHeight * cam.aspect;

        float minX = cam.transform.position.x - cameraWidth / 2f + cursorPadding;
        float maxX = cam.transform.position.x + cameraWidth / 2f - cursorPadding;
        float minY = cam.transform.position.y - cameraHeight / 2f + cursorPadding;
        float maxY = cam.transform.position.y + cameraHeight / 2f - cursorPadding;

        position.x = Mathf.Clamp(position.x, minX + leftInset, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);
        position.z = 0f;

        return position;
    }

    //Called in SettingsMenu script
    public void RefreshSettings()
    {
        settings = SettingsStorage.Load();
        joystickSensitivity = settings.joystickSensitivity * 20f;
    }

    // --- Hover detection ---
    void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("Hovered over: " + other.gameObject.name);
    }

    void OnCollisionExit2D(Collision2D other)
    {
        Debug.Log("Stopped hovering: " + other.gameObject.name);
    }
}
