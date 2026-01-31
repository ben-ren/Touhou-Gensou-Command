using UnityEngine;

public class HealthItem : Item
{
    public int healthItemValue = 1;
    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void ChangeValue()
    {
        resourceReceiver.SetHealth(resourceReceiver.GetHealth() + healthItemValue);
    }
}
