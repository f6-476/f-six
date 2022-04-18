using UnityEngine;

public class GreenMissile : Missile
{

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent(out Ship ship))
        {
            ship.Movement.SetThrust();
        }

        Destroy(gameObject);
    }
}
