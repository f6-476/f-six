using UnityEngine;

public class PreviewCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float height;
    [SerializeField] private float distance;
    [SerializeField] private Vector3 lookOffset;
    [SerializeField] private float revolutionDuration = 1.0f;

    private void Update()
    {
        float angle = 2 * Mathf.PI * ((Time.time % revolutionDuration) / revolutionDuration);
        Vector3 direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        transform.position = target.position + direction * distance + Vector3.up * height;
        transform.LookAt(target.position + lookOffset);
    }
}
