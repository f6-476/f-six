using UnityEngine;

[RequireComponent(typeof(Ship))]
public class Controller : MonoBehaviour
{
    [SerializeField] protected Ship ship;
    protected float rudderValue, thrustValue, reverseValue = 0;
    protected bool fire;

    public float GetRudderValue()
    {
        return rudderValue;
    }

    public float GetThrustValue()
    {
        return thrustValue;
    }

    public float GetReverseValue()
    {
        return reverseValue;
    }

    public bool GetFire()
    {
        return fire;
    }
}
