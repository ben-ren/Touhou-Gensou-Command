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
    
    [Header("Team Assignment")]
    [SerializeField] private Team team = Team.Neutral;
    public Team TeamAlignment => team;

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
        if (Time.time >= nextTime)
        {
            nextTime = Time.time + (1f / accumulationRate);
            AccumulatedGrazePoints++;
        }
    }

    //Apply the graze effect & calculation when projectile leaves graze box
    void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out ITeamMember teamMember)) return;

        // Skip if the object was recently damaged by a projectile
        if (GetComponentInParent<EntitySystems>() is EntitySystems entity && !entity.RecentlyDamaged && teamMember.TeamAlignment != team)
        {
            ApplyGraze(entity, AccumulatedGrazePoints);     //apply graze function from script
        }

        AccumulatedGrazePoints = 0;         // Reset point accumulation
    }
}
