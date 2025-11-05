using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectileSpawner : MonoBehaviour
{
    [Header("Input Settings")]
    public InputActionAsset inputActionAsset;
    private InputAction _fire;
    private InputAction _brake;

    [Header("Projectile")]
    public GameObject primaryPrefab;
    public GameObject secondaryPrefab;
    private GameObject projectile;

    [Header("Firing Settings")]
    [SerializeField] private float primaryFireRate = 1f; // shots per second for primary fire
    [SerializeField] private float secondaryFireRate = 5f; // shots per second for secondary fire (focus mode)
    [SerializeField] private bool automaticFiring; // Applies continuous firing without input
    private float fireRate = 5f; // shots per second
    private float nextFireTime = 0f;              // internal timer

    [Header("Team Alignment")]
    [SerializeField] private Team spawnerTeam = Team.Player;
    private GameObject parent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _fire = inputActionAsset.FindAction("Player/PrimaryFire");
        _fire?.Enable();
        _brake = inputActionAsset.FindAction("Brake");

        parent = gameObject.transform.parent.gameObject;
        projectile = primaryPrefab; // default mode
    }

    // Update is called once per frame
    void Update()
    {
        float fireInput = _fire.ReadValue<float>();

        SwitchFiringModes();

        if (automaticFiring ^ fireInput > 0)
        {
            FireProjectile();
        }
    }

    void SwitchFiringModes()
    {
        float brakeValue = _brake.ReadValue<float>();
        projectile = brakeValue > 0 ? secondaryPrefab : primaryPrefab;
        fireRate = brakeValue > 0 ? secondaryFireRate : primaryFireRate;
    }

    void FireProjectile()
    {
        if (Time.time >= nextFireTime){
            nextFireTime = Time.time + (1f / fireRate); // set next available fire time
            GameObject proj = Instantiate(projectile, transform.position, transform.rotation);

            //Assign team to projectile
            Projectile projectileScript = proj.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.SetTeam(spawnerTeam);
            }
        }
    }
}