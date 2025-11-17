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

        Bomb bomb = currentBomb.GetComponent<Bomb>();
        if (bomb != null)
        {
            bomb?.SetTeam(team);
            bombLifespan = bomb.projectileLifeSpan;
        }

        return true;
    }

    public bool IsBombActive() => currentBomb != null;
    public void ClearCurrentBomb() => currentBomb = null;
}
