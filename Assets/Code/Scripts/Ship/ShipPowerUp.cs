using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Ship))]
public class ShipPowerUp : MonoBehaviour
{
    private AbstractPowerUp powerUp = null;
    public AbstractPowerUp PowerUp 
    { 
        get => powerUp;
    }
    private Ship ship;

    private void Start()
    {
        ship = GetComponent<Ship>();
    }

    public void AttachPowerUp(GameObject powerUp)
    {
        if (this.powerUp != null) return;
        Debug.Log($"Collected {powerUp.name}.");
        GameObject gameObject = Instantiate(powerUp, Vector3.zero, Quaternion.identity);
        this.powerUp = gameObject.GetComponent<AbstractPowerUp>();
        this.powerUp.OnEnter(ship);
    }

    public void ClearPowerUp()
    {
        this.powerUp = null;
    }

    public void OnActivate()
    {
        if (powerUp == null) return;
        powerUp.OnActivate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (powerUp == null) return;
        powerUp.OnTriggerEnter(other);
    }
}
