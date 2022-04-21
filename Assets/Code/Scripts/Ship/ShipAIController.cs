using UnityEngine;

[RequireComponent(typeof(Ship))]
public class ShipAIController : Controller
{
    private void Update()
    {
    }

    public void SetRudder(float value)
    {
        rudderValue = value;
    }
    
    public void SetThrust(float value)
    {
        thrustValue = value;
    }

    public void SetReverse(float value)
    {
        reverseValue = value;
    }

    public void SetBoost(bool value)
    {
        boost = value;
    }
    
    public void Fire()
    {
        _ship.PowerUp.OnActivate();
    }


    
    
}
