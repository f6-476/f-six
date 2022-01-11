using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Ship))]
public class ShipAudio : MonoBehaviour
{
    private float basePitch = 0;
    public AnimationCurve pitchCurve;
    [SerializeField] private Ship _ship;
    [SerializeField] private AudioSource _audioSource;

    private void Start()
    {
        basePitch = _audioSource.pitch;
    }

    private void Update()
    {
        _audioSource.pitch = basePitch + basePitch * pitchCurve.Evaluate(_ship.Movement.VelocityPercent);
    }
}
