using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType { SHIELD, BOOST, RED_MISSILE, GREEN_MISSILE };

public class PowerUp : MonoBehaviour
{
    public PowerUpType type;

    [SerializeField] private GameObject shields;
    [SerializeField] private GameObject greenMissiles;//reg
    [SerializeField] private GameObject redMissiles;//homing

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            switch (this.type)
            {
                case PowerUpType.SHIELD:
                    var shieldSpawnPosGameObj = other.transform.Find("ShieldSpawnPosition");
                    Vector3 spawnPosition;
                    spawnPosition = shieldSpawnPosGameObj.transform.position;
                    Instantiate(shields, spawnPosition, other.transform.rotation, other.transform);
                    break;
                case PowerUpType.BOOST:
                    //do something with boost
                    break;
                case PowerUpType.RED_MISSILE:
                    Instantiate(this.redMissiles, other.transform);
                    break;
                case PowerUpType.GREEN_MISSILE:
                    Instantiate(this.greenMissiles, other.transform);
                    break;
                default:

                    break;
            }

        }
    }


}