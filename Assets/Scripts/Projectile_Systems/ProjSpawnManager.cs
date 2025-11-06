using System.Collections.Generic;
using UnityEngine;

public class ProjSpawnManager : MonoBehaviour
{
    [Header("Spawner Settings")]
    [Tooltip("All ProjectileSpawner children are automatically registered.")]
    [SerializeField] protected List<ProjectileSpawner> spawners = new List<ProjectileSpawner>();

    [Header("Spawner Control")]
    [Tooltip("Floating point value that determines how many spawners are active.")]
    public float power = 0; // 0 = first spawner active, 3 = all 4 active, etc.

    private void Start()
    {
        // Automatically find and register all ProjectileSpawner components in children
        spawners.Clear();
        spawners.AddRange(GetComponentsInChildren<ProjectileSpawner>(true));
    }

    protected virtual void Update()
    {
        // Determine how many spawners should be active
        int activeCount = Mathf.Clamp(Mathf.FloorToInt(power) + 1, 1, spawners.Count);

        for (int i = 0; i < spawners.Count; i++)
        {
            if (spawners[i] != null)
            {
                bool isActive = i < activeCount; // only the first N spawners are active
                spawners[i].IsFiring = isActive;
                spawners[i].enabled = isActive; // optional: disable update if not active
            }
        }
    }

    /// <summary>
    /// Manually set a specific number of spawners active
    /// </summary>
    public void SetActiveSpawners(int count)
    {
        count = Mathf.Clamp(count, 1, spawners.Count);

        for (int i = 0; i < spawners.Count; i++)
        {
            if (spawners[i] != null)
            {
                bool isActive = i < count;
                spawners[i].IsFiring = isActive;
                spawners[i].enabled = isActive;
            }
        }
    }

    /// <summary>
    /// Add a new spawner at runtime (optional)
    /// </summary>
    public void RegisterSpawner(ProjectileSpawner spawner)
    {
        if (spawner != null && !spawners.Contains(spawner))
        {
            spawners.Add(spawner);
        }
    }

    public void SetAllProjectiles(GameObject newProjectile)
    {
        foreach (var spawner in spawners)
        {
            if (spawner != null)
                spawner.SetProjectile(newProjectile);
        }
    }

    /// <summary>
    /// Returns the list of currently registered spawners
    /// </summary>
    public List<ProjectileSpawner> GetSpawners() => spawners;
}