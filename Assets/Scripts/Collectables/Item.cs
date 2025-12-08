using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private AudioClip itemCollectSoundClip;
    public bool IsCollected;
    protected bool destroyObject = false;
    protected EntitySystems entity;
    protected float moveSpeed = 10f;
    protected GameObject target;

    // --- Scatter variables ---
    private Vector3 velocity;
    private float gravity = -9f;
    private float drag = 4f;
    private float scatterDuration = .5f;
    private bool isScattering = true;

    public bool GetIsCollected() => IsCollected;
    public void SetIsCollected(bool isCollected) => IsCollected = isCollected;
    public GameObject SetTarget() => target;
    public void SetTarget(GameObject target) => this.target = target;

    public virtual void Start(){}

    // Called externally by ResourceDeploy
    public void ScatterFrom(Vector3 origin)
    {
        Vector3 dir = Random.onUnitSphere;
        if (dir.y < 0) dir.y *= -1; // upward bias

        float force = Random.Range(4f, 8f);
        velocity = dir * force;
        isScattering = true;
        scatterDuration = Random.Range(0.9f, 1.4f);
    }

    public virtual void Update()
    {
        // If scattering, simulate fake physics
        if (isScattering)
        {
            scatterDuration -= Time.deltaTime;
            if (scatterDuration <= 0f)
            {
                isScattering = false;
                return;
            }

            // gravity
            velocity.y += gravity * Time.deltaTime;

            // drag to smooth deceleration
            velocity = Vector3.Lerp(velocity, Vector3.zero, drag * Time.deltaTime);

            // move item
            transform.position += velocity * Time.deltaTime;
            return;
        }

        // Once scattering finishes, homing behavior works as before
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target.transform.position,
                moveSpeed * Time.deltaTime
            );
        }
    }

    public virtual void ChangeValue(){}
    
    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetIsCollected(true);
        }
        if (IsCollected & other.TryGetComponent(out entity))
        {
            SFXManager.instance.PlaySFXClip(itemCollectSoundClip,transform,1f);
            ChangeValue();
            Destroy(gameObject);
        }
    }
}
