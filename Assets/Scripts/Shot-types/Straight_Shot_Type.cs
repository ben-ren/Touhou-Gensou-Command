using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class Straight_Shot_Type : MonoBehaviour
{
    public float projectileSpeed = 10f;

    public float projectileLifeSpan = 5f;


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

        //Destory after set period of time
        Destroy(gameObject, projectileLifeSpan);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * projectileSpeed * Time.deltaTime;   //BUGGED: Doesn't fly in straight line from instantiation point
    }

    void OnTriggerEnter(Collider other)
    {
        if (!(other.CompareTag("Player") || other.CompareTag("HomingSystem")))
        {
            Destroy(gameObject);
        }
    }
}
