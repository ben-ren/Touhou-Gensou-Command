using UnityEngine;

public class BombSpawner : MonoBehaviour
{
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private Team team = Team.Neutral;
    private GameObject currentBomb;

    public bool TrySpawnBomb()
    {
        if (currentBomb != null || bombPrefab == null)
            return false;

        ProjectileSpawnMode mode = ProjectileSpawnMode.Global;
        if (bombPrefab.TryGetComponent<ISpawnMode>(out var spawnSource))
            mode = spawnSource.SpawnMode;

        if (mode == ProjectileSpawnMode.Attached)
            currentBomb = Instantiate(bombPrefab, transform.position, transform.rotation, transform);
        else
            currentBomb = Instantiate(bombPrefab, transform.position, transform.rotation);

        currentBomb.GetComponent<IWeaponTeam>()?.SetTeam(team);

        return true;
    }

    // Only true if the bomb still exists in the scene
    public bool IsBombActive() => currentBomb != null;

    public void ClearCurrentBomb() => currentBomb = null;
}
