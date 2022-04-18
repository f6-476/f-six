using UnityEngine;

public class MissilePowerUp : AbstractPowerUp
{
    [SerializeField]
    private GameObject prefab;
    [SerializeField]
    private int count = 3;

    public int Count => count;

    public override void OnActivate()
    {
        if (count <= 0) {
            Exit();
            return;
        }

        Transform spawn = ship.transform.Find("MissileSpawnPos");
        Instantiate(prefab, spawn.position, spawn.rotation);
        --count;
    }
}
