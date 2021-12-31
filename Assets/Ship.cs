using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    private Controller _controller;
    public float RudderValue  => _controller.GetRudderValue();
    public float ThrustValue => _controller.GetThrustValue();

    public bool Boost => _controller.GetBoost();

    private void Start()
    {
        _controller = GetComponent<Controller>();
    }
}
