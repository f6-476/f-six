using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Ship))]
public class ShipMultiplayer : NetworkBehaviour 
{
    [SerializeField] private Ship ship;
    private CameraController cameraController;

    private NetworkVariable<Vector3> position;
    private NetworkVariable<Quaternion> rotation;

    private void Start() 
    {
        if(IsOwner)
        {
            NetworkGameManager.Local.AttachShip(ship);
        }
        else
        {
            ship.Movement.enabled = false;
            ship.Hover.enabled = false;
        }
    }

    [ServerRpc]
    private void UpdateTransformServerRpc(Vector3 position, Quaternion rotation) {
        this.position.Value = position;
        this.rotation.Value = rotation;
    }

    private void LocalUpdate()
    {
        UpdateTransformServerRpc(transform.position, transform.rotation);
    }

    private void RemoteUpdate()
    {
        transform.position = position.Value;
        transform.rotation = rotation.Value;
    }

    private void Update()
    {
        if(IsOwner) LocalUpdate(); 
        else RemoteUpdate();
    }
}