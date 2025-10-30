using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectileSpawner : MonoBehaviour
{
    [Header("Input Settings")]
    public InputActionAsset inputActionAsset;
    private InputAction _fire;

    [Header("References")]
    public Transform crosshair;
    public Transform CameraRig;

    [Header("Projectile")]
    public GameObject prefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _fire = inputActionAsset.FindAction("Player/PrimaryFire");
        _fire?.Enable();
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
        transform.position = (CameraRig.position + crosshair.position) * 0.5f;
    }

    //BUG: Doesn't fire projectile from ProjectileSpawner
    //BUG: Doesn't fire projectile in direction of ProjectileSpawner. (Instantiate in same rotational direction as ProjectileSpawner)
    void FireProjectile()
    {
        if (_fire.WasPerformedThisFrame()){
            Instantiate(prefab);    
        }
    }
}