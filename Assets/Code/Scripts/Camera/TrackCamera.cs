using System.Collections.Generic;
using UnityEngine;

public class TrackCamera : MonoBehaviour 
{
    private HashSet<Transform> targets = new HashSet<Transform>();
    private float heightOffset = 2.0f;
    private Vector2 rotationOffset = new Vector2(0.0f, 25.0f);
    private float distance = 6.0f;
    private float translationSpeed = 10.0f;
    private float rotationSpeed = 5.0f;

    private Camera camera;

    private void Awake()
    {
        Ship.OnLocal += AttachLocalShip;
        Spectator.OnLocal += AttachLocalSpectator;
        camera = GetComponent<Camera>();
    }

    private void OnDestroy()
    {
        Ship.OnLocal -= AttachLocalShip;
        Spectator.OnLocal -= AttachLocalSpectator;
    }

    private void AttachLocalShip(Ship ship)
    {
        targets.Add(ship.transform);
    }

    private void AttachLocalSpectator(Spectator spectator)
    {
        targets.Add(spectator.transform);
    }

    public void AddTarget(Transform target) 
    {
        this.targets.Add(target);
    }

    public void RemoveTarget(Transform target)
    {
        this.targets.Remove(target);
    }

    private Vector3 AveragePosition() 
    {
        Vector3 average = Vector3.zero;

        if(targets.Count == 0) return average;

        foreach(Transform target in targets) 
        {
            if (target != null) average += target.position;
        }

        return average / targets.Count;
    }

    private Quaternion AverageDirection(Vector3 normal)
    {
        Vector3 average = Vector3.zero;

        if(targets.Count == 0) return Quaternion.Euler(average);

        foreach(Transform target in targets) 
        {
            if (target != null) average += target.forward;
        }

        return Quaternion.LookRotation(average, normal);
    }

    private Vector3 AverageNormal()
    {
        Vector3 average = Vector3.zero;

        if(targets.Count == 0) return Vector3.up;

        foreach(Transform target in targets)
        {
            if (target != null) average += target.transform.up;
        }

        return average.normalized;
    }

    private float AverageSpeed()
    {
        float average = 0;

        if (targets.Count == 0) return 0;

        foreach (Transform target in targets)
        {
            if (target != null)
            {
                Ship ship = target.GetComponent<Ship>();
                
                if (ship != null) average += ship.Rigidbody.velocity.magnitude;
            }
        }

        average = average / targets.Count;

        return average;
    }

    private void UpdateTransform()
    {
        if (targets.Count == 0) return;

        Vector3 normal = AverageNormal();
        Vector3 target = AveragePosition();
        Quaternion direction = AverageDirection(normal) * Quaternion.Euler(rotationOffset.y, rotationOffset.x, 0.0f);
        Vector3 offsetDirection = direction * Vector3.back;
        Vector3 offset = offsetDirection * distance;

        transform.position = Vector3.Lerp(transform.position, target + offset, Time.deltaTime * translationSpeed);

        Vector3 lookForward = (normal * heightOffset - offset).normalized;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookForward, normal), Time.deltaTime * rotationSpeed);
    
        float speed = AverageSpeed();
        camera.fieldOfView = 60 + Mathf.Min(speed, 60);
    }

    private void Update() 
    {
        UpdateTransform();
    }
}
