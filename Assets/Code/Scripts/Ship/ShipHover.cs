using UnityEngine;

[RequireComponent(typeof(Ship))]
public class ShipHover : MonoBehaviour
{
    private Ship _ship;

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
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, Mathf.Infinity))
        {
            var normal = BarycentricCoordinateInterpolator.GetInterpolatedNormal(hit);
            var direction = (transform.position - hit.point).normalized;
         
            Debug.DrawLine(transform.position,hit.point,Color.green);
        
            _ship.Rigidbody.AddForce(-_gravity * direction, ForceMode.Acceleration);
            _ship.Rigidbody.AddForce(normal * _hoverPidController.GetOutput((hoverHeight - hit.distance), Time.fixedDeltaTime));
            _ship.Rigidbody.MoveRotation(Quaternion.FromToRotation(transform.up, normal) * Quaternion.AngleAxis(_ship.RudderValue * 2f, transform.up) * _ship.Rigidbody.rotation);
        }

        
        
    }
}

