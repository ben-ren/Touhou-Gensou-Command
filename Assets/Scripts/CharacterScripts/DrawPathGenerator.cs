using UnityEngine;
using UnityEngine.Splines;

public class DrawPathGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TokenController token;
    [SerializeField] private CursorFollowPoint cursor;

    [Header("Spline Prefab")]
    [SerializeField] private GameObject splinePrefab; // drag your "Spline" prefab here
    [SerializeField] private Material pathMaterial;   // assign default material
    [SerializeField] private float pathRadius = 0.15f;

    public const float RESOLUTION = 1f;

    private GameObject currentPath;
    private SplineContainer currentSplineContainer;
    private Spline spline;

    private InputController IC;
    private Vector3 lastPoint;
    private bool wasClicking;
    private bool isDrawing;

    void Start()
    {
        IC = InputController.instance;
    }

    void Update()
    {
        DrawSplinePath();

        if (currentSplineContainer != null && token.GetIsTravellingAlongPath())
        {
            token.AttachToPath(currentSplineContainer);
        }

        token.ResetClicked();
    }

    private void DrawSplinePath()
    {
        bool clicking = IC.GetClickState() > 0f;

        if (clicking && !wasClicking && token.GetIsClicked())
        {
            CreateNewSpline(pathMaterial); // pass material here
            AddPoint(cursor.transform.position);
            isDrawing = true;
        }

        if (clicking && isDrawing && spline != null)
        {
            Vector3 pos = cursor.transform.position;
            pos.z = 0f;

            if (Vector3.Distance(lastPoint, pos) >= RESOLUTION)
            {
                AddPoint(pos);
            }
        }

        // Stop drawing when click ends
        if (!clicking && isDrawing)
        {
            isDrawing = false;
        }

        wasClicking = clicking;
    }

    private void CreateNewSpline(Material material)
    {
        // Clear old path
        if (currentPath != null) Destroy(currentPath);

        // Instantiate prefab
        currentPath = Instantiate(splinePrefab, transform);
        currentPath.name = "RuntimeSplinePath";
        currentPath.transform.position = new Vector3(0, 0, -1f);

        // Assign material
        var renderer = currentPath.GetComponent<MeshRenderer>();
        if (renderer != null && material != null) renderer.material = material;

        // Use the existing spline in the prefab
        currentSplineContainer = currentPath.GetComponent<SplineContainer>();
        if (currentSplineContainer == null || currentSplineContainer.Splines.Count == 0)
        {
            Debug.LogError("Spline prefab missing SplineContainer or initial spline!");
            return;
        }

        spline = currentSplineContainer.Splines[0]; // <- USE EXISTING SPLINE

        // SplineExtrude radius
        var extrude = currentPath.GetComponent<SplineExtrude>();
        if (extrude != null) extrude.Radius = pathRadius;

        spline.Clear();     // Clear any existing knots from prefab spline
    }


    private void AddPoint(Vector3 pos)
    {
        pos.z = 0f;

        BezierKnot knot = new BezierKnot(pos);

        int count = spline.Count;

        knot.TangentIn = Vector3.zero;
        knot.TangentOut = Vector3.zero;

        // ---- Set rotation for XY plane ----
        knot.Rotation = Quaternion.Euler(270f, 270f, 90f);

        // ---- Add to spline ----
        spline.Add(knot);
        lastPoint = pos;
    }
}
