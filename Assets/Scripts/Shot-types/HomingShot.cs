using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class HomingShot : MonoBehaviour
{
    [SerializeField] private float turnSpeed = 5f;
    [SerializeField] private bool homingActive = true;
    private Transform target;
    private GameObject parent;

    void Awake()
    {
        // Check if this GameObject has a parent
        if (transform.parent == null)
        {
            Debug.LogError($"HomingShot on '{gameObject.name}' must be attached to a child object with a parent.", this);
            // Optionally disable the script so it doesn't run
            enabled = false;
            return;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Ensure Rigidbody is kinematic
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        // Ensure Collider is trigger (impact detection)
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        //Get parent gameObject to apply transforms to.
        parent = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (parent == null)
        {
            parent = transform.parent.gameObject;
        }
        AimObjectTowardsTarget(parent, target);
    }

    void AimObjectTowardsTarget(GameObject obj, Transform target)
    {
        if (homingActive && target != null)
        {
            Vector3 direction = (target.position - obj.transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            obj.transform.rotation = Quaternion.Slerp(obj.transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        parent = transform.parent.gameObject;
        EntitySystems entity = other.GetComponent<EntitySystems>();
        if (entity == null || other.CompareTag("GrazeBox")) return;

        if(entity.TeamAlignment != parent.GetComponent<Projectile>().TeamAlignment)
        {
            target = other.transform;
            homingActive = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        parent = transform.parent.gameObject;
        EntitySystems entity = other.GetComponent<EntitySystems>();
        if (entity == null) return;

        if(other.transform == target && entity.TeamAlignment != parent.GetComponent<Projectile>().TeamAlignment)
        {
            homingActive = false;
            target = null;
        }
    }
}
