using System.Collections.Generic;
using UnityEngine;

public class AI_EnemyToken : MonoBehaviour
{
    public GameObject defaultTarget;
    [SerializeField] float moveSpeed = 5f;
    GameObject target;
    public bool resetTargeting;
    public bool startMovement;
    AStarPathway pathfinder;    //Pathfinder code

    List<Vector3> path;         //Pathfinder code
    int pathIndex;              //Pathfinder code

    void Start()
    {
        ResetTarget();          //reset target to default (mothership)
        pathfinder = FindFirstObjectByType<AStarPathway>();    //Pathfinder code
        if (pathfinder == null)
        {
            Debug.LogWarning("AStarPathway not found. Enemy will use direct movement.");
        }
        InvokeRepeating(nameof(RecalculatePath), 0f, 0.5f); //Pathfinder code
        
    }
    
    void Update()
    {
        if (resetTargeting) //reset target
        {
            ResetTarget();
            path = null;        //Pathfinder code
            pathIndex = 0;
            resetTargeting = false; // prevent infinite reset
        }
        if (startMovement)
        {
            MoveEnemyToken();
        }
    }

    void SetTarget(GameObject target){ 
        this.target = target;
        path = null;            //Pathfinder code
        pathIndex = 0;
    }

    void ResetTarget()
    {
        target = defaultTarget;
        RecalculatePath();
    }

    void RecalculatePath()
    {
        if (target == null || pathfinder == null) return;
        
        path = pathfinder.FindPath(transform.position, target.transform.position);
        pathIndex = 0;
    }

    //MoveTowards target
    void MoveEnemyToken()
    {
        // Fallback if no pathfinder exists
        if (pathfinder == null)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                defaultTarget.transform.position,
                moveSpeed * Time.deltaTime
            );
            return;
        }
        if (path == null || pathIndex >= path.Count)//Pathfinder code
        {
            return;
        }

        Vector3 nextPoint = path[pathIndex];//Pathfinder code

        transform.position = Vector2.MoveTowards(
            transform.position,
            nextPoint,
            moveSpeed * Time.deltaTime
        );

        //Pathfinder code
        if (Vector2.Distance(transform.position, nextPoint) < 0.05f)
        {
            pathIndex++;//Pathfinder code
        }
    }
    
    //Set player as target when token has moved into range. 
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            SetTarget(col.gameObject);
        }
    }
}
