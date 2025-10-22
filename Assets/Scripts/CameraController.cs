using UnityEngine;

/*
* Rotate's Camera to point at a target
*/
public class CameraController : MonoBehaviour
{
    public GameObject target;           // The object the camera should look at
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        transform.LookAt(target.transform);
    }
    
}
