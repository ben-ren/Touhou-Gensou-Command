using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private AudioClip itemCollectSoundClip;
    public bool IsCollected;
    protected bool destroyObject = false;
    protected EntitySystems entity;
    protected float moveSpeed = 10f;
    protected GameObject target;

    public bool GetIsCollected()
    {
        return IsCollected;
    }

    public void SetIsCollected(bool isCollected)
    {
        this.IsCollected = isCollected;
    }

    public GameObject SetTarget()
    {
        return target;
    }

    public void SetTarget(GameObject target)
    {
        this.target = target;
    }

    public virtual void Start(){}

    public virtual void Update()
    {
        if (this.target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position,target.transform.position, moveSpeed * Time.deltaTime);
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
