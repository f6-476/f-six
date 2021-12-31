using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Ship))]
public class ShipAudio : MonoBehaviour
{
    public AudioSource audio;
    private float basePitch = 0;
    public AnimationCurve pitchCurve;

    public ShipMovement movement;
    
    private void Start()
    {
        movement = GetComponent<ShipMovement>();
        audio = GetComponent<AudioSource>();
        basePitch = audio.pitch;
    }

    // Update is called once per frame
    void Update()
    {
        audio.pitch = basePitch + basePitch * pitchCurve.Evaluate(movement.VelocityPercent);
    }
}
