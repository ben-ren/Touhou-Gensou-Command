using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class Graze : MonoBehaviour
{
    [Header("Variables")]
    public float grazeDelay = 0f;
    private int AccumulatedGrazePoints = 0;
    private float nextTime = 0f;
    private float accumulationRate = 1f;
    [SerializeField] private GameObject resource;
    
    [Header("Team Assignment")]
    [SerializeField] private Team team = Team.Neutral;
    public Team TeamAlignment => team;

    [Header("Effects")]
    public ParticleSystem VFXPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyGraze(EntitySystems entity, int points)
    {
        Debug.Log("graze");
        entity.SetGraze(entity.GetGraze() + points);
    }

    //As long as projectile is in box accumulate graze points
    void OnTriggerStay(Collider other)
    {
        EntitySystems entity = GetComponentInParent<EntitySystems>();

        if (Time.time >= nextTime && other.TryGetComponent<IGrazable>(out IGrazable grazable))
        {
            if(grazable.GetGrazePoints() > 0){
                nextTime = Time.time + (1f / accumulationRate);
                
                AccumulatedGrazePoints++;
                grazable.SetGrazePoints(grazable.GetGrazePoints() - 1);

                if(VFXPrefab != null && !entity.RecentlyDamaged){
                    VFXManager.Instance.GenerateParticleVFX(VFXPrefab, transform, .1f);
                }
            }
        }
    }

    //Apply the graze effect & calculation when projectile leaves graze box
    void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out ITeamMember teamMember)) return;

        // Skip if the object was recently damaged by a projectile
        if (GetComponentInParent<EntitySystems>() is EntitySystems entity && !entity.RecentlyDamaged && teamMember.TeamAlignment != team)
        {
            GameObject drop = Instantiate(resource, other.transform.position, Quaternion.identity);
            
            if (drop.TryGetComponent(out ResourceDeploy res))
            {
                res.graze = AccumulatedGrazePoints;
                // You can immediately deploy here OR let the prefab’s Start trigger it
                res.DeployItems();
            }
        }

        AccumulatedGrazePoints = 0;         // Reset point accumulation
    }
}
