using UnityEngine;
using Unity.Netcode;

public abstract class AbstractManager<T>: NetworkBehaviour where T: AbstractManager<T> 
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
}
