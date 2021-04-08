using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothPathVisualization : MonoBehaviour
{
    [SerializeField] private Transform[] _points;
    [SerializeField] private float _smoothPower = 1;
    private BezierCurve[] _curves;

    private void OnDrawGizmos()
    {
        if (_points.Length < 2)
            return;

        Transform firstPoint = _points[0];
        Transform lastPoint = _points[_points.Length - 1];

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(firstPoint.position, firstPoint.position + firstPoint.forward);
        Gizmos.DrawLine(lastPoint.position, lastPoint.position + lastPoint.forward);

        Gizmos.color = Color.white;
        List<BezierCurve> curves = new List<BezierCurve>();
        for (int i = 0; i < _points.Length - 1; i++)
        {
            Transform currentTransform = _points[i];
            Transform nextTransform = _points[i + 1];
            Vector3 point = currentTransform.position;
            Vector3 nextPoint = nextTransform.position;

            Gizmos.DrawSphere(point, 0.1f);
            for (float t = 0; t < 1; t += 0.1f)
            {
                Gizmos.DrawSphere(Vector3.Lerp(point, nextPoint, t), 0.05f);
            }

            Vector3 controlPoint1;
            if (i == 0)
                controlPoint1 = point + currentTransform.forward * _smoothPower;
            else
            {
                Transform prevTransform = _points[i - 1];
                Vector3 prevPoint = prevTransform.position;
                Vector3 direction = ((point - prevPoint).normalized + (nextPoint - point).normalized).normalized;
                controlPoint1 = point + direction * _smoothPower;
            }

            Vector3 controlPoint2;
            if (i + 1 == _points.Length - 1)
                controlPoint2 = nextPoint + nextTransform.forward * -_smoothPower;
            else
            {
                Transform nextNextTransform = _points[i + 2];
                Vector3 nextNextPoint = nextNextTransform.position;
                Vector3 direction = ((nextPoint - nextNextPoint).normalized + (point - nextPoint).normalized).normalized;
                controlPoint2 = nextPoint + direction * _smoothPower;
            }

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(controlPoint1, 0.1f);
            Gizmos.DrawSphere(controlPoint2, 0.1f);
            Gizmos.color = Color.white;

            curves.Add(new BezierCurve(point, controlPoint1, controlPoint2, nextPoint));
        }

        Gizmos.DrawSphere(lastPoint.position, 0.1f);

        Gizmos.color = Color.blue;
        foreach (var curve in curves)
            for (float t = 0; t < 1; t += 0.025f)
                Gizmos.DrawSphere(curve.GetPoint(t), 0.05f);

    }
}
