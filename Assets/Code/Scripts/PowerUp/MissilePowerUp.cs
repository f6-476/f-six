using UnityEngine;

public class MissilePowerUp : AbstractPowerUp
{
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private int count = 3;

    public override void OnActivate()
    {
        if (count <= 0) {
            return;
        }

        Transform spawn = ship.transform.Find("MissileSpawnPos");
        Instantiate(prefab, spawn.position, spawn.rotation);

        if (--count <= 0) {
            Exit();
        }
    }
}
