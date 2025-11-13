using UnityEngine;

public class Item : MonoBehaviour
{
    public bool IsCollected;
    protected bool destroyObject = false;
    protected EntitySystems entity;
    //Get reference to other's entitySystem
    //On collision, set public flag for EneitySystem value update then destroy current item
    public bool GetIsCollected()
    {
        return IsCollected;
    }

    public void SetIsCollected(bool isCollected)
    {
        this.IsCollected = isCollected;
    }

    public virtual void Start(){}

    public virtual void Update(){}

    public virtual void ChangeValue(){}
    
    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetIsCollected(true);
        }
        if (IsCollected & other.TryGetComponent(out entity))
        {
            ChangeValue();
            Destroy(gameObject);
        }
    }
}
