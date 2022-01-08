using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Ship))]
public class ShipController : Controller
{
    
    public void GetRudder(InputAction.CallbackContext ctx)
    {
        rudderValue = ctx.ReadValue<float>();
    }

    public void GetThrust(InputAction.CallbackContext ctx)
    {
        thrustValue = ctx.ReadValue<float>();
    }

    public void GetBoost(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log("boost true");
            boost = true;
        }
        if (ctx.canceled)
        {
            Debug.Log("boost false");
            boost = false;
        }
    }
    
}
