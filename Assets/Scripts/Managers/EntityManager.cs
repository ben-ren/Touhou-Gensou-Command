using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public TokenController[] playerTokens;
    public Enemy2D[] enemy2DTokens;
    public Item[] items;

    private void Awake()
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
