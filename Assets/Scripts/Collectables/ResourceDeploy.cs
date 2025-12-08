using System;
using UnityEngine;

public class ResourceDeploy : MonoBehaviour
{
    [HideInInspector] public int health = 0;
    [HideInInspector] public int power = 0;
    [HideInInspector] public int graze = 0;
    [HideInInspector] public int fuel = 0;
    [HideInInspector] public int money = 0;
    [HideInInspector] public int bombs = 0;

    [SerializeField] private GameObject healthItem;
    [SerializeField] private GameObject powerItem;
    [SerializeField] private GameObject grazeItem;
    [SerializeField] private GameObject fuelItem;
    [SerializeField] private GameObject moneyItem;
    [SerializeField] private GameObject bombItem;

    void Update()
    {
        
    }

    public void DeployItems()
    {
        if (health > 0)
            ItemDeployLoop(healthItem, health);

        if (power > 0)
            ItemDeployLoop(powerItem, power);

        if (graze > 0)
            ItemDeployLoop(grazeItem, graze);

        if (fuel > 0)
            ItemDeployLoop(fuelItem, fuel);

        if (money > 0)
            ItemDeployLoop(moneyItem, money);

        if (bombs > 0)
            ItemDeployLoop(bombItem, bombs);
    }

    void ItemDeployLoop(GameObject currentItem, int currentItemVal)
    {
        for (int i = 0; i < currentItemVal; i++)
        {
            GameObject drop = Instantiate(currentItem, transform.position, Quaternion.identity);

            if (drop.TryGetComponent<Item>(out var item))
                item.ScatterFrom(transform.position);
            }
    }

    Vector3 RandomPositionRange(float range)
    {
        return new Vector3(
            UnityEngine.Random.Range(-range, range),
            UnityEngine.Random.Range(-range, range),
            UnityEngine.Random.Range(-range, range)
        );
    }
}
