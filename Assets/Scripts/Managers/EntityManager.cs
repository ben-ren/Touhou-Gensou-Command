using UnityEngine;
using UnityEngine.SceneManagement;

public class EntityManager : MonoBehaviour
{
    public TokenController[] playerTokens;
    public Enemy2D[] enemy2DTokens;
    public Item[] items;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        playerTokens = FindObjectsByType<TokenController>(FindObjectsSortMode.None);
        enemy2DTokens = FindObjectsByType<Enemy2D>(FindObjectsSortMode.None);
        items = FindObjectsByType<Item>(FindObjectsSortMode.None);

        GameState.Instance.Data.totalRequiredOrbs = CalculateOrbCount();
    }

    public void Update()
    {
        //
    }

    int CalculateOrbCount()
    {
        int orbCount = 0;

        foreach (var enemy in enemy2DTokens)
        {
            orbCount += enemy.orbCount;
        }

        return orbCount;
    }
}
