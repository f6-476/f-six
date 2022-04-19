using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using Unity.Collections;

public class LobbyPlayer : NetworkBehaviour
{
    private NetworkVariable<bool> ready = new NetworkVariable<bool>(false);
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
    public string Username
    {
        get => username.Value.ToString();
    }

    private NetworkVariable<int> rank = new NetworkVariable<int>(1);
    public int Rank
    {
        get => rank.Value;
        set
        {
            if (IsServer) rank.Value = value;
        }
    }

    private void Start()
    {
        LobbyManager.Singleton.Players.Add(this);

        if (IsOwner)
        {
            LobbyManager.Singleton.LocalPlayer = this;
            UpdateUsernameServerRpc(AuthManager.Singleton.Username);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (LobbyManager.Singleton != null) LobbyManager.Singleton.Players.Remove(this);

        base.OnNetworkDespawn();
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
