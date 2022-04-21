using UnityEngine;
using Unity.Netcode;

public class LobbySettings : NetworkBehaviour
{
    private NetworkVariable<int> mapIndex = new NetworkVariable<int>(0);

    private void Start()
    {
        mapIndex.OnValueChanged += OnMapIndexUpdate;
    }

    private void OnMapIndexUpdate(int previous, int next)
    {
        LobbyManager.Singleton.MapIndex = next;
    }

    public void PreviousMap()
    {
        if (LobbyManager.Singleton == null) return;
        if (!IsServer) return;

        int nextIndex = LobbyManager.Singleton.PreviousMap();
        mapIndex.Value = nextIndex;
    }

    public void NextMap()
    {
        if (LobbyManager.Singleton == null) return;
        if (!IsServer) return;

        int nextIndex = LobbyManager.Singleton.NextMap();
        mapIndex.Value = nextIndex;
    }
}
