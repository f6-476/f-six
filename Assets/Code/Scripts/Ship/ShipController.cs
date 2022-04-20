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

    public void GetReverse(InputAction.CallbackContext ctx)
    {
        reverseValue = ctx.ReadValue<float>();
    }

    public void GetFire(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            ship.PowerUp.ActivatePowerUp();
        }
    }
}
