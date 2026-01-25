using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Line : MonoBehaviour {
    [SerializeField] private LineRenderer _renderer;
    private readonly List<Vector3> _points = new();

    void Start() {
        _renderer = GetComponent<LineRenderer>();
    }
 
 
    void Update()
    {
 
    }
 
    public void SetPosition(Vector3 pos)
    {
        if (!CanAppend(pos)) return;

        _points.Add(pos);

        _renderer.positionCount++;
        _renderer.SetPosition(_renderer.positionCount - 1, pos);
    }

    private bool CanAppend(Vector3 pos)
    {
        if (_renderer.positionCount == 0) return true;

        return Vector3.Distance(
            _renderer.GetPosition(_renderer.positionCount - 1),
            pos
        ) > DrawPathGenerator.RESOLUTION;
    }
}
