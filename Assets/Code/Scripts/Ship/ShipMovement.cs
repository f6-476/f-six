using System.Collections;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    private Ship _ship;

    public float speed = 20f;
    [Range(0.0f, 2.0f)]
    public float thrustMultiplier = 1.0f;
    public float boost = 1.5f;

    public float Speed => _ship.Boost ? speed * boost : speed;

    public float VelocityPercent => _ship.Rigidbody.velocity.magnitude / speed;

    private void Awake() 
    {
        _ship = GetComponent<Ship>();    
    }

    private void FixedUpdate()
    {
        _ship.Rigidbody.AddForce(transform.forward * (Speed * _ship.ThrustValue * thrustMultiplier), ForceMode.Acceleration);
    }

    public void SetThrust()
    {
        StartCoroutine(DisableShip());
    }

    private IEnumerator DisableShip()
    {
        thrustMultiplier = 0f;
        yield return new WaitForSeconds(3f);
        thrustMultiplier = 1f;
    }
}
