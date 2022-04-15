using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] protected Ship _ship;
    protected float rudderValue, thrustValue = 0;
    protected bool boost, fire;
    public float GetRudderValue()
    {
        return rudderValue;
    }

    public float GetThrustValue()
    {
        return thrustValue;
    }

    public bool GetBoost()
    {
        return boost;
    }

    public bool GetFire()
    {
        return fire;
    }
}
