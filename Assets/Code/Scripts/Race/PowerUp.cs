using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType { SHIELD, BOOST, RED_MISSILE, GREEN_MISSILE };

public class PowerUp : MonoBehaviour
{
    public PowerUpType type;

    [SerializeField] private GameObject shields;
    [SerializeField] private GameObject greenMissilesController;//reg
    [SerializeField] private GameObject greenMissile;
    [SerializeField] private GameObject redMissilesController;//homing

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Vector3 spawnPosition;
            switch (this.type)
            {
                case PowerUpType.SHIELD:
                    var shieldSpawnPosGameObj = other.transform.Find("ShieldSpawnPos");
                    spawnPosition = shieldSpawnPosGameObj.transform.position;
                    var shield = Instantiate(shields, spawnPosition, other.transform.rotation, other.transform);
                    var shipShields = shield.GetComponent<ShipShields>();
                    shipShields.InitializeShield(other.gameObject.GetComponent<Ship>());
                    break;
                case PowerUpType.BOOST:
                    //do something with boost
                    break;
                case PowerUpType.GREEN_MISSILE:
                    var playerShip = other.gameObject.GetComponent<Ship>();
                    if (!playerShip.Info.CurrentMissile)
                    {
                        playerShip.Info.SetMissile(greenMissile);
                    }
                    break;
                case PowerUpType.RED_MISSILE:
                    var redMissleSpawnPosGameObj = other.transform.Find("MissleControllerSpawnPos");
                    spawnPosition = redMissleSpawnPosGameObj.transform.position;
                    Instantiate(this.redMissilesController, spawnPosition, other.transform.rotation, other.transform);
                    break;
                default:

                    break;
            }

        }
    }


}