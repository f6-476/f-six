using UnityEngine;
using UnityEngine.UI;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private Text lobbyNames;

    [SerializeField]
    private RankingPopup rankingPopup;

    void Start()
    {
        StartRankingPopup();
    }

    private void StartRankingPopup()
    {
        if (LobbyManager.Singleton == null || LobbyManager.Singleton.Players.Count < LobbyManager.Singleton.MaxPlayers)
        {
            rankingPopup.Hide();
            return;
        }

        string[] playerRanking = new string[LobbyManager.Singleton.Players.Count];
        foreach (LobbyPlayer player in LobbyManager.Singleton.Players)
        {
            switch (player.ClientMode)
            {
                case ClientMode.PLAYER:
                case ClientMode.AI:
                    playerRanking[player.Rank - 1] = player.Username;
                    break;
                case ClientMode.SPECTATOR:
                    break;
                default:
                    Debug.LogError("Unhandled ClientMode.");
                    break;
            }
        }

        rankingPopup.SetPlayers(new List<string>(playerRanking));
        rankingPopup.Show();
    }

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
                string readyIcon = player.Ready ? "<color=#45BF55FF>\u2713</color>" : "<color=#FF0054FF>X</color>";
                playerNames += $"{readyIcon}\t{player.Username}\n";
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
