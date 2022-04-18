using UnityEngine;

[CreateAssetMenu(fileName = "Throwable PowerUp", menuName = "PowerUp/Throwable")]
public class ThrowablePowerUp : PowerUpConfig
{
    protected override GameObject MakePrefab(Ship ship)
    {
        Transform spawn = ship.transform.Find("ThrowSpawn");
        GameObject gameObject = Instantiate(prefab, spawn.position, spawn.rotation);
        return gameObject;
    }
}
