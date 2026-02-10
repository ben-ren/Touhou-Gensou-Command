using UnityEngine;

public class PlayerProjManager : ProjSpawnManager
{
    [Header("Input & Projectiles")]
    private InputController IC => InputController.instance;
    [SerializeField] private GameObject primaryProjectile;
    [SerializeField] private GameObject secondaryProjectile;
    [SerializeField] private BombSpawner bombSpawner;
    [SerializeField] private float primaryFireRate = 1f;
    [SerializeField] private float secondaryFireRate = 5f;
    private CharacterData parentSystemData;

    private bool bombButtonPreviouslyPressed = false;

    protected override void Start()
    {
        base.Start();
        parentSystemData = GetComponentInParent<EntitySystems>()?.GetCharacterData();
    }

    protected override void Update()
    {
        base.Update();  // handle active spawners
        power = parentSystemData.powerData/100; //determines active spawner count

        if (IC != null && primaryProjectile != null && secondaryProjectile != null)
        {
            UpdateProjectileType();
            FiringControls();
        }

        // Bomb input: only spawn on new press AND if no bomb exists
        bool bombPressed = IC.GetBomb() > 0f;
        if (bombPressed && !bombButtonPreviouslyPressed && bombSpawner != null && !bombSpawner.IsBombActive())
        {
            LaunchBomb();
        }
        bombButtonPreviouslyPressed = bombPressed;
    }

    private void UpdateProjectileType()
    {
        bool isFocus = IC.GetBrake() > 0;
        GameObject currentProjectile = isFocus ? secondaryProjectile : primaryProjectile;
        float currentFireRate = isFocus ? secondaryFireRate : primaryFireRate;

        foreach (var spawner in spawners)
        {
            if (spawner != null)
            {
                spawner.SetProjectile(currentProjectile);
                spawner.SetFireRate(currentFireRate);
            }
        }
    }

    private void FiringControls()
    {
        bool shoot = IC.GetFire() > 0f;
        foreach (var spawner in spawners)
        {
            spawner.IsFiring = shoot;
            spawner.LaserCheck(shoot);
        }
    }

    private void LaunchBomb()
    {
        if (parentSystemData == null || parentSystemData.bombsData <= 0) 
            return; 

        if (!bombSpawner.TrySpawnBomb()) 
            return; 
        
        parentSystemData.bombsData--;
    }
}
