using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Straight_Shot_Type : Projectile
{
    [SerializeField] private AudioClip projectileSoundClip;
    
    public override void Start()
    {
        base.Start();
        SFXManager.instance.PlaySFXClip(projectileSoundClip,transform,1f);
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        transform.position += transform.forward * projectileSpeed * Time.deltaTime;
    }
}
