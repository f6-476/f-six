using UnityEngine;

[
    RequireComponent(typeof(ShipInfo)),
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
    [SerializeField] private ShipInfo _info;
    [SerializeField] private ShipView _view;

    public ShipMovement Movement => _movement;
    public ShipHover Hover => _hover;
    public ShipAudio Audio => _audio;
    public ShipInfo Info => _info;
    public ShipView View => _view;
    
    //TODO: Ship stats --> scriptable object to set values (speed, maneuverability, etc...)
    //TODO: State machine for animation?

    // Properties
    public Rigidbody Rigidbody => _rigidbody;
    public float RudderValue  => _controller.GetRudderValue();
    public float ThrustValue => _controller.GetThrustValue();
    public bool Boost => _controller.GetBoost();
}
