using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;

public class LobbyMenu : UIMenu
{
    [SerializeField] private Text lobbyNames;

    private void Update()
    {
        string playerNames = "";
        foreach (LobbyPlayer.Raw player in LobbyManager.Singleton.players)
        {
            string readyIcon = player.ready ? "+" : "X";
            playerNames += $"{readyIcon} - {player.name}\n";
        }
        lobbyNames.text = playerNames.TrimEnd();
    }

    public void ToggleReady()
    {
        LobbyPlayer player = LobbyManager.Singleton.localPlayer;
        player.Ready = !player.Ready;
    }
}
