using UnityEngine;

public class EntitySystems : MonoBehaviour, ITeamMember
{
    [Header("Entity Stats")]
    [SerializeField] private int health = 50;
    [SerializeField] private int power = 0;
    [SerializeField] private int graze = 0;
    [SerializeField] private int fuel = 0;
    [SerializeField] private int money = 0;
    [SerializeField] private int bombs = 0;

    [Header("Team Alignment")]
    [SerializeField] private Team team = Team.Neutral;
    public Team TeamAlignment => team;

    [Header("Resources")]
    [SerializeField] private int grazePoints = 0;
    [SerializeField] private AudioClip damageRecievedSoundClip;
    [SerializeField] private AudioClip entityDefeatedSoundClip;

    [Header("Invincibility Frames")]
    private IFrameVisuals iFrameVisuals;
    [SerializeField] private float iFrameDuration = 1f; // seconds
    private bool isInvincible = false;
    private float iFrameTimer = 0f;

    public bool RecentlyDamaged { get; private set; } = false;
    
    public int GetHealth() => health;
    public int GetPower() => power;
    public int GetGraze() => graze;
    public int GetFuel() => fuel;
    public int GetMoney() => money;
    public int GetBombs() => bombs;
    public int GetGrazePoints() => grazePoints;
    public Team GetTeam() => team;

    public void SetHealth(int value) => health = value;
    public void SetPower(int value) => power = value;
    public void SetGraze(int value) => graze = value;
    public void SetFuel(int value) => fuel = value;
    public void SetMoney(int value) => money = value;
    public void SetBombs(int value) => bombs = value;
    public void SetGrazePoints(int value) => grazePoints = value;
    public void SetTeam(Team newTeam) => team = newTeam;

    void Start()
    {
        if (gameObject.TryGetComponent(out IFrameVisuals vis))
        {
            iFrameVisuals = vis;
        }
    }

    void Update()
    {
        IFrameTimer();
    }

    void IFrameTimer()
    {
        if (isInvincible)
        {
            iFrameTimer -= Time.deltaTime;
            if (iFrameTimer <= 0f)
                isInvincible = false;
        }
    }

    public void ApplyDamage(float amount, bool ignoreIFrames = false)
    {
        if (isInvincible) return; // skip damage if in I-frames

        health -= Mathf.RoundToInt(amount);
        RecentlyDamaged = true;  // mark that entity was damaged

        SFXManager.instance.PlaySFXClip(damageRecievedSoundClip,transform,1f);

        Debug.Log($"{gameObject.name} took {amount} damage! Remaining health: {health}");

        if(ignoreIFrames){
            // Trigger I-frames
            isInvincible = true;
            iFrameTimer = iFrameDuration;
        }

        if (iFrameVisuals != null)  // Trigger visual feedback on all renderers under Visuals
        {
            iFrameVisuals?.StartIFrameVisual(iFrameDuration);
        }

        if (TeamAlignment == Team.Player)   //shake camera if entity is player
        {
            VFXManager.Instance.ShakeCamera(.2f, .1f);
        }

        if (health <= 0)
        {
            SFXManager.instance.PlaySFXClip(entityDefeatedSoundClip,transform,1f);
            Kill();
        }
    }

    public void Kill()
    {
        Debug.Log($"{gameObject.name} has been killed!");
        gameObject.SetActive(false);
    }

    // Reset damaged flag.
    public void ResetRecentlyDamaged() => RecentlyDamaged = false;
}
