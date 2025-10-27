using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject target;
    public float rotateSpeed = 5f;
    public float followSpeed = 5f;
    Vector3 offset;

    void Start()
    {
        offset = transform.position - target.transform.position;
    }

    void LateUpdate()
    {
        Debug.Log(offset);
        CameraFollow();
        //CameraRotate();
    }

    //Follow player position
    void CameraFollow()
    {
        transform.position = target.transform.position + offset;
    }

    //Rotate around player to match player rotation, with a delay (use Slerp).
    void CameraRotate()
    {
        Vector3 currentDir = transform.forward;
        Vector3 targetDir = (target.transform.position - transform.position).normalized;
        transform.Rotate(Vector3.Slerp(currentDir, targetDir, rotateSpeed * Time.deltaTime));

    }
}
