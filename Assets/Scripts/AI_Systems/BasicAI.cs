using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EntitySystems))]
public class BasicAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Transform target;
    [SerializeField] protected List<ProjectileSpawner> spawners = new();
    protected Rigidbody rb;
    protected AIManager manager;

    [Header("Layers")]
    [SerializeField] protected LayerMask terrainMask;

    [Header("Movement Settings")]
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float rotationSpeed = 10f;

    [Header("Patrol Settings")]
    [SerializeField] protected float patrolRadius = 30f;

    [Header("Detection Ranges")]
    [SerializeField] protected float visionRange = 20f;
    [SerializeField] protected float engagementRange = 10f;
    [SerializeField] protected bool StopMovingToFire = false;

    protected bool isTargetVisible;
    protected bool isTargetInRange;

    public virtual void Start()
    {
        manager = AIManager.instance;
        rb = GetComponent<Rigidbody>();
        spawners.AddRange(GetComponentsInChildren<ProjectileSpawner>());
    }

    public virtual void FixedUpdate()
    {
        DetectTarget();
        // NO AIMOVEMENT HERE
        // Children decide their logic
    }

    protected void DetectTarget()
    {
        if (target == null)
        {
            isTargetVisible = false;
            isTargetInRange = false;
            return;
        }

        Vector3 dir = target.position - transform.position;
        float dist = dir.magnitude;

        isTargetVisible = dist <= visionRange &&
            !Physics.Raycast(transform.position, dir.normalized, dist, terrainMask);

        isTargetInRange = isTargetVisible && dist <= engagementRange;
    }
}
