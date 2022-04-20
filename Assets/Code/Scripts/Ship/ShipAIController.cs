using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(Ship))]
public class ShipAIController : Controller
{
    private Waypoint[] path;
    private int pathIndex = 0;
    private float waypointArrivalRadius = 10.0f;
    private float powerUpArrivalRadius = 0.0f;
    private float alignAngle = 12.5f;
    private float powerUpAngle = 15.0f;
    private float slowdownSpeed = 10.0f;
    private PowerUp nearbyPowerUp = null;

    private void Start()
    {
        UpdateManualPath();
        StartCoroutine(RandomIntervalPowerUp());
    }

    public void Update()
    {
        UpdateNearbyPowerUp();
        UpdateMovement();
    }

    private IEnumerator RandomIntervalPowerUp() 
    {
        yield return new WaitForSeconds(Random.Range(3.0f, 7.0f));
        
        if (!ship.PowerUp.IsEmpty) ship.PowerUp.ActivatePowerUp();

        StartCoroutine(RandomIntervalPowerUp());
    }

    private void UpdateMovement() 
    {
        Waypoint waypoint = path[pathIndex];

        if (ship.PowerUp.IsEmpty && nearbyPowerUp != null && nearbyPowerUp.Active && Mathf.Abs(SignedAngleToTarget(nearbyPowerUp.transform.position)) < powerUpAngle)
        {
            OrientedPoint target = new OrientedPoint(nearbyPowerUp.transform.position, nearbyPowerUp.transform.rotation);
            Arrive(target, powerUpArrivalRadius, waypoint.maxThrust);
        }
        else
        {
            OrientedPoint target = new OrientedPoint(waypoint.transform.position, waypoint.transform.rotation);
            if (Arrive(target, waypointArrivalRadius, waypoint.maxThrust))
            {
                pathIndex = (pathIndex + 1) % path.Length;
            }
        }
    }

    private void ResetMovement()
    {
        thrustValue = 0.0f;
        reverseValue = 0.0f;
        rudderValue = 0.0f;
    }

    private float SignedAngleToTarget(Vector3 target)
    {
        Vector3 displacement = target - transform.position;
        // Project displacement onto same plane.
        Vector3 projectedDisplacement = Vector3.ProjectOnPlane(displacement, transform.up);
        return Vector3.SignedAngle(transform.forward, projectedDisplacement, transform.up);
    }

    private bool Arrive(OrientedPoint target, float arrivalRadius, float maxThrust = 1.0f)
    {
        float angle = SignedAngleToTarget(target.position);

        if (Vector3.Distance(transform.position, target.position) < arrivalRadius)
        {
            ResetMovement();
            return true;
        }

        if (Mathf.Abs(angle) > alignAngle)
        {
            if (ship.Rigidbody.velocity.magnitude > slowdownSpeed)
            {
                thrustValue = 0.0f;
                reverseValue = 1.0f;
            }
            else
            {
                thrustValue = (Mathf.Abs(angle) < alignAngle * 2.0f) ? maxThrust / 4.0f : 0;
                reverseValue = 0.0f;
            }
            
            rudderValue = angle > 0 ? 1.0f : -1.0f;

            return false;
        }

        thrustValue = maxThrust;
        reverseValue = 0.0f;
        rudderValue = 0.0f;

        return false;
    }

    private void UpdateManualPath()
    {
        GameObject[] waypointObjects = GameObject.FindGameObjectsWithTag("Waypoint");
        path = new Waypoint[waypointObjects.Length];

        foreach(GameObject waypointObject in waypointObjects)
        {
            Waypoint waypoint = waypointObject.GetComponent<Waypoint>();
            path[waypoint.Index] = waypoint;
        }

        pathIndex = 0;
    }

    private void UpdateNearbyPowerUp(float range = 20.0f)
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, range);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.TryGetComponent(out PowerUp powerUp))
            {
                if (!powerUp.Active) continue;

                nearbyPowerUp = powerUp;
                return;
            }
        }

        nearbyPowerUp = null;
    }

    private void UpdatePathIndex()
    {
        int index = -1;
        float minDistance = float.MaxValue;
        for (int i = 0; i < path.Length; i++)
        {
            if (Vector3.Distance(transform.position, path[i].transform.position) < minDistance)
            {
                index = i;
                minDistance = Vector3.Distance(transform.position, path[i].transform.position);
            }
        }

        if (index >= 0) this.pathIndex = index;
    }

    private Vector3 GetSeperationDirection(float range = 100.0f)
    {
        Vector3 displacement = Vector3.zero;

        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, range);
        foreach (var hitCollider in hitColliders)
        {
            GameObject gameObject = hitCollider.gameObject;
            if (gameObject && (gameObject.CompareTag("Player") || gameObject.CompareTag("AI") || gameObject.CompareTag("Obstacle")))
            {
                if (gameObject == this.gameObject) continue;

                Vector3 toOther = this.transform.position - gameObject.transform.position;
                float distance = toOther.magnitude - gameObject.transform.localScale.z;

                if (distance <= range)
                {
                    displacement += toOther.normalized * ((range - distance) / range);
                }
            }
        }

        return displacement.normalized;
    }
}
