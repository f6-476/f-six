using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Hover))]
public class Missile : MonoBehaviour
{   
    [SerializeField] protected float missileSpeed = 5f;
    [SerializeField] protected Rigidbody missileRigidbody;
    [SerializeField] protected AudioSource explosionSound;

    private static readonly float MISSILE_DURATION = 10.0f;

    public void Start()
    {
        missileRigidbody.velocity = transform.forward * missileSpeed;
        StartCoroutine(DelayedDestroy());
    }

    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(MISSILE_DURATION);
        explosionSound.Play();
        Destroy(this.gameObject, +explosionSound.clip.length);
    }
}