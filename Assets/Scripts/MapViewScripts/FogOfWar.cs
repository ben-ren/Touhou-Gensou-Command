using System.Linq.Expressions;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{
    public GameObject m_fogOfWarPlane;
    public Transform[] m_fogBusters;
    public LayerMask m_fogLayer;
    public float m_radius = 5f;
    private float m_radiusSqr { get { return m_radius*m_radius; }}

    private Mesh m_mesh;
    private Vector3[] m_vertices;
    private Color[] m_colours;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialise();
    }

    void Update()
    {
        Vector3 center = m_fogBusters[0].position;

        for (int i = 0; i < m_vertices.Length; i++)
        {
            Vector3 worldPos = m_fogOfWarPlane.transform.TransformPoint(m_vertices[i]);
            float dist = Vector3.SqrMagnitude(worldPos - center);

            ApplyHardEdge(i, dist, true);
        }

        UpdateColours();
    }


    void Initialise()
    {
        m_mesh = m_fogOfWarPlane.GetComponent<MeshFilter>().mesh;
        m_vertices = m_mesh.vertices;
        m_colours = new Color[m_vertices.Length];
        for(int i=0; i < m_colours.Length; i++)
        {
            m_colours[i] = Color.black;
        }
        UpdateColours();
    }

    void UpdateColours()
    {
       m_mesh.colors = m_colours;
    }

    void ApplyGradient(int index, float distance, bool persistant)
    {
        float alpha = Mathf.Clamp01(distance / m_radiusSqr);

        if (persistant)
        {
            // Only reduce alpha
            m_colours[index].a = Mathf.Min(m_colours[index].a, alpha);
        }
        else
        {
            // Dynamic: reset each frame
            m_colours[index].a = alpha;
        }
    }

    void ApplyHardEdge(int index, float distance, bool persistant)
    {
        if (distance < m_radiusSqr)
        {
            m_colours[index].a = 0f;   // fully transparent (revealed)
        }
        else if (!persistant)
        {
            m_colours[index].a = 1f;   // fully opaque (fog)
        }
    }
}
