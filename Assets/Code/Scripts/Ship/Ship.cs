using System;
using UnityEngine;

[
    RequireComponent(typeof(ShipRace)),
    RequireComponent(typeof(ShipMovement)),
    RequireComponent(typeof(ShipHover)),
    RequireComponent(typeof(ShipAudio)),
    RequireComponent(typeof(Rigidbody))
]
public class Ship : MonoBehaviour
{
    [SerializeField] private Controller _controller;
    [SerializeField] ShipMovement _movement;
    [SerializeField] private ShipHover _hover;
    [SerializeField] private ShipAudio _audio;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private ShipRace _race;

    public ShipMovement Movement => _movement;
    public ShipHover Hover => _hover;
    public ShipAudio Audio => _audio;
    public ShipRace Race => _race;
    public ShipShields Shields { get; set; }

    //TODO: Ship stats --> scriptable object to set values (speed, maneuverability, etc...)
    //TODO: State machine for animation?

    // Properties
    public Rigidbody Rigidbody => _rigidbody;
    public float RudderValue  => _controller.GetRudderValue();
    public float ThrustValue => _controller.GetThrustValue();
    public bool Boost => _controller.GetBoost();

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject);
    }
}
