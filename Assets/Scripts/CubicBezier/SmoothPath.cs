using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothPath
{
    private Vector3 _pointA;
    private Vector3 _pointB;
    private Vector3 _directionA;
    private Vector3 _directionB;
    private Vector3[] _pointsBetween;
    private float _smoothPower;

    private List<BezierCurve> _curves = new List<BezierCurve>();
    private List<float> _curvesLength = new List<float>();

    public SmoothPath(Vector3 pointA, Vector3 pointB, Vector3[] pointsBetween, Vector3 directionA, Vector3 directionB, float smoothPower = 1)
    {
        _pointA = pointA;
        _pointB = pointB;
        _pointsBetween = pointsBetween;
        _directionA = directionA.normalized;
        _directionB = directionB.normalized;
        _smoothPower = smoothPower;
        Init();
    }

    private void Init()
    {
        _curves.Add(CreateStartCurve());

        for (int i = 0; i < _pointsBetween.Length - 1; i++)
        {
            Vector3 prevPoint = i == 0 ? _pointA : _pointsBetween[i - 1];
            Vector3 currentPoint = _pointsBetween[i];
            Vector3 nextPoint = _pointsBetween[i + 1];
            Vector3 nextNextPoint = i == _pointsBetween.Length - 2 ? _pointB : _pointsBetween[i + 2];

            Vector3 controlPoint1Direction = ((currentPoint - prevPoint).normalized + (nextPoint - currentPoint).normalized).normalized;
            Vector3 controlPoint2Direction = ((nextPoint - nextNextPoint).normalized + (currentPoint - nextPoint).normalized).normalized;
            Vector3 controlPoint1 = currentPoint + controlPoint1Direction * _smoothPower;
            Vector3 controlPoint2 = nextPoint + controlPoint2Direction * _smoothPower;

            _curves.Add(new BezierCurve(currentPoint, controlPoint1, controlPoint2, nextPoint));
        }

        if (_pointsBetween.Length > 0)
            _curves.Add(CreateEndCurve());


        foreach (var curve in _curves)
        {

        }
    }

    private BezierCurve CreateStartCurve()
    {
        Vector3 controlPoint1 = _pointA + _directionA.normalized * _smoothPower;

        Vector3 nextPoint = _pointB;
        Vector3 controlPoint2;
        if (_pointsBetween.Length == 0)
        {
            controlPoint2 = _pointB - _directionB * _smoothPower;
        }
        else
        {
            nextPoint = _pointsBetween[0];
            Vector3 nextNextPoint;
            if (_pointsBetween.Length > 1)
                nextNextPoint = _pointsBetween[1];
            else
                nextNextPoint = _pointB;

            Vector3 direction = ((nextNextPoint - nextPoint).normalized + (nextPoint - _pointA).normalized).normalized;
            controlPoint2 = nextPoint + direction * _smoothPower;
        }

        return new BezierCurve(_pointA, controlPoint1, controlPoint2, nextPoint);    
    }

    private BezierCurve CreateEndCurve()
    {
        Vector3 controlPoint2 = _pointB + _directionB * -_smoothPower;

        Vector3 prevPoint = _pointsBetween[_pointsBetween.Length - 1];
        Vector3 prevPrevPoint;
        if (_pointsBetween.Length == 1)
            prevPrevPoint = _pointA;
        else
            prevPrevPoint = _pointsBetween[_pointsBetween.Length - 2];

        Vector3 direction = ((prevPoint - prevPrevPoint).normalized + (_pointB - prevPoint)).normalized;
        Vector3 controlPoint1 = prevPoint + direction * _smoothPower;

        return new BezierCurve(prevPoint, controlPoint1, controlPoint2, _pointB);
    }
}
