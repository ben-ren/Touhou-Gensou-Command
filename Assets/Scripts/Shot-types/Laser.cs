using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class Laser : MonoBehaviour, ITeamMember, IWeaponTeam, ISpawnMode, IGrazable
{
    [Header("Laser Settings")]
    public float laserDamage = 1f;
    public float laserTickRate = .15f; 
    public float laserRange = 30f;
    public float laserLifeSpan = 10f;
    public float laserWidth = 1f;
    public bool penetrateEnemies = false;

    [Header("Resources")]
    [SerializeField] private int grazePoints = 0;

    [Header("Team Assignment")]
    [SerializeField] private Team team = Team.Neutral;
    public Team TeamAlignment => team;

    [Header("Spawn Settings")]
    [SerializeField] private ProjectileSpawnMode spawnMode = ProjectileSpawnMode.Global;
    public ProjectileSpawnMode SpawnMode => spawnMode;

    [Header("Audio")]
    [SerializeField] private AudioClip laserSoundClip;

    // Components
    private Transform beam;         // child cylinder
    private BoxCollider hitbox;     // Real damage hit area
    EntitySystems entity = null;    // Reference to hit entity
    private float nextTick = 0f;

    Vector3 start;
    Vector3 direction;
    Vector3 end;

    public int GetGrazePoints() => grazePoints;
    public void SetGrazePoints(int value) => grazePoints = value;
    public void SetTeam(Team newTeam) => team = newTeam;

    public virtual void Start()
    {
        beam = transform.GetChild(0);   // Get child cylinder
        hitbox = GetComponent<BoxCollider>(); // gameplay hitbox
        hitbox.isTrigger = true;

        Destroy(gameObject, laserLifeSpan);

        SFXManager.instance.PlaySFXClip(laserSoundClip,transform,1f);

        if (spawnMode == ProjectileSpawnMode.Attached && transform.parent != null)
            transform.localPosition = Vector3.zero;
    }

    public virtual void Update()
    {
        start = transform.position;
        direction = transform.forward;
        end = start + direction * laserRange;
        
        FireBeam();
    }

    private void FireBeam()
    {
        CalcBeamEndFromRaycast();
        ResizeBeam();
        UpdateHitbox();
    }

    private void CalcBeamEndFromRaycast()
    {
        if (Physics.Raycast(start, direction, out RaycastHit hit, laserRange))
        {
            if (hit.collider.CompareTag("Level_geometry"))
                end = hit.point;
        }
    }

    private void ResizeBeam()
    {
        float length = Vector3.Distance(start, end);

        // mid-point positioning
        beam.position = (start + end) * 0.5f;

        // rotate to face forward
        beam.rotation = Quaternion.LookRotation(direction) * Quaternion.Euler(90, 0, 0);

        // scale visual beam
        Vector3 scale = beam.localScale;
        scale.y = length * 0.5f;
        scale.x = laserWidth * 0.5f;
        scale.z = laserWidth * 0.5f;
        beam.localScale = scale;
    }

    private void UpdateHitbox()
    {
        float length = Vector3.Distance(start, end);

        // collider size
        hitbox.size = new Vector3(laserWidth, laserWidth, length);

        // offset so the collider extends forward
        hitbox.center = new Vector3(0, 0, length * 0.5f);
    }

    private void OnTriggerStay(Collider other)
    {
        if (Time.time < nextTick)
            return;

        if (!other.TryGetComponent(out entity))
            return;

        // ignore friendly/neutral
        if (entity.TeamAlignment == team)
            return;

        // apply tick
        nextTick = Time.time + 1f / laserTickRate;
        entity.ApplyDamage(laserDamage, true);
    }
}
