using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipShields : MonoBehaviour
{
    private Image shieldHealthBar;
    private GameObject ship;
    private int hitCount = 3;

    private void Awake()
    {
        ship = GameObject.Find("Ship");
        GameObject tempGameObj = GameObject.Find("Top Right");

        //Filling Bar
        Transform tempChild = tempGameObj.transform.GetChild(0);
        //Fill
        tempChild = tempChild.GetChild(0);
        
        //get ref to shield bars
        shieldHealthBar = tempChild.gameObject.GetComponent<Image>();
        shieldHealthBar.fillAmount = 100f;

        //remove any previous shields when player runs over another shield power up, but keep *this* fresh new one
        Component[] previousShields = ship.GetComponentsInChildren<ShipShields>();
        foreach (ShipShields shield in previousShields)
        {
            if (shield == this)
            {
                continue;
            }
            else
            {
                Destroy(shield.gameObject);
            }
        }
        //player ship gets 10 seconds of shields
        Destroy(gameObject, 15);
    }

    private void OnTriggerEnter(Collider other)
    {
        //layer 7 and 8 is a missle (7 reg, 8 homing missle)
        //we could add other stuff the shield can block/destroy, but don't know what else
        //i kind of imagine this like mario kart were the shield of shells block one attack
        // and after each consecutive hit the shells circling the player decreases.To mimic that here
        //i can reduce the radius of the sphere to signify to the player that their shields 
        //are depleting.
        if (other.gameObject.layer == 7)
        {
            Destroy(other.gameObject);
            this.hitCount--;
            
            if(hitCount == 2)
            {
                transform.localScale -= new Vector3(0.55f, 0.1f, 0.2f);
                shieldHealthBar.fillAmount -= 0.33f;
            }
            else if(hitCount == 1)
            {
                transform.localScale -= new Vector3(0.5f, 0.1f, 0.15f);
                shieldHealthBar.fillAmount -= 0.33f;
            }
            if (hitCount == 0)
            {
                Destroy(gameObject);
                shieldHealthBar.fillAmount = 0f;
            }
        }
        else if (other.gameObject.layer == 8)
        {
            //insta take down ship shields!
            shieldHealthBar.fillAmount = 0f;
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

}
