using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    private Controller _controller;
    private ShipMovement _movement;
    private ShipHover _hover;
    private ShipAudio _audio;
    private Rigidbody _rigidbody;

    public ShipMovement Movement => _movement;
    public ShipHover Hover => _hover;
    public ShipAudio Audio => _audio;
    
    
    // Properties
    public Rigidbody Rigidbody => _rigidbody;
    public float RudderValue  => _controller.GetRudderValue();
    public float ThrustValue => _controller.GetThrustValue();
    public bool Boost => _controller.GetBoost();
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _controller = GetComponent<Controller>();
        _movement = GetComponent<ShipMovement>();
        _hover = GetComponent<ShipHover>();
        _audio = GetComponent<ShipAudio>();
    }
}
