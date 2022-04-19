using UnityEngine;

[CreateAssetMenu(fileName = "Equipable PowerUp", menuName = "FSix/PowerUp/Equipable")]
public class EquipablePowerUp : PowerUpConfig
{
    protected override GameObject MakePrefab(Ship ship)
    {
        GameObject gameObject = Instantiate(prefab, ship.transform.position, ship.transform.rotation);
        return gameObject;
    }

    protected override void PostNetworkSpawn(GameObject gameObject, Ship ship)
    {
        gameObject.transform.parent = ship.transform;
    }
}
