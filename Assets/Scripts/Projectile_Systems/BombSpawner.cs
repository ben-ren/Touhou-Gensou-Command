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

        // Spawn bomb
        currentBomb = Instantiate(bombPrefab, transform.position, transform.rotation);

        // Assign the team to the projectile
        IWeaponTeam weapon = currentBomb.GetComponent<IWeaponTeam>();
        weapon?.SetTeam(team);

        return true;
    }

    public bool IsBombActive() => currentBomb != null;
    public void ClearCurrentBomb() => currentBomb = null;
}
