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
    [SerializeField] private Image greenMissle1;
    [SerializeField] private Image greenMissle2;
    [SerializeField] private Image greenMissle3;
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
        else if(ship!=null && ship.PowerUp.PowerUp is MissilePowerUp)
        {
            var missile = (MissilePowerUp) ship.PowerUp.PowerUp;
            int missileCount = missile.Count;
            switch (missileCount)
            {
                case 3:
                    greenMissle3.color = Color.green;
                    greenMissle2.color = Color.green;
                    greenMissle1.color = Color.green;
                    break;
                case 2:
                    greenMissle3.color = Color.white;
                    break;
                case 1:
                    greenMissle2.color = Color.white;
                    break;
                case 0:
                    greenMissle1.color = Color.white;
                    break;
                default:
                    greenMissle1.color = Color.white;
                    break;
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

        if (RaceManager.Singleton != null && RaceManager.Singleton.Ships != null)
        {
            foreach (var player in RaceManager.Singleton.Ships)
            {
                playerRankingText += $"{player.Race.Rank}. TODO\n";
            }
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

    private void SetShieldBar(float amount)
    {
        shieldBar.fillAmount = amount;
    }

    private void SetSpeedText(int speed)
    {
        speedText.text = $"{speed}";
    }
}
