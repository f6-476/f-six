using Unity.Netcode;
using UnityEngine;
using Unity.Collections;

[RequireComponent(typeof(Ship))]
public class ShipMultiplayer : NetworkBehaviour 
{
    [SerializeField] private Ship ship;
    public new bool IsServer => base.IsServer;
    public LobbyPlayer Lobby { get; set; }
    private NetworkVariable<Vector3> position = new NetworkVariable<Vector3>(Vector3.zero);
    private NetworkVariable<Quaternion> rotation = new NetworkVariable<Quaternion>(Quaternion.identity);
    private NetworkVariable<int> rank = new NetworkVariable<int>(1);
    public int Rank
    {
        get => rank.Value;
        set
        {
            rank.Value = value;
            if (Lobby) Lobby.Rank = value;
        }
    }
    private NetworkVariable<int> powerUpIndex = new NetworkVariable<int>(0);
    public int PowerUpIndex
    {
        get => powerUpIndex.Value;
        set => powerUpIndex.Value = value;
    }
    private NetworkVariable<int> powerUpCount = new NetworkVariable<int>(0);
    public int PowerUpCount
    {
        get => powerUpCount.Value;
        set => powerUpCount.Value = value;
    }
    private NetworkVariable<bool> shipDisabled = new NetworkVariable<bool>(false);
    public bool ShipDisabled
    {
        get => shipDisabled.Value;
        set => shipDisabled.Value = value;
    }

    private void Start() 
    {
        if(IsOwner)
        {
            // TODO: Set from manager?
            Camera.main.GetComponent<TrackCamera>().AddTarget(transform);
            FindObjectOfType<HUD>().SetShip(ship);
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

    private void UpdateTransform()
    {
        if (IsOwner)
        {
            UpdateTransformServerRpc(transform.position, transform.rotation);
        }
        else
        {
            transform.position = position.Value;
            transform.rotation = rotation.Value;
        }
    }
    
    [ServerRpc]
    public void ActivatePowerUpServerRpc()
    {
        if (PowerUpCount <= 0) return;
        ship.PowerUp.Config.SpawnPrefab(ship);
        PowerUpCount--;
    }

    private void Update()
    {
        UpdateTransform();
    }
}
