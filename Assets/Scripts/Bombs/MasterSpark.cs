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
        if (laserComponent != null){
            laserComponent.laserLifeSpan = projectileLifeSpan;
            laserComponent.SetTeam(TeamAlignment);
        }
    }

    public override void Update()
    {
        laserComponent?.Update();
    }

    protected override void OnTriggerEnter(Collider other){}
}
