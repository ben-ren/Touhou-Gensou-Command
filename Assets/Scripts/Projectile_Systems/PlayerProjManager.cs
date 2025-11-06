using UnityEngine;

// Player-specific manager, inherits from base
public class PlayerProjManager : ProjSpawnManager
{
    [Header("Input & Projectiles")]
    [SerializeField] private InputController IC;
    [SerializeField] private GameObject primaryProjectile;
    [SerializeField] private GameObject secondaryProjectile;
    [SerializeField] private float primaryFireRate = 1f;
    [SerializeField] private float secondaryFireRate = 5f;

    protected override void Update()
    {
        base.Update();  // handle active spawners

        if (IC != null && primaryProjectile != null && secondaryProjectile != null)
        {
            UpdateProjectileType();
        }
        FiringControls();
    }

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
    
    void FiringControls()
    {
        bool shoot = IC.GetFire() > 0f;
        foreach (var spawner in spawners)
        {
            spawner.IsFiring = shoot;
        }
    }
}
