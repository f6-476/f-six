using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Hover))]
public abstract class Missile : MonoBehaviour
{   
    [SerializeField] protected float missileSpeed = 5f;
    [SerializeField] protected Rigidbody missileRigidbody;
    //some audio source
    private static readonly float MISSILE_DURATION = 15.0f;

    public void Start()
    {
        missileRigidbody.velocity = transform.forward * missileSpeed;
        StartCoroutine(DelayedDestroy());
    }

    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(MISSILE_DURATION);
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    public abstract void FixedUpdate();

    public void OnDestroy()
    {
        //play some explosion animation idk
    }
}