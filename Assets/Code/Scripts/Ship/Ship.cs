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
    private Controller _controller;
    private ShipMovement _movement;
    private ShipHover _hover;
    private ShipAudio _audio;
    private Rigidbody _rigidbody;
    private ShipInfo _info;

    private void Awake() 
    {
        _controller = GetComponent<Controller>();
        _movement = GetComponent<ShipMovement>();
        _hover = GetComponent<ShipHover>();
        _audio = GetComponent<ShipAudio>();
        _rigidbody = GetComponent<Rigidbody>();
        _info = GetComponent<ShipInfo>();
    }

    public ShipMovement Movement => _movement;
    public ShipHover Hover => _hover;
    public ShipAudio Audio => _audio;
    public ShipInfo Info => _info;
    
    //TODO: Ship stats --> scriptable object to set values (speed, maneuverability, etc...)
    //TODO: State machine for animation?

    // Properties
    public Rigidbody Rigidbody => _rigidbody;
    public float RudderValue  => _controller.GetRudderValue();
    public float ThrustValue => _controller.GetThrustValue();
    public bool Boost => _controller.GetBoost();
}
