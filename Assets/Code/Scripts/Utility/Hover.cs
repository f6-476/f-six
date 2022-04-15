using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Hover : MonoBehaviour
{
    [SerializeField] private LayerMask trackLayer;
    [SerializeField] private float hoverHeight = 4;
    private bool tunePID = false;
    [SerializeField] private float gravity = 9.8f;
    protected new Rigidbody rigidbody;
    [SerializeField]
    [Range(-100f, 100f)]
    private float p, i, d;
    private PID hoverPidController;
    public float Rudder { get; set; }
    private static readonly float MAX_FLOOR_DISTANCE = 5.0f;
    private static readonly float RUDDER_MULTIPLIER = 10.0f;
    private static readonly float ROTATION_MULTIPLIER = 0.4f;

    protected virtual void Awake()
    {
        hoverPidController = new PID(p, i, d);
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
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, MAX_FLOOR_DISTANCE, trackLayer))
        {
            Vector3? cornerNormal = GetAverageCornerNormal();

            Vector3 normal = hit.normal;
            if (cornerNormal.HasValue) normal = cornerNormal.Value;

            Vector3 direction = (transform.position - hit.point).normalized;

            if (Physics.Raycast(transform.position, -normal, out RaycastHit hit2, MAX_FLOOR_DISTANCE, trackLayer))
            {
                Quaternion turn = Quaternion.AngleAxis(Rudder * RUDDER_MULTIPLIER, transform.up);
                Quaternion align = Quaternion.FromToRotation(transform.up, normal);

                rigidbody.AddForce(-gravity * direction, ForceMode.Acceleration);
                rigidbody.AddForce(normal * hoverPidController.GetOutput((hoverHeight - hit2.distance), Time.fixedDeltaTime));

                Quaternion rot = Quaternion.Slerp(rigidbody.rotation, align * rigidbody.rotation, ROTATION_MULTIPLIER);
                rot = Quaternion.Slerp(rot, turn * rot, ROTATION_MULTIPLIER);

                Quaternion quat = rot * Quaternion.Inverse(rigidbody.rotation);
                rigidbody.AddTorque(quat.x * 100, quat.y * 100, quat.z * 100, ForceMode.Acceleration);
            }
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
