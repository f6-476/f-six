using UnityEngine;

public class GreenMissile : Missile
{
    public override void FixedUpdate()
    {}

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent(out Ship ship))
        {
            ship.Movement.SetThrust();
        }

        Destroy(gameObject);
    }
}
