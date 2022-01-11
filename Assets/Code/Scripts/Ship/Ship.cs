using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    [SerializeField] private Controller _controller;
    [SerializeField] private ShipMovement _movement;
    [SerializeField] private ShipHover _hover;
    [SerializeField] private ShipAudio _audio;
    [SerializeField] private Rigidbody _rigidbody;

    public ShipMovement Movement => _movement;
    public ShipHover Hover => _hover;
    public ShipAudio Audio => _audio;
    
    //TODO: Ship stats --> scriptable object to set values (speed, maneuverability, etc...)
    //TODO: State machine for animation?

    // Properties
    public Rigidbody Rigidbody => _rigidbody;
    public float RudderValue  => _controller.GetRudderValue();
    public float ThrustValue => _controller.GetThrustValue();
    public bool Boost => _controller.GetBoost();
}
