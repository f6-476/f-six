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
            _ship.Rigidbody.rotation = Quaternion.Slerp(_ship.Rigidbody.rotation, align * _ship.Rigidbody.rotation,  .3f);
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
        List<RaycastHit> hits = new List<RaycastHit>();
        hits.Clear();
        
        //front right
        hits.Add(SampleRayCast(-transform.up + transform.forward * 1 + transform.right * 1));
        
        //front left
        hits.Add(SampleRayCast(-transform.up + transform.forward * 1 - transform.right * 1));
        
        //back right
        hits.Add(SampleRayCast(-transform.up - transform.forward * 1 + transform.right * 1));
        
        //back right
        hits.Add(SampleRayCast(-transform.up - transform.forward * 1 - transform.right * 1));

        Vector3 norm = Vector3.zero;
        if (hits.Count > 0)
        {
            foreach (var hit in hits)
            {
                norm += (BarycentricCoordinateInterpolator.GetInterpolatedNormal(hit));
                Debug.DrawRay(transform.position, hit.point - transform.position);
            }
            norm = norm / hits.Count;
        }
        else norm = transform.up;
        
        return norm;

    }
}

