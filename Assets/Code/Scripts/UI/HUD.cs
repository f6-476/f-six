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
    [SerializeField] private Image shieldBar;
    [SerializeField] private Ship ship;

    private void Start()
    {
        quitPopup.Hide();
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        quitPopup.Show();
    }

    private void Update()
    {
        SetRankingsText();
        
        if (ship != null)
        {
            SetRankText(ship.Race.Rank);
            SetLapText(ship.Race.Lap);
            SetLapTimeText(ship.Race.GetLapDifference());
            SetStopwatchText(Time.time - ship.Race.LapTime);
            SetSpeedText((int)ship.Rigidbody.velocity.magnitude);
        }

        float shieldBar = 0.0f;
        if (ship != null && ship.PowerUp.PowerUp is ShieldPowerUp)
        {
            ShieldPowerUp shieldPowerUp = (ShieldPowerUp)ship.PowerUp.PowerUp;

            if (shieldPowerUp.Active)
            {
                shieldBar = (float)shieldPowerUp.HitCount / (float)ShieldPowerUp.MAX_HIT_COUNT;
            }
        }
        SetShieldBar(shieldBar);
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
        lapText.text = $"{lap}/{RaceManager.Singleton.Laps} LAPS";
    }
    
    private void SetStopwatchText(float lapTime)
    {
        stopWatchText.text = SetFloatToTimer(lapTime);
    }

    private void SetLapTimeText(float lapTimeDifference)
    {
        timeLapText.text = $"{SetFloatToTimer(lapTimeDifference)}";
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

        if (RaceManager.Singleton != null && RaceManager.Singleton.Ships != null)
        {
            foreach (var player in RaceManager.Singleton.Ships)
            {
                playerRankingText += $"{player.Race.Rank}. TODO\n";
            }
        }

        rankingsText.text = playerRankingText;
    }

    private string SetFloatToTimer(float timeFloat)
    {
        var minutes = Mathf.Abs(Mathf.FloorToInt(timeFloat / 60));
        var seconds = Mathf.Abs(Mathf.FloorToInt(timeFloat % 60)).ToString("D2");
        //(n-(int)n)*1000;
        var milliseconds = Mathf.Abs((int)((timeFloat - (int) timeFloat) * 1000f)).ToString("D3");
        return $"{minutes}:{seconds}:{milliseconds}";
    }

    private void SetShieldBar(float amount)
    {
        shieldBar.fillAmount = amount;
    }

    private void SetSpeedText(int speed)
    {
        speedText.text = $"{speed}";
    }
}
