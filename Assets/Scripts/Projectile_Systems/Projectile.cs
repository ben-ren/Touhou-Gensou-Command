using UnityEngine;

public class Projectile : MonoBehaviour, ITeamMember
{
    [Header("Projectile Stats")]
    public float projectileDamage = 5f;
    public float projectileSpeed = 10f;
    public float projectileLifeSpan = 5f;

    [Header("Team Assignment")]
    [SerializeField] private Team team = Team.Neutral;
    public Team TeamAlignment => team;

    [Header("Resources")]
    [SerializeField] private int grazePoints = 0;
    
    [Header("Spawn Settings")]
    [SerializeField] private ProjectileSpawnMode spawnMode = ProjectileSpawnMode.Global;
    public ProjectileSpawnMode SpawnMode => spawnMode;

    public int GetGrazePoints() => grazePoints;
    public void SetGrazePoints(int value) => grazePoints = value;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        // Ensure Rigidbody is kinematic
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        // Ensure Collider is trigger (impact detection)
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        //Destory after set period of time
        Destroy(gameObject, projectileLifeSpan);
    }

    // Update is called once per frame
    public virtual void Update(){}

    public float GetProjectileDamage() => projectileDamage;
    public float GetProjectileSpeed() => projectileSpeed;
    public float GetProjectileLifeSpan() => projectileLifeSpan;
    public void SetTeam(Team newTeam) => team = newTeam;

    void OnTriggerEnter(Collider other)
    {
        EntitySystems entity = other.GetComponent<EntitySystems>();

        if (other.CompareTag("HomingSystem") || (entity != null && entity.TeamAlignment == team)) return;

        if (entity != null)
        {
            if (entity.TeamAlignment != team)
            {
                entity.ApplyDamage(GetProjectileDamage());
                Destroy(gameObject);
            }
        }
    }
}
