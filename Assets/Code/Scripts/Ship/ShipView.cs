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

    public void SetRankText(int rank)
    {
        _rankText.text = $"RANK: {rank}";
    }
    
    public void SetLapText(int lap)
    {
        _lapText.text = $"LAPS: {lap}";
    }
    
    public void SetStopwatchText(float lapTime)
    {
        _stopWatchText.text = $"LAP TIME: {lapTime}";
    }

    public void SetLapTimeText(float lapTimeDifference)
    {
        if (lapTimeDifference < 0f)
        {
            _timeLapText.text = $"LAP TIME: +{Mathf.Abs(lapTimeDifference)}";
            _timeLapText.color = Color.green;
        }
        else if (lapTimeDifference > 0f)
        {
            _timeLapText.text = $"LAP TIME: -{Mathf.Abs(lapTimeDifference)}";
            _timeLapText.color = Color.red;
        }
        else
        {
            _timeLapText.text = $"LAP TIME: {Mathf.Abs(lapTimeDifference)}";
            _timeLapText.color = Color.grey;
        }
    }

}
