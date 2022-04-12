using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Ship))]
public class ShipMultiplayer : NetworkBehaviour 
{
    private Ship ship;
    private NetworkVariable<Vector3> position = new NetworkVariable<Vector3>(Vector3.zero);
    private NetworkVariable<Quaternion> rotation = new NetworkVariable<Quaternion>(Quaternion.identity);

    private void Awake() 
    {
        ship = GetComponent<Ship>();    
    }

    private void Start() 
    {
        if(IsOwner)
        {
            // TODO: Set from manager?
            Camera.main.GetComponent<CameraController>().AddTarget(transform);
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
