using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class LobbyMenu : MonoBehaviour
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
                switch (player.ClientMode)
                {
                    case ClientMode.PLAYER:
                        string readyIcon = player.Ready ? "<color=#45BF55FF>\u2713</color>" : "<color=#FF0054FF>X</color>";
                        playerNames += $"{readyIcon}\t{player.Username}\n";
                        break;
                    case ClientMode.SPECTATOR:
                        playerNames += $"<color=#2D007AFF>~</color>\t{player.Username}\n";
                        break;
                }
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
