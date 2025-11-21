using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InnateDream : GrazeAbility
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
    public override void Activate()
    {
        base.Activate();
        Debug.Log("Innate Dream!");
        //turn off Capsule Collider
        if (TryGetComponent<Collider>(out Collider col))
        {
            col.enabled = false;
        }
        
        // Optional placeholder visual feedback
        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = false;  // hide meshes for now
    }

    public override void Deactivate()
    {
        base.Deactivate();
        if (TryGetComponent<Collider>(out Collider col))
        {
            col.enabled = true;
        }
        
        // restore visuals
        foreach (var r in GetComponentsInChildren<Renderer>())
            r.enabled = true;
    }
}
