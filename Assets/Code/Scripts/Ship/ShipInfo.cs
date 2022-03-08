using System;
using UnityEngine;

[RequireComponent(typeof(Ship))]
public class ShipInfo : MonoBehaviour
{
    [SerializeField] private Transform _transform;
    public int CurrentRank { get; set; }
    private float _totalDistance;
    private Vector3 _previousPosition;

    private void Start()
    {
        _previousPosition = _transform.position;
    }

    public float TrackDistance()
    {
        var currentPosition = _transform.position;
        var deltaDistance = (currentPosition - _previousPosition).magnitude;
        _totalDistance += deltaDistance;
        _previousPosition = currentPosition;
        
        return _totalDistance;
    }
}
