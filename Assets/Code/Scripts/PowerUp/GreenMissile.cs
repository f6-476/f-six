using UnityEngine;

public class GreenMissile : Missile
{
    protected override void Start()
    {
        base.Start();

        if (IsServer)
        {
            missileRigidbody.velocity = transform.forward * missileSpeed;
        }
    }
}
