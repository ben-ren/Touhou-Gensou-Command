using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class ConcaveShellCollider : MonoBehaviour
{
    [Header("Shell Settings")]
    public float innerRadius = 3f;
    public float outerRadius = 5f;
    public int radialSegments = 24;      // Horizontal segments
    public int verticalSegments = 12;    // Vertical segments
    public bool isTrigger = true;

    [Header("Debug")]
    public bool showMesh = false; // Toggle for visibility

    private Mesh mesh;
    private MeshRenderer meshRenderer;

    void Start()
    {
        GenerateShell();

        // Cache the MeshRenderer
        meshRenderer = GetComponent<MeshRenderer>();
        UpdateMeshVisibility();
    }

    /// <summary>
    /// Call this to toggle the mesh visibility at runtime.
    /// </summary>
    public void SetMeshVisible(bool visible)
    {
        showMesh = visible;
        UpdateMeshVisibility();
    }

    private void UpdateMeshVisibility()
    {
        if (meshRenderer != null)
            meshRenderer.enabled = showMesh;
    }

    void GenerateShell()
    {
        mesh = new Mesh();
        mesh.name = "ProceduralConcaveShell";

        Vector3[] vertices = new Vector3[(radialSegments + 1) * (verticalSegments + 1) * 2];
        int[] triangles = new int[radialSegments * verticalSegments * 6 * 2]; // outer + inner
        Vector2[] uvs = new Vector2[vertices.Length];

        int vert = 0;
        int tri = 0;

        // Generate vertices
        for (int y = 0; y <= verticalSegments; y++)
        {
            float v = (float)y / verticalSegments;
            float phi = v * Mathf.PI; // angle from bottom to top

            for (int i = 0; i <= radialSegments; i++)
            {
                float u = (float)i / radialSegments;
                float theta = u * Mathf.PI * 2f;

                // Outer vertex
                float xOuter = Mathf.Sin(phi) * Mathf.Cos(theta) * outerRadius;
                float yOuter = Mathf.Cos(phi) * outerRadius;
                float zOuter = Mathf.Sin(phi) * Mathf.Sin(theta) * outerRadius;
                vertices[vert] = new Vector3(xOuter, yOuter, zOuter);
                uvs[vert] = new Vector2(u, v);
                vert++;

                // Inner vertex
                float xInner = Mathf.Sin(phi) * Mathf.Cos(theta) * innerRadius;
                float yInner = Mathf.Cos(phi) * innerRadius;
                float zInner = Mathf.Sin(phi) * Mathf.Sin(theta) * innerRadius;
                vertices[vert] = new Vector3(xInner, yInner, zInner);
                uvs[vert] = new Vector2(u, v);
                vert++;
            }
        }

        // Generate triangles
        for (int y = 0; y < verticalSegments; y++)
        {
            for (int i = 0; i < radialSegments; i++)
            {
                int outer0 = (y * (radialSegments + 1) + i) * 2;
                int outer1 = outer0 + 2;
                int outer2 = outer0 + (radialSegments + 1) * 2;
                int outer3 = outer2 + 2;

                // Outer shell
                triangles[tri++] = outer0;
                triangles[tri++] = outer2;
                triangles[tri++] = outer1;

                triangles[tri++] = outer1;
                triangles[tri++] = outer2;
                triangles[tri++] = outer3;

                // Inner shell (flip winding)
                int inner0 = outer0 + 1;
                int inner1 = outer1 + 1;
                int inner2 = outer2 + 1;
                int inner3 = outer3 + 1;

                triangles[tri++] = inner0;
                triangles[tri++] = inner1;
                triangles[tri++] = inner2;

                triangles[tri++] = inner1;
                triangles[tri++] = inner3;
                triangles[tri++] = inner2;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        // Assign mesh
        MeshFilter mf = GetComponent<MeshFilter>();
        mf.mesh = mesh;

        MeshCollider mc = GetComponent<MeshCollider>();
        mc.sharedMesh = mesh;
        mc.convex = false;
    }
}
