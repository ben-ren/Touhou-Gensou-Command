using Mono.Cecil;
using UnityEngine;

public class EntitySystems : MonoBehaviour, ITeamMember, IGrazable
{
    [Header("Character Template")]
    [SerializeField] private bool isPlayerEntity = false;
    private CharacterData characterData;
    public int characterIndex;
    
    /* =========================
     * Entity Stats
     * ========================= */
    [Header("Entity Stats")]
    [SerializeField] private int health = 50;
    [SerializeField] private int power = 0;
    [SerializeField] private int graze = 0;
    [SerializeField] private int fuel = 0;
    [SerializeField] private int money = 0;
    [SerializeField] private int bombs = 0;

    private int initialHealth;
    private int initialBombs;

    /* =========================
     * Team
     * ========================= */
    [Header("Team Alignment")]
    [SerializeField] private Team team = Team.Neutral;
    public Team TeamAlignment => team;

    /* =========================
     * Resource Drop
     * ========================= */
    [Header("Resources")]
    [SerializeField] private int grazePoints = 0;
    [SerializeField] private GameObject resource;
    [SerializeField] private AudioClip damageRecievedSoundClip;
    [SerializeField] private AudioClip entityDefeatedSoundClip;

    /* =========================
     * I-Frames
     * ========================= */
    [Header("Invincibility Frames")]
    private IFrameVisuals iFrameVisuals;
    [SerializeField] private float iFrameDuration = 1f; // seconds
    private float iFrameTimer = 0f;

    [HideInInspector] public bool isInvincible = false;
    public bool RecentlyDamaged { get; private set; } = false;
    
    /* =========================
     * Getters
     * ========================= */
    public int GetHealth() => health;
    public int GetPower() => power;
    public int GetGraze() => graze;
    public int GetFuel() => fuel;
    public int GetMoney() => money;
    public int GetBombs() => bombs;
    public int GetGrazePoints() => grazePoints;
    public Team GetTeam() => team;

    /* =========================
     * Setters
     * ========================= */
    public void SetHealth(int value) => health = value;
    public void SetPower(int value) => power = value;
    public void SetGraze(int value) => graze = value;
    public void SetFuel(int value) => fuel = value;
    public void SetMoney(int value) => money = value;
    public void SetBombs(int value) => bombs = value;
    public void SetGrazePoints(int value) => grazePoints = value;
    public void SetTeam(Team newTeam) => team = newTeam;

    /* =========================
    * Unity Lifecycle
    * ========================= */
    void Awake()
    {
        if (isPlayerEntity)
        {
            characterData = GameState.Instance.Data.partyMembers[characterIndex];
        }
        else
        {
            initialHealth = health;
            initialBombs = bombs;
        }
    }

    void Start()
    {
        initialHealth = health;
        initialBombs = bombs;

        if (gameObject.TryGetComponent(out IFrameVisuals vis))
        {
            iFrameVisuals = vis;
        }
    }

    void Update()
    {
        ResourceCap();
        IFrameTimer();

        if (isPlayerEntity)
            ReadCharacterData();
    }

    /* =========================
     * PlayerData sync (read-only)
     * ========================= */
    private void ReadCharacterData()
    {
        if (characterData == null) return;

        // EntitySystems reads live stats from CharacterData
        health = characterData.healthData;
        power = characterData.powerData;
        graze = characterData.grazeData;
        bombs = characterData.bombsData;
    }

    /* =========================
     * Damage & Death
     * ========================= */
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

        GameObject reso = Instantiate(resource, transform.position, Quaternion.identity);

        if (reso.TryGetComponent(out ResourceDeploy newRes))
        {
            SetResources(newRes);
            newRes.DeployItems();
        }
        
        gameObject.SetActive(false);
    }

    /* =========================
     * Resource Transfer
     * ========================= */
    void SetResources(ResourceDeploy res)
    {
        res.health = this.health;
        res.power = this.power;
        res.graze = this.graze;
        res.fuel = this.fuel;
        res.money = this.money;
        res.bombs = this.bombs;
    }

    /* =========================
     * Helpers
     * ========================= */
    void IFrameTimer()
    {
        if (isInvincible)
        {
            iFrameTimer -= Time.deltaTime;
            if (iFrameTimer <= 0f)
                isInvincible = false;
        }
    }

    void ResourceCap()
    {
        health = Mathf.Clamp(health, 0, initialHealth);
        bombs = Mathf.Clamp(bombs, 0, initialBombs);
        power = Mathf.Clamp(power, 0, 400);
    }

    // Reset damaged flag.
    public void ResetRecentlyDamaged() => RecentlyDamaged = false;

    public CharacterData GetCharacterData() => characterData;
}