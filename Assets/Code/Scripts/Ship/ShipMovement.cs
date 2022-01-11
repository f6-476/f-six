using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    [SerializeField] private Ship _ship;

    public float speed = 20f;
    public float boost = 1.5f;

    public float Speed => _ship.Boost ? speed * boost : speed;

    public float VelocityPercent => _ship.Rigidbody.velocity.magnitude / speed;

    private void FixedUpdate()
    {
        _ship.Rigidbody.AddForce(transform.forward * (Speed * _ship.ThrustValue), ForceMode.Acceleration);
    }
}
