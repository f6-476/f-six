using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Ship))]
public class ShipHover : Hover
{
    private Ship ship;

    protected override void Start()
    {
        base.Start();
        this.ship = GetComponent<Ship>();
        this.rigidbody = this.ship.Rigidbody;
    }

    protected override void Update()
    {
        base.Update();
        Rudder = ship.RudderValue;
    }
}

 