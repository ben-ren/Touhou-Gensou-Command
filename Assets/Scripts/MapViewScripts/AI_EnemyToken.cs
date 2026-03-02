using UnityEngine;

public class AI_EnemyToken : MonoBehaviour
{
    public GameObject defaultTarget;
    [SerializeField] float moveSpeed = 5f;
    GameObject target;
    public bool resetTargeting;
    public bool startMovement;

    void SetTarget(GameObject target) => this.target = target;
    
    void Start()
    {
        target = defaultTarget; //set initial target to mothership
    }
    
    void Update()
    {
        if (resetTargeting) //reset target
        {
            target = defaultTarget;
        }
        if (startMovement)
        {
            MoveEnemyToken();
        }
    }

    //MoveTowards target
    //TODO: Replace with A* pathway naviagtion.
    void MoveEnemyToken()
    {
        transform.position = Vector2.MoveTowards(
            transform.position, target.transform.position,
            moveSpeed * Time.deltaTime
        );
    }
    
    //Set player as target when token has moved into range. 
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            SetTarget(col.gameObject);
        }
    }
}
