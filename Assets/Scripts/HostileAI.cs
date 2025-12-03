using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EntitySystems))]
public class HostileAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Transform target;            // optional target
    [SerializeField] protected List<ProjectileSpawner> spawners = new();
    private Rigidbody rb;
    
    [Header("Layers")]
    [SerializeField] private LayerMask terrainMask;
    [SerializeField] private LayerMask targetMask;

    [Header("Movement Settings")]
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float rotationSpeed = 10f;
    [SerializeField] protected bool enableTargeting = false; // toggle on/off

    [Header("Patrol Settings")]
    [SerializeField] protected float patrolRadius = 30f;
    private Vector3 currentPatrolPoint;
    private bool hasPatrolpoint;

    [Header("Detection Ranges")]
    [SerializeField] protected float visionRange = 20f;
    [SerializeField] protected float engagementRange = 10f;
    [SerializeField] protected bool StopMovingToFire = false;
    protected bool isTargetVisible;
    protected bool isTargetInRange;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        spawners.AddRange(GetComponentsInChildren<ProjectileSpawner>());
    }

    // Update is called once per frame
    public virtual void FixedUpdate()
    {
        DetectTarget();
        AIPatrolLogic();
    }


    //Move HostileAI randomly within patrolRadius
    void AIPatrolLogic()
    {
        SetFiringState(isTargetInRange);

        if (StopMovingToFire && isTargetInRange)
        {
            LookTowards(target.position);
            return;
        }

        if (!hasPatrolpoint && !isTargetVisible) FindPatrolPoint();
        else if (isTargetVisible) MoveTo(target.position);
        else if (hasPatrolpoint) MoveTo(currentPatrolPoint);

        if (Vector3.Distance(transform.position, currentPatrolPoint) < 1f) hasPatrolpoint = false;        
    }
    

    //if player is in detection range (check detection range && check if other team = player) enableTargeting = true
    void DetectTarget()
    {
        if (target == null) return;

        Vector3 dir = target.position - transform.position;
        float distance = dir.magnitude;

        isTargetVisible = distance <= visionRange && !Physics.Raycast(transform.position, dir.normalized, distance, terrainMask);
        isTargetInRange = isTargetVisible && distance <= engagementRange;
    }

    void MoveTo(Vector3 targetPos)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.fixedDeltaTime);
        LookTowards(targetPos);
    }

    void LookTowards(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        if (direction == Vector3.zero) return;
        Quaternion directionRot = Quaternion.LookRotation(direction);
        Quaternion newRot = Quaternion.RotateTowards(transform.rotation, directionRot, rotationSpeed * Time.fixedDeltaTime);
        
        //-------LOCK Z-ROTATION---------
        newRot = Quaternion.Euler(newRot.eulerAngles.x, newRot.eulerAngles.y, 0f);
        transform.rotation = newRot;
    }

    void FindPatrolPoint()
    {
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * patrolRadius;

        Vector3 direction = randomPoint - transform.position;
        float distance = direction.magnitude;
        direction.Normalize(); // normalize for the raycast

        float clearanceRadius = 1.5f; // AI size
        if (!Physics.Raycast(transform.position, direction, distance, terrainMask)
            && !Physics.CheckSphere(randomPoint, clearanceRadius, terrainMask))
        {
            currentPatrolPoint = randomPoint;
            hasPatrolpoint = true;
        }
    }

    void SetFiringState(bool state)
    {
        foreach(ProjectileSpawner spawner in spawners)
        {
            spawner.IsFiring = state;
        }
    }

    private void OnDrawGizmosSelected()
    {
        //engagement range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, engagementRange);
        //vision range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }
}
