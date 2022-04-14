using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType { SHIELD, BOOST, RED_MISSILE, GREEN_MISSILE };

public class PowerUp : MonoBehaviour
{
    public PowerUpType type;

    [SerializeField] private GameObject shields;
    [SerializeField] private GameObject greenMissilesControllerSpawn;//reg
    [SerializeField] private GameObject redMissilesControllerSpawn;//homing

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
                    var greenMissleSpawnPosGameObj = other.transform.Find("MissleControllerSpawnPos");
                    spawnPosition = greenMissleSpawnPosGameObj.transform.position;
                    var missleRotation = other.transform.rotation;
                    Instantiate(this.greenMissilesControllerSpawn, spawnPosition, missleRotation*Quaternion.Euler(90f,0f,0f), other.transform);
                    break;
                case PowerUpType.RED_MISSILE:
                    var redMissleSpawnPosGameObj = other.transform.Find("MissleControllerSpawnPos");
                    spawnPosition = redMissleSpawnPosGameObj.transform.position;
                    Instantiate(this.redMissilesControllerSpawn, spawnPosition, other.transform.rotation, other.transform);
                    break;
                default:

                    break;
            }

        }
    }


}