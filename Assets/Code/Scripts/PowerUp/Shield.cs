using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class Shield : NetworkBehaviour, IEquipable
{
    private Vector3 originalScale;
    public static readonly float SHIELD_DURATION = 5.0f;
    public static readonly int MAX_HIT_COUNT = 3;
    private int hitCount = MAX_HIT_COUNT;
    public int HitCount => hitCount;

    private void Start()
    {
        this.originalScale = transform.localScale;

        Transform target = transform.parent.Find("EquipSpawn");
        transform.localPosition = target.localPosition;

        if (IsServer) StartCoroutine(DelayedDestroy());
    }

    private void Update()
    {
        transform.localScale = originalScale * ((float)hitCount / (float)MAX_HIT_COUNT);
    }

    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(SHIELD_DURATION);
        DestroyMe();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        if (hitCount <= 0) return;

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
            hitCount--;
        }
        else if (otherLayer == 31)
        {
            //insta take down ship shields!
            hitCount = 0;
        }

        if (hitCount <= 0) DestroyMe();
    }

    private void DestroyMe()
    {
        if (TryGetComponent(out NetworkObject networkObject))
        {
            networkObject.Despawn();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
