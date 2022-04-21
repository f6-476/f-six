using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Collections;

public class HUD : MonoBehaviour
{
    [SerializeField] private GameObject parent;
    [SerializeField] private QuitPopup quitPopup;
    [SerializeField] private Text rankText;
    [SerializeField] private Text lapText;
    [SerializeField] private Text stopWatchText;
    [SerializeField] private Text timeLapText;
    [SerializeField] private Text rankingsText;
    [SerializeField] private Text speedText;
    [SerializeField] private Image powerUpBar;
    [SerializeField] private Image powerUpImage;
    [SerializeField] private Text usernameText;
    private Ship ship;
    private static readonly float POWER_UP_BAR_SPEED = 8.0f;
    private static readonly float POWER_UP_BAR_MIN_DELTA = 0.005f;

    private void Awake()
    {
        Ship.OnLocal += AttachLocalShip;
        Spectator.OnLocal += AttachLocalSpectator;
    }

    private void OnDestroy()
    {
        Ship.OnLocal -= AttachLocalShip;
        Spectator.OnLocal -= AttachLocalSpectator;
    }

    private void Start()
    {
        quitPopup.Hide();
        powerUpBar.fillAmount = 0;
        powerUpImage.gameObject.SetActive(false);
    }

    private void AttachLocalShip(Ship ship)
    {
        if (this.ship != null)
        {
            this.ship.Multiplayer.username.OnValueChanged -= UpdateUsername;
        }

        this.ship = ship;
        this.ship.Multiplayer.username.OnValueChanged += UpdateUsername;
        UpdateUsername("", this.ship.Multiplayer.username.Value);
    }

    private void UpdateUsername(FixedString64Bytes previous, FixedString64Bytes next)
    {
        switch (LobbyManager.Singleton.LocalPlayer.ClientMode)
        {
            case ClientMode.PLAYER:
            case ClientMode.AI:
            case ClientMode.SPECTATOR:
                usernameText.text = ship.Multiplayer.Username;
                break;
        }
    }

    private void AttachLocalSpectator(Spectator spectator)
    {
        spectator.OnSelect += AttachLocalShip;
        spectator.OnDeselect += () => this.ship = null;
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        quitPopup.Show();
    }

    private void UpdateShipUI()
    {
        if (ship == null) return;

        SetRankText(ship.Race.Rank.Value);
        SetLapText(ship.Race.LapCount.Value + 1);
        SetLapTimeText(ship.Race.GetLapDifference());
        SetSpeedText((int)Mathf.Min(ship.Rigidbody.velocity.magnitude * 10, 999));

        bool active = false;
        float fill = 0.0f;
        PowerUpConfig config = ship.PowerUp.Config;
        if (config != null)
        {
            active = true;
            fill = (float)ship.PowerUp.Count.Value / (float)config.count;
            SetPowerUpIcon(config.icon);
            SetColor(config.color);
        }
        powerUpImage.gameObject.SetActive(active);
        SetPowerUpBar(fill);
    }

    private void Update()
    {
        parent.SetActive(ship != null && (RaceManager.Singleton == null || RaceManager.Singleton.Started));

        SetRankingsText();
        if (RaceManager.Singleton != null) SetStopwatchText(RaceManager.Singleton.GameTime);
        UpdateShipUI();
    }

    private void SetRankText(int rank)
    {
        string suffix;
        switch (rank)
        {
            case 1:
                suffix = "st";
                break;
            case 2:
                suffix = "nd";
                break;
            case 3:
                suffix = "rd";
                break;
            default:
                suffix = "th";
                break;
        }

        rankText.text = $"{rank}{suffix}";
    }

    private void SetLapText(int lap)
    {
        int laps = 3;
        if (RaceManager.Singleton != null) laps = RaceManager.Singleton.Laps;
        
        lap = Mathf.Min(lap, laps);
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
                switch (player.ClientMode)
                {
                    case ClientMode.AI:
                    case ClientMode.PLAYER:
                        playerRankingLines[player.Rank - 1] = $"{player.Rank}. {player.Username}";
                        break;
                    case ClientMode.SPECTATOR:
                        break;
                }
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
        if (Mathf.Abs(powerUpBar.fillAmount - amount) < POWER_UP_BAR_MIN_DELTA)
        {
            powerUpBar.fillAmount = amount;
        }
        else
        {
            powerUpBar.fillAmount = Mathf.Lerp(powerUpBar.fillAmount, amount, POWER_UP_BAR_SPEED * Time.deltaTime);
        }
    }

    private void SetSpeedText(int speed)
    {
        speedText.text = $"{speed}";
    }
}
