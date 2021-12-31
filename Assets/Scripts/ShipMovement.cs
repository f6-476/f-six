using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    private Rigidbody _rigidbody;
    private Ship _ship;


    public float speed = 20f;
    public float boost = 1.5f;

    public float Speed
    {
        get => _ship.Boost ? speed * boost : speed;
    }

    public float VelocityPercent => _rigidbody.velocity.magnitude / speed;
    void Start()
    {
        _ship = GetComponent<Ship>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {

        _rigidbody.AddForce(transform.forward * (Speed * _ship.ThrustValue), ForceMode.Acceleration);

    }

    

}
