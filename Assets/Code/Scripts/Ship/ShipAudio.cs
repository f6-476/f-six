using UnityEngine;

[RequireComponent(typeof(Ship))]
public class ShipAudio : MonoBehaviour
{
    private float basePitch = 0;
    public AnimationCurve pitchCurve;
    private Ship _ship;
    [SerializeField] private AudioSource _audioSource;

    private void Start()
    {
        _ship = GetComponent<Ship>();
        basePitch = _audioSource.pitch;
    }

    private void Update()
    {
        _audioSource.pitch = basePitch + basePitch * pitchCurve.Evaluate(_ship.Movement.VelocityPercent);
    }
}
