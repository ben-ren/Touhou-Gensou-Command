using UnityEngine;

public class WheelGhostAI : BasicAI
{
    public override void FixedUpdate()
    {
        base.FixedUpdate();

        RunWheelGhostLogic();
    }

    void RunWheelGhostLogic()
    {
        // CHASE target
        if (isTargetVisible)
        {
            manager.MoveTo(transform, target.position, moveSpeed);
        }
    }
}
