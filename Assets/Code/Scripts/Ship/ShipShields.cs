using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipShields : MonoBehaviour
{
    private BoxCollider shipCollider;
    private GameObject ship;
    private Image shieldHealthBar;

    private Vector3 originalScale;

    private int hitCount = 3;

    private void Awake()
    {
        ship = GameObject.Find("Ship");
        //disable ship's collider for now while shields are active
        shipCollider = ship.GetComponent<BoxCollider>();
        shipCollider.enabled = !shipCollider.enabled;
        //keeps track of the OG scale of shields prefab. It's a visual aid on shield status
        originalScale = transform.localScale;

        //Fill for UI getting for next three lines
        GameObject tempGameObj = GameObject.Find("Top Right");
        //Filling Bar
        Transform tempChild = tempGameObj.transform.GetChild(0);
        //Fill
        tempChild = tempChild.GetChild(0);
        
        //get ref to shield bars UI
        shieldHealthBar = tempChild.gameObject.GetComponent<Image>();
        shieldHealthBar.fillAmount = 100f;

        //Destroy this shields instance in 10 sec
        Invoke("DestroyMe", 10.0f);
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
            this.hitCount--;
            Destroy(other.gameObject);

            if (hitCount == 2)
            {
                //reduce scale of shields
                transform.localScale -= new Vector3(0.55f, 0.1f, 0.2f);
                shieldHealthBar.fillAmount -= 0.33f;
            }
            else if (hitCount == 1)
            {
                //reduce scale of shields
                transform.localScale -= new Vector3(0.55f, 0.1f, 0.2f);
                shieldHealthBar.fillAmount -= 0.33f;
            }
            if (hitCount == 0)
            {
                shieldHealthBar.fillAmount = 0f;
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
                CancelInvoke("DestroyMe");
                transform.localScale = originalScale;
                shieldHealthBar.fillAmount = 100f;
                this.hitCount = 3;
                Invoke("DestroyMe", 10.0f);
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
        if(this.hitCount > 0)
        {
            shieldHealthBar.fillAmount = 0f;
        }
        //Adjust UI
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        //re-enable ship collider
        shipCollider.enabled = !shipCollider.enabled;
    }
}
