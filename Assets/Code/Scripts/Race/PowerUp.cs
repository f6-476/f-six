using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private GameObject[] powerUpPrefabs;

    private void OnTriggerEnter(Collider other)
    {
        var shipPowerUp = other.GetComponent<ShipPowerUp>();

        if (shipPowerUp == null) return;

        GameObject randomPowerUp = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
        shipPowerUp.AttachPowerUp(randomPowerUp);

        /// TODO: Cooldown respawn instead of destroy?
        Destroy(this.gameObject);
    }
}
