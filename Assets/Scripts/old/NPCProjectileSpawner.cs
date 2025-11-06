using UnityEngine;

public class NPCProjectileSpawner : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    [SerializeField] private float fireRate = 1f; // shots per second
    [SerializeField] private Team npcTeam = Team.Enemy;

    [Header("Firing Settings")]
    [SerializeField] private bool autoFire = true; // true for continuous shooting

    [Header("Targeting Settings")]
    [SerializeField] private bool enableTargeting = false; // toggle on/off
    [SerializeField] private Transform target;            // optional target

    private float nextFireTime = 0f;

    void Update()
    {
        if (!autoFire) return;

        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + (1f / fireRate);
            FireProjectile();
        }
    }

    void FireProjectile()
    {
        if (projectilePrefab == null) return;

        // Rotate firePoint toward target only if targeting is enabled
        if (enableTargeting && target != null)
        {
            transform.rotation = Quaternion.LookRotation(target.position - transform.position);
        }

        GameObject proj = Instantiate(projectilePrefab, transform.position, transform.rotation);

        // Assign the NPC team to the projectile
        Projectile projectileScript = proj.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.SetTeam(npcTeam);
        }
    }
}
