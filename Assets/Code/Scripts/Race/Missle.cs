using UnityEngine;

public abstract class Missle : MonoBehaviour
{   
    public LayerMask trackLayer;
    public float hoverHeight = 1;
    public float thrustMultiplier = 1;
    public bool tunePID = false;   

    [SerializeField] protected float missleSpeed = 5f;
    [SerializeField] protected float _gravity = 9.8f;
    [SerializeField][Range(-100f, 100f)] protected float p, i, d;

    protected PID _hoverPidController;
    [SerializeField] protected CapsuleCollider _missileCollider;
    [SerializeField] protected Rigidbody _missileRigidbody;
    public Ship _owner { get; set; }
    //some audio source
    public GameObject thrustersParticles;

    private void Awake()
    {
        //Destroy this missle instance in 10 sec
        Invoke(nameof(DestroyMe), 15.0f);
    }
    // Start is called before the first frame update
    public void Start()
    {
        //finding the actual missle spawn
        _hoverPidController = new PID(p, i, d);
    }

    // Update is called once per frame
    public abstract void FixedUpdate();

    public abstract void Fire();

    private void DestroyMe()
    {
        Destroy(gameObject);
    }


    public void OnDestroy()
    {
        //play some explosion animation idk
    }
}