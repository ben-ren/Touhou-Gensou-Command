using UnityEngine;

public class FairyAI : BasicAI
{
    private Vector3 patrolPoint;
    private bool hasPatrolPoint;

    public override void FixedUpdate()
    {
        base.FixedUpdate();   // keeps detection + shared logic

        RunFairyLogic();
    }

    private void RunFairyLogic()
    {
        // FIRE if target is within firing range
        manager.SetFiringState(spawners, isTargetInRange);

        // STOP and SHOOT (no movement)
        if (StopMovingToFire && isTargetInRange)
        {
            manager.LookTowards(transform, target.position, rotationSpeed);
            return;
        }

        // PICK A NEW PATROL POINT if idle
        if (!isTargetVisible && !hasPatrolPoint)
        {
            hasPatrolPoint = manager.TryFindPatrolPoint(
                transform,
                patrolRadius,
                terrainMask,
                out patrolPoint
            );
        }

        // CHASE target
        if (isTargetVisible)
        {
            manager.MoveTo(transform, target.position, moveSpeed);
        }
        // PATROL
        else if (hasPatrolPoint)
        {
            manager.MoveTo(transform, patrolPoint, moveSpeed);

            if (Vector3.Distance(transform.position, patrolPoint) < 1f)
                hasPatrolPoint = false;
        }
    }
}
