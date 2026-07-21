using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    [SerializeField] private float damage = 10f;

    private EntitySystems entity;

    private void Awake()
    {
        entity = GetComponent<EntitySystems>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out EntitySystems target))
            return;

        if (entity != null)
        {
            if (target.TeamAlignment == entity.TeamAlignment)
                return;
        }

        target.ApplyDamage(damage);
    }
}
