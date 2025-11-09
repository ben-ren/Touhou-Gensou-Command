using UnityEngine;

public class BlazingStar : GrazeAbility
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
        Debug.Log("BlazingStar!");
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }
}
