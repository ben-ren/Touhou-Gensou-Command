using System;
using UnityEngine;
using System.Collections;

public class ProjectileSpawner : MonoBehaviour
{
    [Header("References")]
    private InputController IC;
    //Projectiles
    public GameObject primaryProjectilePrefab;
    public GameObject focusProjectilePrefab;
    public GameObject bombProjectilePrefab;

    //Projectile Spawnpoints
    public GameObject[] spawnPoints = new GameObject[5];

    //Allignment
    [SerializeField] private Team team = Team.Enemy;

    //variables
    [SerializeField] private float primaryfireRate = 1f; // shots per second
    [SerializeField] private float focusfireRate = 1f; // shots per second
    [SerializeField] private float bombTimer = 1f; // timer for shootingEnabled
    [HideInInspector] public bool shootingEnabled = true;
    EntitySystems entitySystems;
    private bool bombActive = false;
    private float fireCooldown = 0f;
    private int lastPowerValue = -1;

    void Start()
    {
        //Reference InputController for device input
        IC = InputController.instance;
        entitySystems = this.GetComponent<EntitySystems>();
        shootingEnabled = true;
    }
    
    void Update()
    {
        fireCooldown -= Time.deltaTime;
        
        int power = entitySystems.GetPower();

        //Enable a number of spawnpoints equal to entitySystems.GetPower()
        if(power != lastPowerValue){
            ReCheckActiveSpawners();
            lastPowerValue = power;
        }

        if (IC.GetBomb() > 0f && !bombActive)
        {
            StartCoroutine(FireBomb());
        }

        if (shootingEnabled) SpawnProjectiles();
    }

    void SpawnProjectiles()
    {
        if (fireCooldown > 0f) return;

        foreach(var spawner in spawnPoints)
        {
            if (spawner.activeInHierarchy && IC.GetFire() > 0f)
            {
                FireProjectile(spawner);
            }
        }

        float currentFireRate = (IC.GetBrake() > 0)
        ? focusfireRate
        : primaryfireRate;

        fireCooldown = 1f / currentFireRate;
    }

    void ReCheckActiveSpawners()
    {
        foreach(var spawner in spawnPoints)
        {
            spawner.SetActive(false);
        }

        int activeSpawners = entitySystems.GetPower()/100;
        activeSpawners = Mathf.Clamp(activeSpawners, 0, patterns.Length - 1);

        //Use entitySystem.GetPower() to loop through spawnPoints
        foreach (int index in patterns[activeSpawners])
        {
            spawnPoints[index].SetActive(true);
        }
    }

    void FireProjectile(GameObject spawner)
    {
        GameObject proj;
        if (IC.GetBrake() > 0f)
            proj = focusProjectilePrefab;   //fire secondary/focus shot
        else
            proj = primaryProjectilePrefab; //fire primary shot

        GameObject projectile = Instantiate(
                proj, 
                spawner.transform.position, 
                this.transform.rotation);
        
        projectile.GetComponent<IWeaponTeam>()?.SetTeam(team);  //Set Team
    }

    //Instantiate bombProjectile & set shootingEnabled to false when bomb
    IEnumerator FireBomb()
    {
        bombActive = true;
        shootingEnabled = false;

        GameObject bomb = Instantiate(
            bombProjectilePrefab, 
            spawnPoints[0].transform.position,
            this.transform.rotation
            );

        bomb.GetComponent<IWeaponTeam>()?.SetTeam(team);    //Set team

        yield return new WaitForSeconds(bombTimer);

        shootingEnabled = true;
        bombActive = false;;
    }

    private readonly int[][] patterns =
    {
        new[] { 0 },            // power 0
        new[] { 1, 2 },         // power 1
        new[] { 0, 1, 2 },      // power 2
        new[] { 0, 1, 2, 3 },   // power 3
        new[] { 0, 1, 2, 3, 4 } // power 4
    };
}
