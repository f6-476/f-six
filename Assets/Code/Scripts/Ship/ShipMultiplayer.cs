using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;

[RequireComponent(typeof(Ship))]
public class ShipMultiplayer : NetworkBehaviour
{
    public System.Action OnRespawn;

    [SerializeField] private Ship ship;
    public new bool IsServer => base.IsServer;
    public LobbyPlayer Lobby { get; set; }
    private NetworkVariable<Vector3> position = new NetworkVariable<Vector3>(Vector3.zero);
    private NetworkVariable<Vector3> velocity = new NetworkVariable<Vector3>(Vector3.zero);
    private NetworkVariable<Quaternion> rotation = new NetworkVariable<Quaternion>(Quaternion.identity);
    private NetworkVariable<int> rank = new NetworkVariable<int>(1);
    private NetworkVariable<int> powerUpIndex = new NetworkVariable<int>(0);
    private NetworkVariable<int> powerUpCount = new NetworkVariable<int>(0);
    private NetworkVariable<bool> shipDisabled = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> shipBoost = new NetworkVariable<bool>(false);
    private NetworkVariable<int> lapCount = new NetworkVariable<int>(0);
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

        AttachVariables();
    }

    private void AttachVariable<T>(SyncVariable<T> syncVariable, NetworkVariable<T> networkVariable) where T : unmanaged
    {
        syncVariable.OnValueChanged += (T previous, T next) => { if (IsServer) networkVariable.Value = next; };
        networkVariable.OnValueChanged += (T previous, T next) => syncVariable.Sync(next);
    }

    private void AttachVariables()
    {
        AttachVariable(ship.PowerUp.Index, powerUpIndex);
        AttachVariable(ship.PowerUp.Count, powerUpCount);
        AttachVariable(ship.PowerUp.Disabled, shipDisabled);
        AttachVariable(ship.PowerUp.Boost, shipBoost);
        AttachVariable(ship.Race.Rank, rank);
        ship.Race.Rank.OnValueChanged += (int previous, int next) => { if (IsServer && Lobby != null) Lobby.Rank = next; };
        AttachVariable(ship.Race.LapCount, lapCount);

        position.OnValueChanged += (Vector3 previous, Vector3 next) => { if (!IsOwner) transform.position = next; };
        rotation.OnValueChanged += (Quaternion previous, Quaternion next) => { if (!IsOwner) transform.rotation = next; };
    }

    [ServerRpc]
    private void UpdateTransformServerRpc(Vector3 position, Quaternion rotation, Vector3 velocity)
    {
        this.position.Value = position;
        this.rotation.Value = rotation;
        this.velocity.Value = velocity;
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
        Vector3 position = checkpoint.Position + Vector3.up * ship.Hover.Height;
        RespawnClientRpc(position, checkpoint.Rotation);
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
            UpdateTransformServerRpc(transform.position, transform.rotation, ship.Rigidbody.velocity);
        }
        else
        {
            transform.position += velocity.Value * Time.deltaTime;
        }
    }

    private void Update()
    {
        UpdateTransform();
    }

    [ServerRpc]
    public void ActivatePowerUpServerRpc()
    {
        if (powerUpCount.Value <= 0) return;
        ship.PowerUp.Config.SpawnPrefab(ship);
        powerUpCount.Value--;
    }
}
