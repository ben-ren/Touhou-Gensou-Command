using System.Collections;
using UnityEngine;

public class AutoTurnAround : MonoBehaviour
{
    Terrain terrain;
    PlayerController p_con;
    BasicAI ai;
    Animator animator;
    bool outOfBounds;
    bool turningAround;
    void Start()
    {
        outOfBounds = false;
        turningAround = false;
        animator = GetComponent<Animator>();
        p_con = GetComponent<PlayerController>();
        ai = GetComponent<BasicAI>();
        terrain = FindFirstObjectByType<Terrain>();

        if (terrain == null)
        {
            Debug.LogWarning("AutoTurnAround: No Terrain found in the scene.");
            return;
        }
    }

    
    void Update()
    {
        if (terrain != null){
            outOfBounds = terrainBoundaryCheck();
            ClampObjectToHeightmap();
            if (outOfBounds && !turningAround) StartCoroutine(TurnAround());
        }
    }

    void ClampObjectToHeightmap()
    {
        float terrainHeight = terrain.SampleHeight(transform.position);
        float Y_clamp = terrainHeight + 10f;

        if (transform.position.y < Y_clamp)
        {
            Vector3 position = transform.position;
            position.y = Y_clamp;
            transform.position = position;
        }
    }

    bool terrainBoundaryCheck()
    {
        Vector3 terrainPosition = terrain.transform.position;
        Vector3 terrainSize = terrain.terrainData.size;
        Vector3 objectPosition = transform.position;

        return objectPosition.x > terrainPosition.x + terrainSize.x
            || objectPosition.x < terrainPosition.x
            || objectPosition.z > terrainPosition.z + terrainSize.z
            || objectPosition.z < terrainPosition.z;
    }

    private IEnumerator TurnAround()
    {
        Debug.Log("Triggered");
        turningAround = true;

        if (p_con != null)
        {
            p_con.enabled = false;  // Player
        }
        else if (ai != null)
        {
            ai.enabled = false;     // AI
        }

        //Run pre-made turn_around animation here
        animator.SetBool("IsTurningAround", true);
        yield return new WaitForSeconds(1f);
        animator.SetBool("IsTurningAround", false);

        if (p_con != null)
        {
            p_con.enabled = true;  // Player
        }
        else if (ai != null)
        {
            ai.enabled = true;      // AI
        }

        turningAround = false;
        outOfBounds = false;
    }
}
