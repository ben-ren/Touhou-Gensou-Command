using UnityEngine;

public class EntitySystems : MonoBehaviour
{
    [Header("Entity Stats")]
    [SerializeField] private int health = 50;
    [SerializeField] private int power = 0;
    [SerializeField] private int graze = 0;
    [SerializeField] private int bombs = 0;

    public int GetHealth() => health;
    public int GetPower() => power;
    public int GetGraze() => graze;
    public int GetBombs() => bombs;

    public void SetHealth(int value) => health = value;
    public void SetPower(int value) => power = value;
    public void SetGraze(int value) => graze = value;
    public void SetBombs(int value) => bombs = value;

    public void ApplyDamage(float amount)
    {
        health -= Mathf.RoundToInt(amount);
        Debug.Log($"{gameObject.name} took {amount} damage! Remaining health: {health}");
    }

    public void Kill()
    {
        Debug.Log($"{gameObject.name} has been killed!");
    }
}
