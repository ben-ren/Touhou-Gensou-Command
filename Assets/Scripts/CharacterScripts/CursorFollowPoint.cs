using UnityEngine;
using UnityEngine.InputSystem;

public class CursorFollowPoint : MonoBehaviour
{
    [SerializeField] private float cursorPadding = 0.0f; 
    [SerializeField] private float leftInset = 2f;
    [SerializeField] private float cursorRadius = 0.25f;
    [Header("Blocking Settings")]
    [SerializeField] private LayerMask blockingLayers; // mountains, walls, etc.
    private InputController IC;
    private Rigidbody2D rb2d;
    private Vector3 mousePos;

    void Start()
    {
        IC = InputController.instance;
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        FollowMouseCursor();
        if (IC.GetClickState() > 0)
        {
            
        }
        
    }

    //follow cursor position
    void FollowMouseCursor()
    {
        Camera cam = Camera.main;

        Vector3 mouse = Input.mousePosition;
        mouse.z = -cam.transform.position.z;

        mousePos = cam.ScreenToWorldPoint(mouse);

        float cameraHeight = cam.orthographicSize * 2f;
        float cameraWidth  = cameraHeight * cam.aspect;

        float minX = cam.transform.position.x - cameraWidth / 2f + cursorPadding;
        float maxX = cam.transform.position.x + cameraWidth / 2f - cursorPadding;
        float minY = cam.transform.position.y - cameraHeight / 2f + cursorPadding;
        float maxY = cam.transform.position.y + cameraHeight / 2f - cursorPadding;

        mousePos.x = Mathf.Clamp(mousePos.x, minX + leftInset, maxX);
        mousePos.y = Mathf.Clamp(mousePos.y, minY, maxY);
        mousePos.z = 0f;

        rb2d.MovePosition(mousePos);
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
