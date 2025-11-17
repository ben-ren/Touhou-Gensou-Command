using UnityEngine;

// Player-specific manager, inherits from base
public class PlayerProjManager : ProjSpawnManager
{
    [Header("Input & Projectiles")]
    [SerializeField] private InputController IC;
    [SerializeField] private GameObject primaryProjectile;
    [SerializeField] private GameObject secondaryProjectile;
    [SerializeField] private BombSpawner bombSpawner;
    [SerializeField] private float primaryFireRate = 1f;
    [SerializeField] private float secondaryFireRate = 5f;

    private bool firingLocked = false;
    private float bombLockTimer = 0f;

    protected override void Update()
    {
        base.Update();  // handle active spawners

        FiringLock();

        if (!firingLocked && IC != null && primaryProjectile != null && secondaryProjectile != null)
        {
            UpdateProjectileType();
            FiringControls();
        }

        // Bomb input
        if (IC.GetBomb() > 0f && bombSpawner != null)
        {
            LaunchBomb();
        }
    }

    //Update the projectile primary fire when braking.
    private void UpdateProjectileType()
    {
        bool isFocus = IC.GetBrake() > 0;
        GameObject currentProjectile = isFocus ? secondaryProjectile : primaryProjectile;
        float currentFireRate = isFocus ? secondaryFireRate : primaryFireRate;

        foreach (var spawner in spawners)
        {
            if (spawner != null)
                spawner.SetProjectile(currentProjectile);
            spawner.SetFireRate(currentFireRate);
        }
    }
    
    //Fires projectiles from all spawners when fire button is held down.
    void FiringControls()
    {
        bool shoot = IC.GetFire() > 0f;
        foreach (var spawner in spawners)
        {
            spawner.IsFiring = shoot;
        }
    }

    //ticks firing lock in-line with bomb lifespan
    void FiringLock()
    {
        if (firingLocked)
        {
            bombLockTimer -= Time.deltaTime;
            if (bombLockTimer <= 0f)
                firingLocked = false;
                bombSpawner.ClearCurrentBomb(); // allow spawning another bomb
        }
    }

    void LaunchBomb()
    {
        if (bombSpawner.TrySpawnBomb(out float bombDuration))
        {
            firingLocked = true;
            bombLockTimer = bombDuration;
        }
    }
}
