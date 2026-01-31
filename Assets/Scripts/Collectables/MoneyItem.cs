using UnityEngine;

public class MoneyItem : Item
{
    public int moneyItemValue = 1;
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
        resourceReceiver.SetMoney(resourceReceiver.GetMoney() + moneyItemValue);
    }
}
