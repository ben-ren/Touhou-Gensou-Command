using UnityEngine;

public class BombSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private Team team = Team.Neutral; // default team
    private GameObject currentBomb;

    public bool TrySpawnBomb(out float bombLifespan)
    {
        bombLifespan = 0f;

        if (currentBomb != null || bombPrefab == null)
            return false;

        // Read spawn mode from the bomb prefab
        ProjectileSpawnMode mode = ProjectileSpawnMode.Global; // fallback
        if (bombPrefab.TryGetComponent<ISpawnMode>(out var spawnSource))
            mode = spawnSource.SpawnMode;

        // Instantiate bomb according to spawn mode
        if (mode == ProjectileSpawnMode.Attached)
            currentBomb = Instantiate(bombPrefab, transform.position, transform.rotation, transform);
        else
            currentBomb = Instantiate(bombPrefab, transform.position, transform.rotation);

        // Assign team
        currentBomb.GetComponent<IWeaponTeam>()?.SetTeam(team);

        return true;
    }

    public bool IsBombActive() => currentBomb != null;
    public void ClearCurrentBomb() => currentBomb = null;
}
