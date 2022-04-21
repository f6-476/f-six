using System.Collections;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Hover))]
public abstract class Missile : NetworkBehaviour, IThrowable
{
    [SerializeField] protected float missileSpeed = 5f;
    [SerializeField] protected Rigidbody missileRigidbody;
    [SerializeField] protected GameObject explosionPrefab;
    private static readonly float MISSILE_DURATION = 15.0f;
    private NetworkVariable<Vector3> position = new NetworkVariable<Vector3>(Vector3.zero);
    private NetworkVariable<Quaternion> rotation = new NetworkVariable<Quaternion>(Quaternion.identity);

    protected virtual void Start()
    {
        if (IsServer)
        {
            StartCoroutine(DelayedDestroy());
        }
        else
        {
            missileRigidbody.isKinematic = false;
        }
    }

    public void Attach(Ship ship)
    {}

    private void ServerUpdate()
    {
        position.Value = transform.position;
        rotation.Value = transform.rotation;
    }

    private void ClientUpdate()
    {
        transform.position = position.Value;
        transform.rotation = rotation.Value;
    }

    protected virtual void Update()
    {
        if (IsServer) ServerUpdate();
        else ClientUpdate();
    }

    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(MISSILE_DURATION);

        DestroyMe();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!IsServer) return;

        if (other.gameObject.TryGetComponent(out Ship ship)) 
        {
            ship.PowerUp.DisableShip();
        }

        DestroyMe();
    }

    private void DestroyMe()
    {
        if (TryGetComponent(out NetworkObject networkObject))
        {
            if (networkObject.IsSpawned) networkObject.Despawn();
            else Destroy(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public override void OnDestroy()
    {
        Instantiate(explosionPrefab, transform.position, explosionPrefab.transform.rotation);
    }
}
