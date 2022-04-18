using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Unity.Netcode;

public class HUD : MonoBehaviour
{
    [SerializeField] private QuitPopup quitPopup;
    [SerializeField] private Text rankText;
    [SerializeField] private Text lapText;
    [SerializeField] private Text stopWatchText;
    [SerializeField] private Text timeLapText;
    [SerializeField] private Text rankingsText;
    [SerializeField] private Text speedText;
    [SerializeField] private Image powerUpBar;
    [SerializeField] private Image powerUpImage;
    [SerializeField] private Ship ship;

    private void Start()
    {
        quitPopup.Hide();
        powerUpImage.gameObject.SetActive(false);
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        quitPopup.Show();
    }

    private void UpdateShipUI()
    {
        if (ship == null) return;

        SetRankText(ship.Race.Rank);
        SetLapText(ship.Race.Lap);
        SetLapTimeText(ship.Race.GetLapDifference());
        SetStopwatchText(Time.time - ship.Race.LapTime);
        SetSpeedText((int)ship.Rigidbody.velocity.magnitude);

        bool active = false;
        float fill = 0.0f;
        Color color = Color.white;
        PowerUpConfig config = ship.PowerUp.Config;
        if (config != null)
        {
            active = true;
            color = config.color;
            fill = (float)ship.PowerUp.Count / (float)config.count;
            SetPowerUpIcon(config.icon);
        }
        powerUpImage.gameObject.SetActive(active);
        SetPowerUpBar(fill);
        SetColor(color);
    }

    private void Update()
    {
        SetRankingsText();
        UpdateShipUI();
    }

    public void SetShip(Ship ship)
    {
        this.ship = ship;
    }

    private void SetRankText(int rank)
    {
        if (rank == 1)
        {
            rankText.text = $"{rank}st";
        }
        else if (rank == 2)
        {
            rankText.text = $"{rank}nd";
        }
        else if (rank == 3)
        {
            rankText.text = $"{rank}rd";
        }
        else
        {
            rankText.text = $"{rank}th";
        }
    }

    private void SetLapText(int lap)
    {
        int laps = 3;
        if (RaceManager.Singleton != null) laps = RaceManager.Singleton.Laps;  

        lapText.text = $"{lap}/{laps} LAPS";
    }
    
    private void SetStopwatchText(float lapTime)
    {
        stopWatchText.text = FloatToTimerString(lapTime);
    }

    private void SetLapTimeText(float lapTimeDifference)
    {
        timeLapText.text = $"{FloatToTimerString(lapTimeDifference)}";
        if (lapTimeDifference < 0f)
        {
            timeLapText.text = $"-{timeLapText.text}";
            timeLapText.color = Color.green;
        }
        else if (lapTimeDifference > 0f)
        {
            timeLapText.text = $"+{timeLapText.text}";
            timeLapText.color = Color.red;
        }
        else
        {
            timeLapText.text = $"{timeLapText.text}";
            timeLapText.color = Color.grey;
        }
    }

    private void SetRankingsText()
    {
        string playerRankingText = "";

        if (LobbyManager.Singleton != null && LobbyManager.Singleton.Players != null)
        {
            int playerCount = LobbyManager.Singleton.Players.Count;
            string[] playerRankingLines = new string[playerCount];

            foreach(var player in LobbyManager.Singleton.Players)
            {
                playerRankingLines[player.Rank - 1] = $"{player.Rank}. {player.Username}";
            }

            playerRankingText = string.Join("\n", playerRankingLines);
        }

        rankingsText.text = playerRankingText;
    }

    private string FloatToTimerString(float timeFloat)
    {
        timeFloat = Mathf.Abs(timeFloat);
        var minutes = (Mathf.FloorToInt(timeFloat / 60)).ToString("D2");
        var seconds = (Mathf.FloorToInt(timeFloat % 60)).ToString("D2");
        //(n-(int)n)*1000;
        var milliseconds = ((int)((timeFloat - (int) timeFloat) * 1000f)).ToString("D3");
        return $"{minutes}:{seconds}:{milliseconds}";
    }

    private void SetColor(Color color)
    {
        powerUpImage.color = color;
        powerUpBar.color = color;
    }

    private void SetPowerUpIcon(Sprite icon)
    {
        powerUpImage.sprite = icon;
    }

    private void SetPowerUpBar(float amount)
    {
        powerUpBar.fillAmount = amount;
    }

    private void SetSpeedText(int speed)
    {
        speedText.text = $"{speed}";
    }
}
