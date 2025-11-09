using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
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
        if (TryGetComponent<CapsuleCollider>(out CapsuleCollider col))
        {
            col.enabled = false;
        }
        //decrease Alpha Value of model to 50% (0.5)
    }

    public override void Deactivate()
    {
        base.Deactivate();
        if (TryGetComponent<CapsuleCollider>(out CapsuleCollider col))
        {
            col.enabled = true;
        }
        //reset Alpha Value of model to 100% (1.0)
    }
}
