using UnityEngine;

public class Boost : MonoBehaviour, IEquipable
{
    public void Start()
    {
        Destroy(this.gameObject);
    }

    public void Attach(Ship ship)
    {
        ship.PowerUp.BoostShip();
    }
}
