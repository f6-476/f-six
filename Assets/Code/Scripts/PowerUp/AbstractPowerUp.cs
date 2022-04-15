using UnityEngine;

public abstract class AbstractPowerUp : MonoBehaviour
{
    protected Ship ship;
    
    /// Called when the power up is collected from a power up pickup.
    public virtual void OnEnter(Ship ship) 
    {
        this.ship = ship;
    }

    /// Called when a user attempts to use the powerup.
    public abstract void OnActivate();

    /// Called by self when power up is complete.
    protected virtual void Exit()
    {
        OnExit();
    }

    public virtual void OnExit()
    {
        ship.PowerUp.ClearPowerUp();
        Destroy(this.gameObject);
    }

    /// Called by the ship by the OnTriggerEnter.
    public virtual void OnTriggerEnter(Collider other)
    {}
}
