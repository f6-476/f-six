using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Ship))]
public class ShipMovement : MonoBehaviour
{
    [SerializeField] private Ship ship;
    public float speed = 20f;
    [Range(0.0f, 2.0f)] public float thrustMultiplier = 1.0f;
    public float boost = 1.5f;

    public float Speed => ship.Boost ? speed * boost : speed;

    public float VelocityPercent => ship.Rigidbody.velocity.magnitude / speed;

    private void FixedUpdate()
    {
        if (!ship.PowerUp.Disabled)
        {
            ship.Rigidbody.AddForce(transform.forward * (Speed * ship.ThrustValue * thrustMultiplier), ForceMode.Acceleration);
        }
    }
}
