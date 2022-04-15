using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipShields : MonoBehaviour
{
    private Ship _ship;
    private BoxCollider _shipCollider;

    private Vector3 _originalScale;
    public static readonly int MAX_HIT_COUNT = 3; 

    private int _hitCount = MAX_HIT_COUNT;
    public int HitCount => _hitCount;

    private void Awake()
    {
        //keeps track of the OG scale of shields prefab. It's a visual aid on shield status
        _originalScale = transform.localScale;

        //Destroy this shields instance in 10 sec
        Invoke(nameof(DestroyMe), 10.0f);
    }

    // When shield is instantiated, set its owner to the ship that picked up that powerup
    public void InitializeShield(Ship owner)
    {
        _ship = owner;
        _ship.Shields = this;
        _shipCollider = owner.gameObject.GetComponent<BoxCollider>();
        // disable ship collider while having a shield
        // Not necessary to have it when the shield has one while it's active
        _shipCollider.enabled = !_shipCollider.enabled;
    }

    private void OnTriggerEnter(Collider other)
    {
        /*
         *layer 30 and 31 is a missle (30 green, 31 red homing missle)
          i kind of imagine this like mario kart were the shield of shells block one attack
          and after each consecutive hit the shells circling the player decreases. To mimic that here
          i reduce the scale of the sphere to signify to the player that their shields
          are depleting. 7 is for obstacles, and when the player is shielded he is immune to getting
          derailed(BUMPED) from them.
         */

        int otherLayer = other.gameObject.layer;
        
        if (otherLayer == 7 || otherLayer == 30)
        {
            this._hitCount--;
            Destroy(other.gameObject);

            if (_hitCount == 2)
            {
                //reduce scale of shields
                transform.localScale -= new Vector3(0.55f, 0.1f, 0.2f);
            }
            else if (_hitCount == 1)
            {
                //reduce scale of shields
                transform.localScale -= new Vector3(0.55f, 0.1f, 0.2f);
            }
            if (_hitCount == 0)
            {
                Destroy(gameObject);
            }
        }
        else if(otherLayer == 6)
        {
            //reset shields to square one after shield re-pickup
            GameObject tempPowerUp = other.gameObject;
            PowerUp powerUpScript = tempPowerUp.GetComponent<PowerUp>();

            if (powerUpScript.type == PowerUpType.SHIELD)
            {
                CancelInvoke(nameof(DestroyMe));
                transform.localScale = _originalScale;
                _hitCount = 3;
                Invoke(nameof(DestroyMe), 10.0f);
            }
        }
        else if (otherLayer == 31)
        {
            //insta take down ship shields!
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    private void DestroyMe()
    {
        //Adjust UI
        _ship.Shields = null;
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        //re-enable ship collider
        _shipCollider.enabled = !_shipCollider.enabled;
    }
}
