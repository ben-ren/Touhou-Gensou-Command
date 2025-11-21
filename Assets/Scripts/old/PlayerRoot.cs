using UnityEngine;

public class PlayerRoot : MonoBehaviour
{
    public float moveSpeed = 5f;
    public bool OnRailsMode = false;

    void Update()
    {
        if (OnRailsMode)
            transform.position = NewPosition();
    }

    public bool GetOnRailsMode() => OnRailsMode;

    private Vector3 NewPosition()
    {
        return transform.position + Vector3.forward * moveSpeed * Time.deltaTime;
    }

    void FollowPath()
    {
        
    }
}
