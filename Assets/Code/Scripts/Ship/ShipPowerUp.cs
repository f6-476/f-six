using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Ship))]
public class ShipPowerUp : MonoBehaviour
{
    [SerializeField] private Ship ship;
    [SerializeField] private PowerUpConfig[] configs;
    private int index = 0;
    public int Index 
    { 
        get => (ship.IsMultiplayer) ? ship.Multiplayer.PowerUpIndex : index;
        set
        {
            if (ship.IsMultiplayer) ship.Multiplayer.PowerUpIndex = value;
            else index = value;
        }
    }
    private int count = 0;
    public int Count 
    { 
        get => (ship.IsMultiplayer) ? ship.Multiplayer.PowerUpCount : count;
        set
        {
            if (ship.IsMultiplayer) ship.Multiplayer.PowerUpCount = value;
            else count = value;
        }
    }
    private bool disabled = false;
    public bool Disabled
    { 
        get => (ship.IsMultiplayer) ? ship.Multiplayer.ShipDisabled : disabled;
        set
        {
            if (ship.IsMultiplayer) ship.Multiplayer.ShipDisabled = value;
            else disabled = value;
        }
    }
    public PowerUpConfig Config => (Count > 0) ? configs[Index] : null;
    public bool IsEmpty => Count == 0;

    private static readonly float DISABLE_DURATION = 3.0f;

    public void PickUpPowerUp()
    {
        if (!ship.IsServer) return;
        if (Count > 0) return;

        int index = Random.Range(0, configs.Length);
        Index = index; 
        var config = configs[index];
        Count = config.count;
    }

    public void ActivatePowerUp()
    {
        if (!ship.IsMultiplayer)
        {
            if (Count <= 0) return;
            var config = configs[Index];
            config.SpawnPrefab(ship);
            Count--;
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
        Disabled = true;
        yield return new WaitForSeconds(DISABLE_DURATION);
        Disabled = false;
    }
}
