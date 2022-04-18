using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Hover : MonoBehaviour
{
    [SerializeField] private LayerMask trackLayer;
    [SerializeField] private float hoverHeight = 4;
    public bool tunePID = false;
    [SerializeField] private float gravity = 9.8f;
    protected new Rigidbody rigidbody;
    [SerializeField]
    [Range(-100f, 100f)]
    private float p, i, d;
    private PID hoverPidController;
    public float Rudder { get; set; }
    private static readonly float MAX_FLOOR_DISTANCE = Mathf.Infinity;
    private static readonly float RUDDER_MULTIPLIER = 10.0f;
    private static readonly float ROTATION_MULTIPLIER = 0.4f;
    private float maxCorrectionForce;

    protected virtual void Awake()
    {
        hoverPidController = new PID(p, i, d);
        maxCorrectionForce = gravity * 10.0f;
    }

    protected virtual void Start()
    {
        this.rigidbody = GetComponent<Rigidbody>();
    }

    protected virtual void Update()
    {
        if (tunePID)
        {
            hoverPidController.Kp = p;
            hoverPidController.Ki = i;
            hoverPidController.Kd = d;
        }
    }

    protected virtual void FixedUpdate()
    {
        UpdatePhysics();
    }

    protected void UpdatePhysics()
    {
        // Cast relative down (should be valid most of the time).
        // If that fails, cast in other 5 directions to try to flip the ship.
        Vector3[] castDirections = new Vector3[] { -transform.up, transform.forward, -transform.forward, transform.right, -transform.right, transform.up };

        foreach (Vector3 castDirection in castDirections)
        {
            Debug.DrawRay(transform.position, castDirection * MAX_FLOOR_DISTANCE, Color.red);

            RaycastHit hit;
            if (!Physics.Raycast(transform.position, castDirection, out hit, MAX_FLOOR_DISTANCE, trackLayer)) continue;

            Vector3? cornerNormal = GetAverageCornerNormal();

            Vector3 normal = hit.normal;
            if (cornerNormal.HasValue)
            {
                normal = cornerNormal.Value;
                Debug.DrawRay(transform.position,-normal,Color.green);
            }

            RaycastHit hit2;
            if (!Physics.Raycast(transform.position, -normal, out hit2, MAX_FLOOR_DISTANCE, trackLayer)) break;

            Quaternion turn = Quaternion.AngleAxis(Rudder * RUDDER_MULTIPLIER, transform.up);
            Quaternion align = Quaternion.FromToRotation(transform.up, normal);

            rigidbody.AddForce(gravity * -normal, ForceMode.Acceleration);
            rigidbody.AddForce(normal * Mathf.Clamp(hoverPidController.GetOutput(hoverHeight - hit2.distance, Time.fixedDeltaTime), -maxCorrectionForce, maxCorrectionForce), ForceMode.Acceleration);

            Debug.DrawRay(hit2.point - transform.forward, transform.up * hoverHeight, Color.yellow);
            
            Quaternion rot = Quaternion.Slerp(rigidbody.rotation, align * rigidbody.rotation, ROTATION_MULTIPLIER);
            rot = Quaternion.Slerp(rot, turn * rot, ROTATION_MULTIPLIER);

            Quaternion quat = rot * Quaternion.Inverse(rigidbody.rotation);
            rigidbody.AddTorque(quat.x * 100, quat.y * 100, quat.z * 100, ForceMode.Acceleration);

            break;
        }
    }

    protected Vector3? GetAverageCornerNormal()
    {
        if (Time.time < 1) return null;

        List<RaycastHit> hits = new List<RaycastHit>();

        (int, int)[] deltas = new (int, int)[]{(1, 1), (1, -1), (-1, 1), (-1, -1)};
        foreach (var (dx, dy) in deltas)
        {
            Vector3 direction = -transform.up + transform.forward * dx + transform.right * dy;
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, 10, trackLayer))
            {
                hits.Add(hit);
                Debug.DrawLine(transform.position,hit.point);
            }
        }

        if (hits.Count > 0)
        {
            Vector3 norm = Vector3.zero;

            foreach (var hit in hits)
            {
                norm += (BarycentricCoordinateInterpolator.GetInterpolatedNormal(hit));
            }

            norm = norm / hits.Count;

            return norm;
        }
        else
        {
            return null;
        }
    }
}
