using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Ship))]
public class ShipView : MonoBehaviour
{
    [SerializeField] private Text _rankText;
    [SerializeField] private Text _lapText;
    [SerializeField] private Text _stopWatchText;
    [SerializeField] private Text _timeLapText;
    [SerializeField] private Text _rankingsText;

    public void SetRankText(int rank)
    {
        if (rank == 1)
        {
            _rankText.text = $"{rank}st";
        }
        else if (rank == 2)
        {
            _rankText.text = $"{rank}nd";
        }
        else if (rank == 3)
        {
            _rankText.text = $"{rank}rd";
        }
        else
        {
            _rankText.text = $"{rank}th";
        }
    }
    
    public void SetLapText(int lap)
    {
        _lapText.text = $"{lap}/{GameManager.Instance.MapLaps} LAPS";
    }
    
    public void SetStopwatchText(float lapTime)
    {
        _stopWatchText.text = SetFloatToTimer(lapTime);
    }

    public void SetLapTimeText(float lapTimeDifference)
    {
        if (lapTimeDifference < 0f)
        {
            _timeLapText.text = $"+{SetFloatToTimer(lapTimeDifference)}";
            _timeLapText.color = Color.green;
        }
        else if (lapTimeDifference > 0f)
        {
            _timeLapText.text = $"-{SetFloatToTimer(lapTimeDifference)}";
            _timeLapText.color = Color.red;
        }
        else
        {
            _timeLapText.text = $"{SetFloatToTimer(lapTimeDifference)}";
            _timeLapText.color = Color.grey;
        }
    }

    public void SetRankingsText()
    {
        var playerList = GameManager.Instance._playersShips;
        var textField = "";
        var rank = 0;
        foreach (var player in GameManager.Instance._playersShips)
        {
            textField += $"{rank + 1}. {player.name}\n";
        }
        //_rankingsText.text = textField;
    }

    private string SetFloatToTimer(float timeFloat)
    {
        var minutes = Mathf.FloorToInt(timeFloat / 60);
        var seconds = Mathf.FloorToInt(timeFloat % 60);
        //(n-(int)n)*1000;
        var milliseconds = (timeFloat - (int) timeFloat) * 1000f;
        return $"{minutes}:{seconds}:{(int) milliseconds}";
    }
}
