using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    [SerializeField] private float fireRate = 1f; // shots per second
    [SerializeField] private Team team = Team.Enemy;

    [Header("Firing Settings")]
    public bool IsFiring = false; // true for continuous shooting
    [HideInInspector] public bool IsLaser = false;
    private GameObject activeLaser;

    [Header("Targeting Settings")]
    [SerializeField] private bool enableTargeting = false; // toggle on/off
    [SerializeField] private Transform target;            // optional target
    private float nextFireTime = 0f;

    void Update()
    {
        if (!IsFiring) return;
        
        Targeting();

        if (IsLaser)
        {
            FireLaser();
        }
        else if (!IsLaser && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + (1f / fireRate);
            FireProjectile();
        }
    }

    public void LaserCheck(bool shoot)
    {
        IsLaser = projectilePrefab.TryGetComponent(out Laser _);
        // Handle laser instantiation/destruction 
        if (IsLaser && shoot) return;

        if (!shoot && activeLaser != null){
            Destroy(activeLaser);
            IsLaser = false;
        }
    }

    void FireLaser()
    {
        if (activeLaser == null)
        {
            activeLaser = Instantiate(projectilePrefab, transform.position, transform.rotation, this.transform);
            activeLaser.GetComponent<IWeaponTeam>()?.SetTeam(team);
        }
    }

    void FireProjectile()
    {
        if (projectilePrefab == null) return;

        // Read spawn mode from the projectile prefab
        ProjectileSpawnMode mode = ProjectileSpawnMode.Global; // fallback
        if (projectilePrefab.TryGetComponent<ISpawnMode>(out var spawnSource))
            mode = spawnSource.SpawnMode;

        GameObject proj;

        // Instantiate based on mode
        if (mode == ProjectileSpawnMode.Attached)
            proj = Instantiate(projectilePrefab, transform.position, transform.rotation, this.transform);
        else
            proj = Instantiate(projectilePrefab, transform.position, transform.rotation);

        // Assign team to projectile
        proj.GetComponent<IWeaponTeam>()?.SetTeam(team);
    }

    void Targeting()
    {
        // Rotate firePoint toward target only if targeting is enabled
        if (enableTargeting && target != null)
        {
            transform.rotation = Quaternion.LookRotation(target.position - transform.position);
        }
    }

    public void SetProjectile(GameObject newProjectile)
    {
        if (newProjectile != null)
            projectilePrefab = newProjectile;
    }

    public void SetFireRate(float newFireRate)
    {
        if (newFireRate > 0f)
            fireRate = newFireRate;
    }

    public float GetFireRate() => fireRate;

    public GameObject GetProjectile() => projectilePrefab;
}
