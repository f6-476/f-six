
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = System.Numerics.Vector2;

public class ShipHover : MonoBehaviour
{
    private Rigidbody _rigidbody;
    [SerializeField]
    private Transform level;
    public float hoverHeight;
    [SerializeField]
    private float force = 100f;

    private float rudderValue, thrustValue = 0;

    [SerializeField] [Range(-100f, 100f)] 
    private float p, i, d;
    private PID hoverPIDController;
    public float speed = 20f;

    public float VelocityPercent => _rigidbody.velocity.magnitude / speed;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        hoverHeight = 4;
        hoverPIDController = new PID(p,i,d);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        hoverPIDController.Kp = p;
        hoverPIDController.Ki = i;
        hoverPIDController.Kd = d;

        _rigidbody.AddForce(transform.forward * speed * thrustValue, ForceMode.Acceleration);

        RaycastHit hit;
        if (Physics.Raycast(transform.position ,transform.TransformDirection(Vector3.down) ,out hit, Mathf.Infinity))
        {
            Vector3 interpolatedNormal = BarycentricCoordinateInterpolator.GetInterpolatedNormal(hit);
           //Transform hitTransform = hit.collider.transform;
           //interpolatedNormal = hitTransform.TransformDirection(interpolatedNormal);

           // Display with Debug.DrawLine
           Debug.DrawRay(hit.point, interpolatedNormal);
          Debug.DrawLine(hit.point,hit.point + interpolatedNormal * 3f);
          var direction = (transform.position - hit.point).normalized;
            _rigidbody.AddForce(-80.8f * direction,ForceMode.Acceleration);
            _rigidbody.AddForce(interpolatedNormal * hoverPIDController.GetOutput((hoverHeight - hit.distance),Time.fixedDeltaTime));
            _rigidbody.MoveRotation(Quaternion.AngleAxis(rudderValue * 2f,transform.up) * Quaternion.FromToRotation(transform.up, interpolatedNormal) * _rigidbody.rotation);
        }
        else
        {
            var direction = (transform.position - level.position).normalized;
            _rigidbody.AddForce(-9.8f * direction,ForceMode.Acceleration);
        }
        

    }

    public void GetRudder(InputAction.CallbackContext ctx)
    {
        rudderValue = ctx.ReadValue<float>();
    }

    public void GetThrust(InputAction.CallbackContext ctx)
    {
        thrustValue = ctx.ReadValue<float>();
    }

  
}

