using UnityEngine;

[RequireComponent(typeof(Ship))]
public class ShipAudio : MonoBehaviour
{
    [SerializeField] private Ship ship;
    [SerializeField] private AudioSource _audioSource;
    private float basePitch = 0;
    public AnimationCurve pitchCurve;

    private void Start()
    {
        basePitch = _audioSource.pitch;
    }

    private void Update()
    {
        _audioSource.pitch = basePitch + basePitch * pitchCurve.Evaluate(ship.Movement.VelocityPercent);
    }
}
