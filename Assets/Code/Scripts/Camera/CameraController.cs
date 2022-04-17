using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour 
{
    [SerializeField]
    private List<Transform> targets = new List<Transform>();
    private float heightOffset = 3.0f;
    private Vector2 rotationOffset = new Vector2(0.0f, 25.0f);
    private float distance = 10.0f;
    private float translationSpeed = 10.0f;
    private float rotationSpeed = 5.0f;

    private void Start()
    {}

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
            average += target.position;
        }

        return average / targets.Count;
    }

    private Quaternion AverageDirection(Vector3 normal)
    {
        Vector3 average = Vector3.zero;

        if(targets.Count == 0) return Quaternion.Euler(average);

        foreach(Transform target in targets) 
        {
            average += target.forward;
        }

        return Quaternion.LookRotation(average, normal);
    }

    private Vector3 AverageNormal()
    {
        Vector3 average = Vector3.zero;

        if(targets.Count == 0) return Vector3.up;

        foreach(Transform target in targets)
        {
            average += target.transform.up;
        }

        return average.normalized;
    }

    private void UpdateTransform()
    {
        Vector3 normal = AverageNormal();
        Vector3 target = AveragePosition();
        Quaternion direction = AverageDirection(normal) * Quaternion.Euler(rotationOffset.y, rotationOffset.x, 0.0f);
        Vector3 offsetDirection = direction * Vector3.back;
        Vector3 offset = offsetDirection * distance;

        transform.position = Vector3.Lerp(transform.position, target + offset, Time.deltaTime * translationSpeed);

        Vector3 lookForward = (normal * heightOffset - offset).normalized;
        transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(lookForward, normal), Time.deltaTime * rotationSpeed);
    }

    private void Update() 
    {
        UpdateTransform();
    }
}
