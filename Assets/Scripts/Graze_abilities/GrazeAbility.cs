using System.Threading;
using UnityEngine;

public class GrazeAbility : MonoBehaviour
{
    public int cost = 1; // Optional: how much graze points are needed
    public float GrazeSpentPerSecond = 1f;
    public bool activateAbility = false;
    private float nextTime = 0f;
    
    EntitySystems entity = null;

    //GETTER: Returns the active state of the ability
    public bool IsAbilityActive()
    {
        return activateAbility;
    }

    //SETTER: Sets the active state of the ability
    public void ActivateAbility(bool state)
    {
        activateAbility = state;
    }

    public int GetCost()
    {
        return cost;
    }

    public void SetCost(int cost)
    {
        this.cost = cost;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        if (GetComponent<EntitySystems>() is EntitySystems entity)
        {
            this.entity = entity;
        }
    }

    // Update is called once per frame
    public virtual void Update()
    {
        EnforceCost();
    }

    public virtual void Activate()
    {

    }

    public virtual void Deactivate()
    {
        
    }
    
    void EnforceCost()
    {
        if (entity == null) return;

        if (activateAbility && entity.GetGraze() >= cost)
        {
            Activate();
            if (Time.time >= nextTime)
            {
                nextTime = Time.time + (1f / GrazeSpentPerSecond);
                entity.SetGraze(entity.GetGraze() - cost);
            }
        }
        else
        {
            Deactivate();
        }
    }
}
