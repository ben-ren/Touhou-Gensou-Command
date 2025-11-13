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

    public override void ChangeValue()
    {
        base.ChangeValue();
        entity.SetGraze(entity.GetGraze() + grazeItemValue);
    }
}
