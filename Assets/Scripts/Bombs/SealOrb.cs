using UnityEngine;

public class SealOrb : Bomb
{
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        transform.position += transform.forward * projectileSpeed * Time.deltaTime;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        // // Destroy other projectiles on contact
        // if (other.TryGetComponent<Projectile>(out Projectile proj))
        // {
        //     // Avoid self-destroying
        //     if (proj != this)
        //         Destroy(proj.gameObject);
        // }
    }
}
