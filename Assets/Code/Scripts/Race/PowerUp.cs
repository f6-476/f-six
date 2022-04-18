using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private new Collider collider;

    private void Awake()
    {
        this.collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ShipPowerUp shipPowerUp))
        {
            shipPowerUp.PickUpPowerUp();

            StartCoroutine(ActiveCooldown());
        }
    }

    private IEnumerator ActiveCooldown()
    {
        SetActive(false);
        yield return new WaitForSeconds(10.0f);
        SetActive(true);
    }

    private void SetActive(bool state)
    {
        this.collider.enabled = state;
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(state);
        }
    }
}
