using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipAI : MonoBehaviour
{
    public float maxSpeed;
    public float maxDegreesDelta;

    private List<OrientedPoint> pathToFollow;
    private Vector3 Velocity;
    private OrientedPoint TargetPoint;
    private int currentFollowingIndex;
    private float arrivalRadius;
    private float ConstMaxSpeed;
    private bool isChasingPowerup = false;


    public LayerMask trackLayer;
    public float hoverHeight = 4;

    public bool tunePID = false;

    [SerializeField] private float _gravity = 9.8f;
    [SerializeField]
    [Range(-100f, 100f)]
    private float p, i, d;
    private PID _hoverPidController;
    private Rigidbody rb;
    void Start()
    {
        pathToFollow = new List<OrientedPoint>();
        arrivalRadius = maxSpeed / 2;
        ConstMaxSpeed = maxSpeed;

        _hoverPidController = new PID(p, i, d);
        rb = GetComponent<Rigidbody>();
    }

    public void TrackTarget(OrientedPoint targetTransform)
    {
        TargetPoint = targetTransform;
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
        Debug.DrawRay(transform.position, -transform.up, Color.red);
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 5, trackLayer))
        {
            //Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 5)

            var normal = BarycentricCoordinateInterpolator.GetInterpolatedNormal(hit);
            var direction = (transform.position - hit.point).normalized;

            Debug.DrawLine(transform.position, hit.point, Color.green);

            rb.AddForce(-_gravity * direction, ForceMode.Acceleration);
            rb.AddForce(normal * _hoverPidController.GetOutput((hoverHeight - hit.distance), Time.fixedDeltaTime));
            //_ship.Rigidbody.rotation = Quaternion.Slerp(Quaternion.FromToRotation(transform.up, normal) * Quaternion.AngleAxis(_ship.RudderValue * 2f, transform.up) * _ship.Rigidbody.rotation);
            float rudder = 0;
            Vector3 targetDir = TargetPoint.position - transform.position;
            //The dot product between the vectors
            float dotProduct = Vector3.Dot(transform.forward, targetDir);

            //Now we can decide if we should turn left or right
            if (dotProduct > 0f)
            {
                rudder = 1;
            }
            else
            {
                rudder = -1;
            }
            var turn = Quaternion.AngleAxis(rudder * 3, transform.up);
            var align = Quaternion.FromToRotation(transform.up, SampleCornersAverage());

            //Debug.Log("Normal: " + normal + " || Magnitude: " + normal.magnitude);
            /*_ship.Rigidbody.MoveRotation(align * _ship.Rigidbody.rotation);
            _ship.Rigidbody.MoveRotation(turn * _ship.Rigidbody.rotation);*/
            //rb.rotation = Quaternion.Slerp(rb.rotation, align * rb.rotation, .3f);
            //rb.rotation = Quaternion.Slerp(rb.rotation, turn * rb.rotation, .5f);
            rb.rotation = Quaternion.RotateTowards(transform.rotation, AlignWithVelocity(), maxDegreesDelta * Time.fixedDeltaTime);
            Vector3 separation = MoveApart();
            
            rb.AddForce((transform.forward + (1.5f * separation)) * (maxSpeed), ForceMode.Acceleration);
            
            //rb.AddForce(separation * maxSpeed * 1.1f, ForceMode.Force);

        }

    }

    void Update()
    {
        maxSpeed = Mathf.Min(ConstMaxSpeed * 1.5f, Mathf.Max(ConstMaxSpeed / 1.5f, maxSpeed + Random.Range(ConstMaxSpeed / -10, ConstMaxSpeed / 10)));
        if (pathToFollow.Count > 0)
        {
            if (!isChasingPowerup && !pathToFollow.Contains(TargetPoint))
            {
                int index = FindNearestWaypointIndex();
                currentFollowingIndex = index;
                TrackTarget(pathToFollow[currentFollowingIndex]);
            }

            // enable this for powerup finding
            // FindPowerupNearby();
            if (!isChasingPowerup)
            {
                if (currentFollowingIndex < pathToFollow.Count - 1 && Vector3.Distance(this.transform.position, pathToFollow[currentFollowingIndex].position) < arrivalRadius)
                {
                    currentFollowingIndex += 1;
                    TrackTarget(pathToFollow[currentFollowingIndex]);
                }
                else if (currentFollowingIndex == pathToFollow.Count - 1)
                {
                    currentFollowingIndex = 0;
                    TrackTarget(pathToFollow[0]);
                }
            }
        }
        else
        {
            initializePath();
        }

    }

    public void FindPowerupNearby(float range = 50.0f)
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position + (transform.forward * range), range);
        foreach (var hitCollider in hitColliders)
        {
            GameObject agent = hitCollider.gameObject;
            if (agent && (agent.layer == LayerMask.NameToLayer("PowerUp")))
            {
                TrackTarget(new OrientedPoint(agent.transform.position, agent.transform.rotation));
                isChasingPowerup = true;
                return;
            }
        }
        isChasingPowerup = false;
    }

    public Quaternion AlignWithVelocity()
    {
        Vector3 direction = TargetPoint.position - transform.position;

        if (direction.normalized == transform.forward || Mathf.Approximately(direction.magnitude, 0))
        {
            return transform.rotation;
        }

        return Quaternion.LookRotation(direction);

        // another implementation, align with the oriented points

        //if (this.Velocity == Vector3.zero)
        //{
        //    return this.transform.rotation;
        //}
        //else
        //{
        //    Quaternion targetRotation = new Quaternion(this.TargetPoint.rotation.x, this.TargetPoint.rotation.y, 0.0f, this.TargetPoint.rotation.w);
        //    return Quaternion.FromToRotation(this.transform.forward, targetRotation * Vector3.forward);
        //}
    }

    public Vector3 MoveApart(float range = 10.0f)
    {
        Vector3 SeparationVelocity = Vector3.zero;
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, range * 10);
        foreach (var hitCollider in hitColliders)
        {
            GameObject agent = hitCollider.gameObject;
            if (agent && (agent.CompareTag("Player") || agent.CompareTag("AI") || agent.CompareTag("Obstacle")))
            {
                if (agent == this.gameObject) continue;

                Vector3 toOther = this.transform.position - agent.transform.position;
                float distance = toOther.magnitude - agent.transform.localScale.z;

                if (distance <= range)
                {
                    SeparationVelocity += toOther.normalized * ((range - distance) / range);
                }
            }
        }

        return SeparationVelocity; //only want the z for avoidance
    }

    public int FindNearestWaypointIndex()
    {
        int index = -1;
        float minDistance = float.MaxValue;
        for (int i = 0; i < pathToFollow.Count; i++)
        {
            if (Vector3.Distance(transform.position, pathToFollow[i].position) < minDistance)
            {
                index = i;
                minDistance = Vector3.Distance(transform.position, pathToFollow[i].position);
            }
        }
        if (index == pathToFollow.Count - 1)
        {
            return 0;
        }
        else
        {
            return index + 1;
        }
    }
    private void initializePath()
    {
        pathToFollow = this.GetComponent<ShipAIPathListGenerator>().waypoints;
        currentFollowingIndex = FindNearestWaypointIndex();
        TrackTarget(pathToFollow[currentFollowingIndex]);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PowerUp"))
        {
            GameObject.Destroy(other.gameObject);
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
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit1, 10, trackLayer))
            hits.Add(hit1);

        //front left
        direction = -transform.up + transform.forward * 1 - transform.right * 1;
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit2, 10, trackLayer))
            hits.Add(hit1);

        //back right
        direction = -transform.up - transform.forward * 1 + transform.right * 1;
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit3, 10, trackLayer))
            hits.Add(hit3);

        //back left
        direction = -transform.up - transform.forward * 1 - transform.right * 1;
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit4, 10, trackLayer))
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