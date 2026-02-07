using UnityEngine;

public class GrazeItem : Item
{
    public int grazeItemValue = 1;
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
        ResourceService.Add(ResourceType.Graze, grazeItemValue, target);
    }
}
