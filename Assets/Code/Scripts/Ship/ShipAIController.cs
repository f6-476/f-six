using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Ship))]
public class ShipAIController : Controller
{
    private void Update()
    {
        thrustValue = 1f;
    }

    private void GetBoost(InputAction.CallbackContext ctx)
    {
        /*if (ctx.performed)
        {
            Debug.Log("boost true");
            boost = true;
        }
        if (ctx.canceled)
        {
            Debug.Log("boost false");
            boost = false;
        }*/
    }
}
