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
    [SerializeField] private Text _lapTimeText;

    public void SetRankText(int rank)
    {
        _rankText.text = $"RANK: {rank}";
    }
    
    public void SetLapText(int lap)
    {
        _lapText.text = $"LAPS: {lap}";
    }
    
    public void SetLapTimeText(float lapTimeText)
    {
        _lapTimeText.text = $"LAP TIME: {lapTimeText}";
    }
}
