using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectileSpawner : MonoBehaviour
{
    [Header("Input Settings")]
    public InputActionAsset inputActionAsset;
    private InputAction _fire;

    [Header("Projectile")]
    public GameObject prefab;

    [Header("References")]
    private GameObject parent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _fire = inputActionAsset.FindAction("Player/PrimaryFire");
        _fire?.Enable();
        parent = gameObject.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        RepositionSpawner();
        FireProjectile();
    }

    //Move into position between crosshair & camera
    void RepositionSpawner()
    {
        
    }

    //BUG: Doesn't fire projectile from ProjectileSpawner
    //BUG: Doesn't fire projectile in direction of ProjectileSpawner. (Instantiate in same rotational direction as ProjectileSpawner)
    void FireProjectile()
    {
        if (_fire.WasPerformedThisFrame()){
            Instantiate(prefab, transform.position, transform.rotation); //(prefab object, new postion, new rotation, parent object, WorldSpace?:boolean)   
        }
    }
}