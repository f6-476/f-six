using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CameraController : MonoBehaviour 
{
    [SerializeField]
    private List<Transform> targets = new List<Transform>();
    private Vector3 transformOffset = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector2 rotationOffset = new Vector2(0.0f, 25.0f);
    private float distance = 10.0f;
    private float translationSpeed = 10.0f;
    private float rotationSpeed = 5.0f;

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnClientConnected(ulong clientId) 
    {
        if(clientId != NetworkManager.Singleton.LocalClientId) return;

        NetworkClient client;
        if(NetworkManager.Singleton.IsServer) 
        {
            client = NetworkManager.Singleton.ConnectedClients[clientId];
        } 
        else 
        {
            client = NetworkManager.Singleton.LocalClient;
        }

        targets.Add(client.PlayerObject.transform);
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

    private Quaternion AverageDirection()
    {
        Vector3 average = Vector3.zero;

        if(targets.Count == 0) return Quaternion.Euler(average);

        foreach(Transform target in targets) 
        {
            average += target.rotation.eulerAngles;
        }

        return Quaternion.Euler(average / targets.Count);
    }

    private void Update() 
    {
        Vector3 target = AveragePosition();
        Quaternion direction = AverageDirection() * Quaternion.Euler(rotationOffset.y, rotationOffset.x, 0.0f);
        Vector3 offsetDirection = direction * Vector3.back;
        Vector3 offset = offsetDirection * distance;

        transform.position = Vector3.Lerp(transform.position, target + offset, Time.deltaTime * translationSpeed);

        Vector3 lookForward = (transformOffset - offset).normalized;
        transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(lookForward), Time.deltaTime * rotationSpeed);
    }
}
