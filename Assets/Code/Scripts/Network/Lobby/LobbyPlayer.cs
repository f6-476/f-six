using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using Unity.Collections;

public class LobbyPlayer : NetworkBehaviour
{
    public NetworkVariable<bool> ready = new NetworkVariable<bool>(false);
    public bool Ready
    {
        get => ready.Value;
        set
        {
            if (IsOwner) UpdateReadyServerRpc(value);
            else ready.Value = value;
        }
    }

    private NetworkVariable<FixedString64Bytes> username = new NetworkVariable<FixedString64Bytes>("");
    public string Username => username.Value.ToString();

    private NetworkVariable<int> rank = new NetworkVariable<int>(1);
    public int Rank
    {
        get => rank.Value;
        set
        {
            if (IsServer) rank.Value = value;
        }
    }

    private NetworkVariable<ClientMode> clientMode = new NetworkVariable<ClientMode>(ClientMode.PLAYER);
    public ClientMode ClientMode
    {
        get => clientMode.Value;
        set => clientMode.Value = value;
    }

    private void Start()
    {
        if (!LobbyManager.Singleton.Players.Contains(this))
        {
            LobbyManager.Singleton.Players.Add(this);
        }

        switch (this.ClientMode)
        {
            case ClientMode.PLAYER:
            case ClientMode.SPECTATOR:
                if (IsOwner)
                {
                    LobbyManager.Singleton.LocalPlayer = this;
                    UpdateUsernameServerRpc(AuthManager.Singleton.Username);
                }
                break;
            case ClientMode.AI:
                if (IsServer)
                {
                    username.Value = AuthManager.GenerateUsername();
                }
                break;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (LobbyManager.Singleton != null) LobbyManager.Singleton.Players.Remove(this);

        base.OnNetworkDespawn();
    }

    public void DestroyMe()
    {
        if (!IsServer) return;
        GetComponent<NetworkObject>().Despawn();
    }

    [ServerRpc]
    private void UpdateReadyServerRpc(bool ready)
    {
        this.ready.Value = ready;
        LobbyManager.Singleton.OnPlayerUpdate(this);
    }

    [ServerRpc]
    private void UpdateUsernameServerRpc(FixedString64Bytes username)
    {
        this.username.Value = username;
    }
}
