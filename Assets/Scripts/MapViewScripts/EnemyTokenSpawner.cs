using UnityEngine;

public class EnemyTokenSpawner : Enemy2D
{
    public GameObject m_enemyPrefab;
    public GameObject defaultTarget;
    public float spawnRadius = 2f;
    public int turnsBetweenSpawns = 0;  //spawning disabled if value less than 1
    protected bool m_spawnEnemy;   //Make this trigger after preset number of turns
    int lastSpawnTurn = -1;
    
    void Update()
    {
        int turn = GameState.Instance.Data.turnNumber;

        if (turnsBetweenSpawns > 0 &&
            turn % turnsBetweenSpawns == 0 &&
            turn != lastSpawnTurn &&
            turn > 0
        ){
            m_spawnEnemy = true;
            lastSpawnTurn = turn;
        }

        if (m_spawnEnemy)
        {
            GameObject new_enemy_token = Instantiate(m_enemyPrefab, this.transform);
            SetTokenDetails(new_enemy_token);
            
            Debug.Log("Token spawn");
            m_spawnEnemy = false;
        }
        
    }

    //move new_enemy_token out of spawn radius
    void SetTokenDetails(GameObject token)
    {
        Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
        token.transform.position = (Vector2)transform.position + randomOffset;

        if (token.TryGetComponent(out AI_EnemyToken enemy))
        {
            enemy.defaultTarget = defaultTarget;
        }
    }
}
