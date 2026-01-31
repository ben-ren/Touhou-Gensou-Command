using UnityEngine;

public class BombItem : Item
{
    public int bombItemValue = 1;
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
        base.ChangeValue();
        resourceReceiver.SetBombs(resourceReceiver.GetBombs() + bombItemValue);
    }
}
