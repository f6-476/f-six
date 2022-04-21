using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Ship))]
public class ShipPowerUp : MonoBehaviour
{
    [SerializeField] private Ship ship;
    public SyncVariable<int> Index = new SyncVariable<int>(0);
    public SyncVariable<int> Count = new SyncVariable<int>(0);
    public SyncVariable<bool> Disabled = new SyncVariable<bool>(false);
    public SyncVariable<bool> Boost = new SyncVariable<bool>(false);
    public PowerUpConfig Config => (Count.Value > 0) ? RaceManager.Singleton.PowerUpConfigs[Index.Value] : null;
    public bool IsEmpty => Count.Value == 0;

    private static readonly float DISABLE_DURATION = 3.0f;
    private static readonly float BOOST_DURATION = 2.0f;

    public void PickUpPowerUp()
    {
        if (!ship.IsServer) return;
        if (Count.Value > 0) return;

        int index = Random.Range(0, RaceManager.Singleton.PowerUpConfigs.Length);
        Index.Value = index; 
        var config = RaceManager.Singleton.PowerUpConfigs[index];
        Count.Value = config.count;
    }

    public void ActivatePowerUp()
    {
        if (!ship.IsMultiplayer)
        {
            if (Count.Value <= 0) return;
            PowerUpConfig config = RaceManager.Singleton.PowerUpConfigs[Index.Value];
            config.SpawnPrefab(ship);
            Count.Value--;
        }
        else
        {
            if (!ship.IsOwner) return;
            ship.Multiplayer.ActivatePowerUpServerRpc();
        }
    }

    public void DisableShip()
    {
        if (!ship.IsServer) return;
        StartCoroutine(DisableShipAsync());
    }

    private IEnumerator DisableShipAsync()
    {
        Disabled.Value = true;
        yield return new WaitForSeconds(DISABLE_DURATION);
        Disabled.Value = false;
    }

    public void BoostShip()
    {
        if (!ship.IsServer) return;
        StartCoroutine(BoostShipAsync());   
    }

    private IEnumerator BoostShipAsync()
    {
        Boost.Value = true;
        yield return new WaitForSeconds(BOOST_DURATION);
        Boost.Value = false;
    }
}
