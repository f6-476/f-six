using System.Collections;
using UnityEngine;
using Unity.Netcode;

public abstract class PowerUpConfig : ScriptableObject
{
    public Sprite icon;
    public Color color;
    public GameObject prefab;
    public int count = 1;

    protected abstract GameObject MakePrefab(Ship ship);
    protected virtual void PostNetworkSpawn(GameObject gameObject, Ship ship)
    {}

    public GameObject SpawnPrefab(Ship ship)
    {
        GameObject gameObject = MakePrefab(ship);
        gameObject.GetComponent<IPowerUp>().Attach(ship);

        if (gameObject.TryGetComponent(out NetworkObject networkObject))
        {
            networkObject.Spawn(true);
            PostNetworkSpawn(gameObject, ship);
        }

        return gameObject;
    }
}
