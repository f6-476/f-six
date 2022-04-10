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
    private bool isChasingPowerup;
    void Start()
    {
        pathToFollow = new List<OrientedPoint>();
        arrivalRadius = maxSpeed / 5;
        ConstMaxSpeed = maxSpeed;
    }

    public void TrackTarget(OrientedPoint targetTransform)
    {
        TargetPoint = targetTransform;
    }


    void Update()
        {
            maxSpeed = Mathf.Min(ConstMaxSpeed* 1.5f,Mathf.Max(ConstMaxSpeed / 1.5f, maxSpeed + Random.Range(ConstMaxSpeed / -10, ConstMaxSpeed / 10)));
            if (pathToFollow.Count > 0)
            {
                GetSteeringSum(out Vector3 steeringForce, out Quaternion rotation);

                Vector3 acceleration = (steeringForce);
                Velocity += acceleration * Time.deltaTime * 50;
                if (Velocity.magnitude > maxSpeed)
                {
                    Velocity = Velocity.normalized * maxSpeed;
                }
                if (steeringForce.magnitude == 0)
                {
                    Velocity = Vector3.zero;
                }
            transform.position += Velocity * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, maxDegreesDelta * Time.deltaTime);

            // Ignore, This is for the other implementation of Align

            //rotation = Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y, 0);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation * transform.rotation, maxDegreesDelta * Time.deltaTime);
            FindPowerupNearby();
            
            if (!isChasingPowerup && !pathToFollow.Contains(TargetPoint))
            {
                int index = FindNearestWaypointIndex();
                currentFollowingIndex = index;
                TrackTarget(pathToFollow[currentFollowingIndex]);
            }

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

    private void GetSteeringSum(out Vector3 steeringForceSum, out Quaternion rotation)
        {
            steeringForceSum = Vector3.zero;
            rotation = Quaternion.identity;
            Vector3 separation = MoveApart();
            if (separation.sqrMagnitude > 0.5)
            {
            steeringForceSum = MoveApart();
            }
            else
            {
                steeringForceSum += Arrive() + MoveApart();
            }

             rotation = AlignWithVelocity();
            }

    public Vector3 Arrive()
    {

        float distanceToGoal = (this.pathToFollow[this.pathToFollow.Count - 1].position - this.transform.position).magnitude;

        Vector3 desiredVelocity = this.TargetPoint.position - this.transform.position;
        float distance = desiredVelocity.magnitude;
        desiredVelocity = desiredVelocity.normalized * maxSpeed;
        return desiredVelocity; //- this.Velocity;
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

        return SeparationVelocity - Vector3.up * SeparationVelocity.y - Vector3.right * SeparationVelocity.x; //only want the z for avoidance
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

}