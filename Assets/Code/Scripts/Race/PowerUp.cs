using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PowerUp : NetworkBehaviour
{
    [SerializeField] private GameObject model;
    [SerializeField] private AudioSource pickUpAudioSource;
    [SerializeField] private ParticleSystem pickUpParticleSystem;
    private bool active = true;
    public bool Active => active;

    private void OnTriggerEnter(Collider other)
    {
        if (NetworkManager.Singleton != null && !IsServer) return;
        if (!active) return;

        if (other.TryGetComponent(out ShipPowerUp shipPowerUp))
        {
            shipPowerUp.PickUpPowerUp();

            StartCoroutine(PickUpCooldown());
        }
    }

    private IEnumerator PickUpCooldown()
    {   
        SetActive(false);
        yield return new WaitForSeconds(10.0f);
        SetActive(true);
    }

    private void SetActiveBase(bool state)
    {
        if (!state) 
        {
            pickUpAudioSource.Play();
            pickUpParticleSystem.Play();
        }

        model.SetActive(state);
    }

    private void SetActive(bool state)
    {
        active = state;
        if (NetworkManager.Singleton == null)
        {
            SetActiveBase(state);
        }
        else
        {
            SetActiveClientRpc(state);
        }
    }

    [ClientRpc]
    private void SetActiveClientRpc(bool state)
    {
        SetActiveBase(state);
    }
}
