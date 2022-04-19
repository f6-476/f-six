using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PowerUp : NetworkBehaviour
{
    [SerializeField] private GameObject[] visuals;
    [SerializeField] private AudioSource pickUpAudioSource;
    [SerializeField] private ParticleSystem pickUpParticleSystem;
    private bool active = true;

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
        active = false;
        SetActiveClientRpc(false);
        yield return new WaitForSeconds(10.0f);
        active = true;
        SetActiveClientRpc(true);
    }

    [ClientRpc]
    private void SetActiveClientRpc(bool state)
    {
        if (!state) 
        {
            pickUpAudioSource.Play();
            pickUpParticleSystem.Play();
        }

        foreach(GameObject visual in visuals)
        {
            visual.SetActive(state);
        }
    }
}
