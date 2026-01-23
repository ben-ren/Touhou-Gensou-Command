using UnityEngine;
using UnityEngine.Splines;

//This script is used to control the map view character token's movement.
public class CharacterTokenController : MonoBehaviour
{
    public Spline splinePathToFollow;
    [SerializeField] private float moveSpeed;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    //Moves character token along path
    void FollowPath()
    {
        
    }

    //Rotates character token to face path direction, occurs instantly.
    void LookAlongPath()
    {
        
    }

    //Check's for enemy unit & item collision
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.name);
    }
}
