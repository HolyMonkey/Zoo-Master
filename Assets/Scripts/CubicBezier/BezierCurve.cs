using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve
{
    private Vector3 _p0;
    private Vector3 _p1;
    private Vector3 _p2;
    private Vector3 _p3;

    public BezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        _p0 = p0;
        _p1 = p1;
        _p2 = p2;
        _p3 = p3;
    }

    public Vector3 GetPoint(float t)
    {
        return Mathf.Pow(1 - t, 3) * _p0 +
                3 * Mathf.Pow(1 - t, 2) * t * _p1 +
                3 * (1 - t) * Mathf.Pow(t, 2) * _p2 +
                Mathf.Pow(t, 3) * _p3;
    }

    public float GetLength()
    {
        float length = 0;
        Vector3 lastPoint = _p0;
        for (float t = 1; t <= 1; t += 0.02f)
        {
            Vector3 point = GetPoint(t);
            length += Vector3.Distance(point, lastPoint);
            lastPoint = point;
        }
        return length;
    }
}
