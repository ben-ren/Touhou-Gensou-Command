using UnityEngine;

public class AutoItemPickup : MonoBehaviour
{
    Item item;
    
    void Awake()
    {
        item = GetComponentInParent<Item>();
        if(item == null)
        {
            Debug.LogError($"AutoItemPickup script on {name} requires the parent to have an Item script component.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && item != null)
        {
            item.SetTarget(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && item != null)
        {
            item.SetTarget(null);
        }
    }
}
