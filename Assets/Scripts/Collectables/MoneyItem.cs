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

    public override void ChangeValue(CharacterData target)
    {
        base.ChangeValue(target);
        ResourceService.Add(ResourceType.Money, moneyItemValue);
    }
}
