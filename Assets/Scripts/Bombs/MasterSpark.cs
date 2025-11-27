using UnityEngine;

[RequireComponent(typeof(Laser))]
public class MasterSpark : Bomb
{
    private Laser laserComponent;

    public override void Start()
    {
        base.Start();
        
        // Grab the Laser component
        laserComponent = GetComponent<Laser>();
        if (laserComponent != null)
            laserComponent.laserLifeSpan = projectileLifeSpan;
            laserComponent.SetTeam(TeamAlignment);

        Destroy(gameObject, projectileLifeSpan);
    }

    public override void Update()
    {
        // Only update the laser if the component exists
        laserComponent?.Update();
        ProjectileDestruction();
    }

    void ProjectileDestruction()
    {
        // Handle projectile destruction along laser path
        RaycastHit[] hits = Physics.RaycastAll(transform.position, transform.forward, laserComponent.laserRange);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.TryGetComponent(out Projectile proj))
            {
                if (proj != this) Destroy(proj.gameObject);
            }
        }
    }
}
