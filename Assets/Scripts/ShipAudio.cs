using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAudio : MonoBehaviour
{
    public AudioSource audio;
    private float basePitch = 0;
    public AnimationCurve pitchCurve;

    public ShipHover hover;
    
    private void Start()
    {
        hover = GetComponent<ShipHover>();
        audio = GetComponent<AudioSource>();
        basePitch = audio.pitch;
    }

    // Update is called once per frame
    void Update()
    {
        audio.pitch = basePitch + basePitch * pitchCurve.Evaluate(hover.VelocityPercent);
    }
}
