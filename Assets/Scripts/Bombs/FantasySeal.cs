using UnityEngine;

public class FantasySeal : Bomb
{
    [Header("Seal Orb Settings")]
    [SerializeField] private GameObject sealOrbPrefab;
    [SerializeField] private int orbCount = 6;
    [SerializeField] private float initialOffset = 1.5f;
    [SerializeField] private float verticalOffset = 0f;
    [SerializeField] private float spawnDelay = 0.5f; // seconds between each orb

    private int nextOrbIndex = 0;
    private float nextFireTime = 0f;
    private float angleStep;

    public override void Start()
    {
        base.Start();
        if (sealOrbPrefab != null)
            angleStep = 360f / orbCount;
    }

    public override void Update()
    {
        base.Update();
        HandleOrbSpawning();
    }

    private void HandleOrbSpawning()
    {
        if (sealOrbPrefab == null) return;
        if (nextOrbIndex >= orbCount) return; // all orbs spawned
        if (Time.time < nextFireTime) return; // wait for next spawn

        float angle = nextOrbIndex * angleStep;
        SpawnSingleOrb(angle);

        nextOrbIndex++;
        nextFireTime = Time.time + spawnDelay;
    }

    private void SpawnSingleOrb(float angle)
    {
        Vector3 offset = CalculateOffset(angle);
        GameObject orb = Instantiate(sealOrbPrefab, transform.position + offset, Quaternion.identity);
        orb.transform.rotation = Quaternion.Euler(0f, angle, 0f);

        // Set the orb's team to the bomb's team
        Projectile proj = orb.GetComponent<Projectile>();
        if (proj != null)
            proj.SetTeam(TeamAlignment);
    }

    private Vector3 CalculateOffset(float angle)
    {
        return new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            verticalOffset,
            Mathf.Sin(angle * Mathf.Deg2Rad)
        ) * initialOffset;
    }
}
