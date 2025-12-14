using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class Graze : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private float accumulationRate = 0f;
    private int AccumulatedGrazePoints = 0;
    private float nextTime = 0f;

    [SerializeField] private GameObject resource;
    
    [Header("Team Assignment")]
    [SerializeField] private Team team = Team.Neutral;
    public Team TeamAlignment => team;

    [Header("Effects")]
    public ParticleSystem VFXPrefab;

    private EntitySystems entity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        entity = GetComponentInParent<EntitySystems>();
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
        if (entity.isInvincible) return;  // Block accumulation during I-frames
        if (Time.time < nextTime) return;
        if (!other.TryGetComponent(out IGrazable grazable)) return;
        if (!other.TryGetComponent(out ITeamMember otherTeam)) return;
        if (otherTeam.TeamAlignment == team) return;
        if (grazable.GetGrazePoints() <= 0) return;

        nextTime = Time.time + accumulationRate;
        AccumulatedGrazePoints++;
        grazable.SetGrazePoints(grazable.GetGrazePoints() - 1);

        if(VFXPrefab != null)
            VFXManager.Instance.GenerateParticleVFX(VFXPrefab, transform, .1f);
    }

    //Apply the graze effect & calculation when projectile leaves graze box
    void OnTriggerExit(Collider other)
    {
        if (AccumulatedGrazePoints <= 0) return;
        if (!other.TryGetComponent(out ITeamMember teamMember)) return;
        if (teamMember.TeamAlignment == team) return;
        if (entity.isInvincible)
        {
            AccumulatedGrazePoints = 0; // Reset
            return;    // Only payout if not invincible  
        } 

        GameObject drop = Instantiate(resource, other.transform.position, Quaternion.identity);

        if (drop.TryGetComponent(out ResourceDeploy res))
        {
            res.graze = AccumulatedGrazePoints;
            res.health = 1;
            res.DeployItems();
        }

        AccumulatedGrazePoints = 0; // Reset
    }
}
