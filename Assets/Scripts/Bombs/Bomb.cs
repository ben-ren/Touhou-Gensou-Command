using UnityEngine;

public class Bomb : Projectile
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        Destroy(gameObject, projectileLifeSpan);
    }

    // Update is called once per frame
    public override void Update()
    {
        
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Projectile proj))
        {
            if (proj != this){
                Destroy(proj.gameObject);   // Destroy other projectiles on contact
            }
        }

        EntitySystems entity = other.GetComponent<EntitySystems>();

        if (other.CompareTag("HomingSystem") || (entity != null && entity.TeamAlignment == team)) return;

        if (entity != null && entity.TeamAlignment != team)
        {
            entity.ApplyDamage(GetProjectileDamage());
        }
    }
}
