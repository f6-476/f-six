using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class Shield : NetworkBehaviour, IEquipable
{
    [SerializeField] private GameObject model;
    private Vector3 originalScale;
    private static readonly float SHIELD_DURATION = 5.0f;
    private static readonly float GROW_SPEED = 10.0f;
    public static readonly int MAX_HIT_COUNT = 2;
    private NetworkVariable<int> hitCount = new NetworkVariable<int>(MAX_HIT_COUNT);

    private void Start()
    {
        this.originalScale = model.transform.localScale;
        model.transform.localScale = Vector3.zero;

        Transform target = transform.parent.Find("EquipSpawn");
        transform.localPosition = target.localPosition;

        if (IsServer) StartCoroutine(DelayedDestroy());
    }

    public void Attach(Ship ship)
    {}

    private void Update()
    {
        Vector3 targetScale = originalScale * ((float)hitCount.Value / (float)MAX_HIT_COUNT);
        model.transform.localScale = Vector3.Lerp(model.transform.localScale, targetScale, GROW_SPEED * Time.deltaTime);
    }

    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(SHIELD_DURATION);
        DestroyMe();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        if (hitCount.Value <= 0) return;

        if (other.TryGetComponent<Missile>(out Missile missile))
        {
            if (missile.DestroySafe()) hitCount.Value--;
        }

        if (hitCount.Value <= 0) DestroyMe();
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
