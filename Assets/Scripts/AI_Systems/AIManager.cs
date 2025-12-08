using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    [Header("Singleton Instance")]
    public static AIManager instance { get; private set; }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // -----------------------------
    //  MOVEMENT FUNCTIONS
    // -----------------------------

    public void MoveTo(Transform ai, Vector3 targetPos, float moveSpeed)
    {
        ai.position = Vector3.MoveTowards(
            ai.position,
            targetPos,
            moveSpeed * Time.fixedDeltaTime
        );
    }

    public void LookTowards(Transform ai, Vector3 target, float rotationSpeed)
    {
        Vector3 direction = target - ai.position;
        if (direction == Vector3.zero) return;

        Quaternion directionRot = Quaternion.LookRotation(direction);
        Quaternion newRot = Quaternion.RotateTowards(
            ai.rotation,
            directionRot,
            rotationSpeed * Time.fixedDeltaTime
        );

        // Lock Z (as requested)
        newRot = Quaternion.Euler(newRot.eulerAngles.x, newRot.eulerAngles.y, 0f);
        ai.rotation = newRot;
    }

    // -----------------------------
    //  PATROL
    // -----------------------------

    public bool TryFindPatrolPoint(
        Transform ai,
        float patrolRadius,
        LayerMask terrainMask,
        out Vector3 patrolPoint)
    {
        Vector3 randomPoint = ai.position + Random.insideUnitSphere * patrolRadius;

        Vector3 dir = randomPoint - ai.position;
        float distance = dir.magnitude;
        dir.Normalize();

        float clearanceRadius = 1.5f;

        bool blockedByRay = Physics.Raycast(ai.position, dir, distance, terrainMask);
        bool blockedByOverlap = Physics.CheckSphere(randomPoint, clearanceRadius, terrainMask);

        if (!blockedByRay && !blockedByOverlap)
        {
            patrolPoint = randomPoint;
            return true;
        }

        patrolPoint = ai.position;
        return false;
    }

    // -----------------------------
    //  FIRING / PROJECTILES
    // -----------------------------

    public void SetFiringState(IEnumerable<ProjectileSpawner> spawners, bool state)
    {
        foreach (ProjectileSpawner spawner in spawners)
            spawner.IsFiring = state;
    }
}
