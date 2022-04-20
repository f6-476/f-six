using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Ship))]
public class ShipMovement : MonoBehaviour
{
    [SerializeField] private Ship ship;
    [SerializeField] private float speed = 20f;
    [Range(0.0f, 2.0f)] public float thrustMultiplier = 1.0f;
    public float reverseMultiplier = 1.0f;
    public float rotationMultiplier = 1.0f;

    public float VelocityPercent => ship.Rigidbody.velocity.magnitude / speed;

    private void FixedUpdate()
    {
        ship.Rigidbody.AddForce(transform.forward * (speed * ship.ThrustValue * thrustMultiplier), ForceMode.Acceleration);
        ship.Rigidbody.AddForce(-transform.forward * (speed * ship.ReverseValue * reverseMultiplier), ForceMode.Acceleration);
    }
}
