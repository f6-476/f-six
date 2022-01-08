using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = System.Numerics.Vector2;

public class Bank : MonoBehaviour
{
    private float rudderValue = 0;
    public float rudderScale = 0.7f;
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(rudderValue < -0.01)
        {
            Debug.Log(rudderValue);
            var targetEulerRotation = Quaternion.Euler( new Vector3(0, -180f, -45));
            transform.localRotation = Quaternion.Slerp(transform.localRotation,targetEulerRotation,rudderScale); // * (rudderValue + 1f) / 2f);
        }
        else if(rudderValue > 0.01)
        {
            Debug.Log(rudderValue);
            var targetEulerRotation = Quaternion.Euler( new Vector3(0, -180f, 45));
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetEulerRotation, rudderScale); // * (rudderValue + 1f) / 2f);
        }
        else
        {
            var targetEulerRotation = Quaternion.Euler( new Vector3(0, -180f, 0));
            transform.localRotation = Quaternion.Slerp(transform.localRotation,targetEulerRotation, rudderScale);
        }

    }
    
    
    
    public void GetRudder(InputAction.CallbackContext ctx)
    {
        rudderValue = ctx.ReadValue<float>();
        
        
    }
}
