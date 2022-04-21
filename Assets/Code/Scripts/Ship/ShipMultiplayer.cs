using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Ship))]
public class ShipMultiplayer : NetworkBehaviour 
{
    public System.Action OnRespawn;

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
    private NetworkVariable<int> lapCount = new NetworkVariable<int>(0);
    public int LapCount
    {
        get => lapCount.Value;
        set => lapCount.Value = value;
    }
    public NetworkList<float> LapTimeList;

    private void Awake()
    {
        LapTimeList = new NetworkList<float>();
    }

    private void Start() 
    {
        if (NetworkManager.Singleton == null)
        {
            this.enabled = false;
        }
        else if (IsOwner)
        {
            if (Ship.OnLocal != null && !ship.IsAI) Ship.OnLocal(ship);
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

    public void Respawn()
    {
        if (!IsOwner) return;
        RespawnServerRpc();
    }

    [ServerRpc]
    private void RespawnServerRpc()
    {
        if (!RaceManager.Singleton.Started) return;
        Checkpoint checkpoint = RaceManager.Singleton.Checkpoints[ship.Race.CheckpointIndex];
        Vector3 position = checkpoint.transform.position + Vector3.up * ship.Hover.Height;
        RespawnClientRpc(position, checkpoint.transform.rotation);
    }

    [ClientRpc]
    private void RespawnClientRpc(Vector3 position, Quaternion rotation)
    {
        if (!IsOwner) return;

        transform.position = position;
        transform.rotation = rotation;
        ship.Rigidbody.velocity = Vector3.zero;

        if (OnRespawn != null) OnRespawn();
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
