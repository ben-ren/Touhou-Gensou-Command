using UnityEngine;

public class Ofuda_Shot_Type : MonoBehaviour
{
    public float projectileSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.forward * projectileSpeed * Time.deltaTime;   //BUGGED: Doesn't fly in straight line from instantiation point
    }
    
    
}
