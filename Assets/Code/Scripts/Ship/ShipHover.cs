using System.Collections.Generic;
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

            var turn = Quaternion.AngleAxis(_ship.RudderValue * 10f, transform.up);
            var align = Quaternion.FromToRotation(transform.up, SampleCornersAverage());
           
            //Debug.Log("Normal: " + normal + " || Magnitude: " + normal.magnitude);
            /*_ship.Rigidbody.MoveRotation(align * _ship.Rigidbody.rotation);
            _ship.Rigidbody.MoveRotation(turn * _ship.Rigidbody.rotation);*/
            
            var rot = Quaternion.Slerp(_ship.Rigidbody.rotation, align * _ship.Rigidbody.rotation,  .4f);
            rot = Quaternion.Slerp(rot, turn  * rot,  .4f);

            var quat = rot * Quaternion.Inverse(_ship.Rigidbody.rotation);
            
            _ship.Rigidbody.AddTorque(quat.x * 100,quat.y * 100,quat.z * 100,ForceMode.Acceleration);

        }
        
        
    }

    

    public Vector3 SampleCornersAverage()
    {
        Vector3 norm = Vector3.zero;
        if (Time.time < 1) return transform.up;
        List<RaycastHit> hits = new List<RaycastHit>();
        hits.Clear();
        
        //front right
        var direction = -transform.up + transform.forward * 1 + transform.right * 1;
        if(Physics.Raycast(transform.position, direction, out RaycastHit hit1, 10, trackLayer))
            hits.Add(hit1);
        
        //front left
        direction = -transform.up              + transform.forward * 1 - transform.right * 1;
        if(Physics.Raycast(transform.position, direction, out RaycastHit hit2, 10, trackLayer))
            hits.Add(hit1);

        //back right
        direction = -transform.up              - transform.forward * 1 + transform.right * 1;
        if(Physics.Raycast(transform.position, direction, out RaycastHit hit3, 10, trackLayer))
            hits.Add(hit3);
        
        //back left
        direction = -transform.up              - transform.forward * 1 - transform.right * 1;
        if(Physics.Raycast(transform.position, direction, out RaycastHit hit4, 10, trackLayer))
            hits.Add(hit4);

        
        if (hits.Count > 0)
        {
            foreach (var hit in hits)
            {
                norm += (BarycentricCoordinateInterpolator.GetInterpolatedNormal(hit));
                Debug.DrawRay(transform.position, hit.point - transform.position);
            }
            norm = norm / hits.Count;
        }
        else 
            norm = transform.up;
        
        return norm;

    }
}

