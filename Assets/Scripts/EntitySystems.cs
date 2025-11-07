using UnityEngine;

public class EntitySystems : MonoBehaviour, ITeamMember
{
    [Header("Entity Stats")]
    [SerializeField] private int health = 50;
    [SerializeField] private int power = 0;
    [SerializeField] private int graze = 0;
    [SerializeField] private int bombs = 0;

    [Header("Team Alignment")]
    [SerializeField] private Team team = Team.Neutral;
    public Team TeamAlignment => team;

    [Header("Resources")]
    [SerializeField] private int grazePoints = 0;

    public bool RecentlyDamaged { get; private set; } = false;
    
    public int GetHealth() => health;
    public int GetPower() => power;
    public int GetGraze() => graze;
    public int GetBombs() => bombs;
    public int GetGrazePoints() => grazePoints;

    public void SetHealth(int value) => health = value;
    public void SetPower(int value) => power = value;
    public void SetGraze(int value) => graze = value;
    public void SetBombs(int value) => bombs = value;
    public void SetGrazePoints(int value) => grazePoints = value;

    public void ApplyDamage(float amount)
    {
        health -= Mathf.RoundToInt(amount);
        RecentlyDamaged = true;  // mark that entity was damaged
        Debug.Log($"{gameObject.name} took {amount} damage! Remaining health: {health}");
        if (health <= 0) Kill();
    }

    public void Kill()
    {
        Debug.Log($"{gameObject.name} has been killed!");
    }

    // Reset damaged flag.
    public void ResetRecentlyDamaged() => RecentlyDamaged = false;
}
