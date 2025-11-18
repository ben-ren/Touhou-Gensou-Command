using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class Laser : MonoBehaviour, ITeamMember, IWeaponTeam
{
    [Header("Laser Settings")]
    public float laserDamage = 1f;
    public float laserTickRate = .15f; 
    public float laserRange = 30f;
    public float laserLifeSpan = 10f;
    public float laserWidth = 1f;
    public bool penetrateEnemies = false;

    [Header("Team Assignment")]
    [SerializeField] private Team team = Team.Neutral;
    public Team TeamAlignment => team;

    [Header("Spawn Settings")]
    [SerializeField] private ProjectileSpawnMode spawnMode = ProjectileSpawnMode.Global;

    private Transform beam; // child cylinder
    EntitySystems entity = null;
    private float nextTick;

    public void SetTeam(Team newTeam) => team = newTeam;

    public virtual void Start()
    {
        // Get child cylinder
        beam = transform.GetChild(0);
        Destroy(gameObject, laserLifeSpan);

        if (spawnMode == ProjectileSpawnMode.Attached && transform.parent != null)
            transform.localPosition = Vector3.zero;
    }

    public virtual void Update()
    {
        BeamWidth();
        FireLaser();
    }

    void FireLaser()
    {
        Vector3 start = transform.position;
        Vector3 direction = transform.forward;
        Vector3 end = start + direction * laserRange;

        RaycastHit[] hits = Physics.RaycastAll(start, direction, laserRange);

        // Sort by distance
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit hit in hits)
        {
            // Stop at geometry
            if (hit.collider.CompareTag("Level_geometry"))
            {
                end = hit.point;
                break; // laser stops at level geometry
            }

            // Skip non-entities
            if (!hit.collider.TryGetComponent(out EntitySystems entity))
                continue;

            // Skip friendly or neutral entities
            if (entity.TeamAlignment == team)
                continue;

            // Damage enemy
            if (Time.time >= nextTick)
            {
                nextTick = Time.time + 1f / laserTickRate;
                entity.ApplyDamage(laserDamage);
            }

            end = hit.point;

            // Stop if not penetrating enemies
            if (!penetrateEnemies)
                break;
        }

        // Position beam at midpoint
        Vector3 midPoint = (start + end) * 0.5f;
        beam.position = midPoint;

        // Rotate beam to face target
        beam.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90f, 0f, 0f);

        // Scale Y for length
        float distance = Vector3.Distance(start, end);
        Vector3 beamScale = beam.localScale;
        beamScale.y = distance * 0.5f;
        beam.localScale = beamScale;
    }

    void BeamWidth()
    {
        Vector3 beamScale = beam.localScale;
        beamScale.x = laserWidth * 0.5f;
        beamScale.z = laserWidth * 0.5f;
        beam.localScale = beamScale;
    }

}
