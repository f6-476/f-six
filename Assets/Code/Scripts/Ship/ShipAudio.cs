using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Ship))]
public class ShipAudio : MonoBehaviour
{
    public AudioSource audioSource;
    private float basePitch = 0;
    public AnimationCurve pitchCurve;
    private Ship _ship;

    private void Start()
    {
        _ship = GetComponent<Ship>();
        audioSource = GetComponent<AudioSource>();
        basePitch = audioSource.pitch;
    }

    private void Update()
    {
        audioSource.pitch = basePitch + basePitch * pitchCurve.Evaluate(_ship.Movement.VelocityPercent);
    }
}
