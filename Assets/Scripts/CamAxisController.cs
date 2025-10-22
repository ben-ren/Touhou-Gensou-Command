using Unity.VisualScripting;
using UnityEngine;

public class CamAxisController : MonoBehaviour
{
    public PlayerController player;
    public float rotateSpeed = 5f;      // Optional: how smoothly to rotate towards target
    private Rigidbody rb;
    private Vector3 initialPos;
    private Vector3 offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        initialPos = transform.position;
        offset = player.transform.position - initialPos;
    }

    // Update is called once per frame
    void Update()
    {
        // --- Move Position to allign with player ---
        rb.MovePosition(player.transform.position - offset); 

        // --- Slerp's MoveRotation to allign with the player's y-rotation ---
        Quaternion lookRotation = Quaternion.Euler(transform.eulerAngles.x, player.transform.eulerAngles.y, transform.eulerAngles.z);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, rotateSpeed * Time.fixedDeltaTime));
    }
}
