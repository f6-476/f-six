using UnityEngine;
using UnityEngine.InputSystem;

public class Bank : MonoBehaviour
{
    private Controller controller;
    public float rudderScale = 0.2f;
    private void Start()
    {
        controller = GetComponentInParent<Controller>();
    }

    private void FixedUpdate()
    {
        if (controller.GetRudderValue() < -0.01)
        {
            var targetEulerRotation = Quaternion.Euler(new Vector3(0, -180f, -45));
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetEulerRotation, rudderScale); // * (rudderValue + 1f) / 2f);
        }
        else if (controller.GetRudderValue() > 0.01)
        {
            var targetEulerRotation = Quaternion.Euler(new Vector3(0, -180f, 45));
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetEulerRotation, rudderScale); // * (rudderValue + 1f) / 2f);
        }
        else
        {
            var targetEulerRotation = Quaternion.Euler(new Vector3(0, -180f, 0));
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetEulerRotation, rudderScale);
        }
    }

    /*public void GetRudder(InputAction.CallbackContext ctx)
    {
        rudderValue = ctx.ReadValue<float>();
    }*/
}
