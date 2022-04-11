using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using Unity.Collections;

public class LobbyPlayer : NetworkBehaviour
{
    private bool ready = false;
    public bool Ready
    {
        get => ready;
        set 
        {
            if (ready != value)
            {
                ready = value;
                UpdatePlayerServerRpc(Get());
            }
        }
    }

    public struct Raw : System.IEquatable<Raw>
    {
        public ulong id;
        public FixedString64Bytes name;
        public bool ready;

        public bool Equals(Raw raw)
        {
            return this.id == raw.id && this.name.Equals(raw.name) && this.ready == raw.ready;
        }
    }

    public Raw Get()
    {
        return new Raw
        {
            id = NetworkManager.Singleton.LocalClientId,
            name = AuthManager.Singleton.Username,
            ready = Ready
        };
    }

    private void Start()
    {
        if (IsOwner)
        {
            LobbyManager.Singleton.localPlayer = this;
            AddPlayerServerRpc(Get());
        }
    }

    [ServerRpc]
    public void AddPlayerServerRpc(Raw raw)
    {
        LobbyManager.Singleton.players.Add(raw);
    }

    [ServerRpc]
    public void UpdatePlayerServerRpc(Raw raw)
    {
        var players = LobbyManager.Singleton.players;
        for(int i = 0; i < players.Count; i++)
        {
            if (players[i].id.Equals(raw.id))
            {
                players[i] = raw;
                break;
            }
        }
    }
}
