using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Ship))]
public class ShipHover : MonoBehaviour
{
    private Ship _ship;
    public LayerMask trackLayer;
    public float hoverHeight = 4;

    public bool tunePID = false;

    [SerializeField] private float _gravity = 9.8f;
    [SerializeField]
    [Range(-100f, 100f)]
    private float p, i, d;
    private PID _hoverPidController;

    private void Awake()
    {
        
    }

    private void Start()
    {
        _ship = GetComponent<Ship>();
        _hoverPidController = new PID(p, i, d);
    }

    private float lastPrintTime = 0;
    private void FixedUpdate()
    {
        //var normal = Vector3.zero;
        if (tunePID)
        {
            _hoverPidController.Kp = p;
            _hoverPidController.Ki = i;
            _hoverPidController.Kd = d;
        }

        
        //var direction = Vector3.zero;
        Debug.DrawRay(transform.position,-transform.up,Color.red);
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 5,trackLayer))
        {
            //Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 5)
            
            var normal = BarycentricCoordinateInterpolator.GetInterpolatedNormal(hit);
            var direction = (transform.position - hit.point).normalized;
         
            Debug.DrawLine(transform.position,hit.point,Color.green);
        
            _ship.Rigidbody.AddForce(-_gravity * direction, ForceMode.Acceleration);
            _ship.Rigidbody.AddForce(normal * _hoverPidController.GetOutput((hoverHeight - hit.distance), Time.fixedDeltaTime));
            //_ship.Rigidbody.rotation = Quaternion.Slerp(Quaternion.FromToRotation(transform.up, normal) * Quaternion.AngleAxis(_ship.RudderValue * 2f, transform.up) * _ship.Rigidbody.rotation);

            var turn = Quaternion.AngleAxis(_ship.RudderValue * 2f, transform.up);
            var align = Quaternion.FromToRotation(transform.up, SampleCornersAverage());
           
            //Debug.Log("Normal: " + normal + " || Magnitude: " + normal.magnitude);
            /*_ship.Rigidbody.MoveRotation(align * _ship.Rigidbody.rotation);
            _ship.Rigidbody.MoveRotation(turn * _ship.Rigidbody.rotation);*/
            _ship.Rigidbody.rotation = Quaternion.Slerp(_ship.Rigidbody.rotation, align * _ship.Rigidbody.rotation,  .2f);
            _ship.Rigidbody.rotation = Quaternion.Slerp(_ship.Rigidbody.rotation, turn  * _ship.Rigidbody.rotation,  .5f);
        }
        
    }

    public RaycastHit SampleRayCast(Vector3 direction)
    {
        Physics.Raycast(transform.position, direction, out RaycastHit hit, 10, trackLayer);
        return hit;
    }

    public Vector3 SampleCornersAverage()
    {
        //front right
        var hit1 =  SampleRayCast(-transform.up + transform.forward * 2 + transform.right * 2);
        Debug.DrawRay(transform.position, hit1.point-transform.position);
        //front left
        var hit2 =  SampleRayCast(-transform.up + transform.forward * 2 - transform.right * 2);
        Debug.DrawRay(transform.position, hit2.point-transform.position);
        //back right
        var hit3 =  SampleRayCast(-transform.up - transform.forward * 2 + transform.right * 2);
        Debug.DrawRay(transform.position, hit3.point-transform.position);
        //back right
        var hit4 =  SampleRayCast(-transform.up - transform.forward * 2 - transform.right * 2);
        Debug.DrawRay(transform.position, hit4.point-transform.position);

        var norm = (BarycentricCoordinateInterpolator.GetInterpolatedNormal(hit1) 
                    + BarycentricCoordinateInterpolator.GetInterpolatedNormal(hit2)
                    + BarycentricCoordinateInterpolator.GetInterpolatedNormal(hit3)
                    + BarycentricCoordinateInterpolator.GetInterpolatedNormal(hit4))*0.25f;
        return norm;

    }
}

