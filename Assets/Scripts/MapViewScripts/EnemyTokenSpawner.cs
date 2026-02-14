using UnityEngine;

public class EnemyTokenSpawner : MonoBehaviour
{
    public GameObject m_enemyPrefab;
    public float spawnRadius = 2f;
    public bool m_spawnEnemy;
    
    void Start()
    {
        
    }

    
    void Update()
    {
        if (m_spawnEnemy)
        {
            GameObject new_enemy_token = Instantiate(m_enemyPrefab, this.transform);
            //move new_enemy_token out of spawn radius
            
        }
    }
}
