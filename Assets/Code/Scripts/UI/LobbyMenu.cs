using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class LobbyMenu : NetworkBehaviour
{
    [SerializeField] private Text lobbyNames;

    public void Back()
    {
        LobbyManager.Singleton.Disconnect();
    }

    private void Update()
    {
        string playerNames = "";

        if (LobbyManager.Singleton != null && LobbyManager.Singleton.Players != null)
        {
            foreach (LobbyPlayer player in LobbyManager.Singleton.Players)
            {
                string readyIcon = player.Ready ? "+" : "X";
                playerNames += $"{readyIcon} - {player.Username}\n";
            }
        }

        lobbyNames.text = playerNames.TrimEnd();
    }

    public void ToggleReady()
    {
        LobbyPlayer player = LobbyManager.Singleton.LocalPlayer;
        player.Ready = !player.Ready;
    }
}
