using UnityEngine;
using Unity.Netcode;

public abstract class AbstractManager<T>: MonoBehaviour where T: AbstractManager<T> 
{
    private static T instance;
    public static T Singleton
    {
        get => instance;
    }
    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = (T)this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public virtual void OnDestroy() 
    {
        if (instance == this) instance = null;
    }

    public bool IsMaster
    {
        get => NetworkManager.Singleton != null && (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer);
    }
}