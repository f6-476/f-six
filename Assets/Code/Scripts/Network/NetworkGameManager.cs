using UnityEngine;
using Unity.Netcode;

public class NetworkGameManager : NetworkBehaviour 
{
    [SerializeField] private GameObject shipPrefab;

    private static NetworkGameManager localNetworkGameManager;
    public static NetworkGameManager Local 
    {
        get 
        {
            return localNetworkGameManager;
        }
    }
    private Ship localShip;
    private Ship Ship 
    {
        get
        {
            return localShip;
        }
    }

    private CameraController cameraController;
    private GameObject audioListener;

    private void Start() 
    {
        this.audioListener = GameObject.Find("Audio Listener");
        this.cameraController = Camera.main.GetComponent<CameraController>();

        if(IsOwner)
        {
            localNetworkGameManager = this;
        }

        if(IsServer)
        {
            InstantiateClient();
        }
    }

    private ClientRpcParams GetOwnerParams() {
        return new ClientRpcParams {
            Send = new ClientRpcSendParams {
                TargetClientIds = new ulong[]{OwnerClientId}
            }
        };
    }

    private void InstantiateClient()
    {
        GameObject gameObject = Instantiate(shipPrefab);
        gameObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(OwnerClientId, false);
    }

    public void AttachShip(Ship ship)
    {
        this.localShip = ship;
        audioListener.transform.SetParent(ship.transform);
        cameraController.AddTarget(ship.transform);
    }
}
