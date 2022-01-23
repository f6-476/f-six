using UnityEngine;
using UnityEngine.InputSystem;

public class Bank : MonoBehaviour
{
    private float rudderValue = 0;
    public float rudderScale = 0.7f;
    private void Start()
    {

    }

    private void FixedUpdate()
    {
        if (rudderValue < -0.01)
        {
            var targetEulerRotation = Quaternion.Euler(new Vector3(0, -180f, -45));
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetEulerRotation, rudderScale); // * (rudderValue + 1f) / 2f);
        }
        else if (rudderValue > 0.01)
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

    public void GetRudder(InputAction.CallbackContext ctx)
    {
        rudderValue = ctx.ReadValue<float>();
    }
}
