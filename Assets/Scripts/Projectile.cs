using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float projectileDamage = 5f;
    public float projectileSpeed = 10f;
    public float projectileLifeSpan = 5f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        // Ensure Rigidbody is kinematic
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        // Ensure Collider is trigger (impact detection)
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        //Destory after set period of time
        Destroy(gameObject, projectileLifeSpan);
    }

    // Update is called once per frame
    public virtual void Update()
    {
    }

    public float GetProjectileDamage()
    {
        return projectileDamage;
    }

    public float GetProjectileSpeed()
    {
        return projectileSpeed;
    }

    public float GetProjectileLifeSpan()
    {
        return projectileLifeSpan;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HomingSystem") || other.CompareTag("Player")) return;

        EntitySystems target = other.GetComponent<EntitySystems>();

        if (target != null && other.CompareTag("Enemy")  )
        {
            target.ApplyDamage(GetProjectileDamage());
        }
        
        Destroy(gameObject);
    }
}
