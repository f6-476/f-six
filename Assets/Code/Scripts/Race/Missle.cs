using UnityEngine;

public abstract class Missle : MonoBehaviour
{   
    public LayerMask trackLayer;
    public float hoverHeight = 1;
    public float thrustMultiplier = 1;
    public bool tunePID = false;   

    [SerializeField] protected float missleSpeed = 3f;
    [SerializeField] protected float _gravity = 9.8f;
    [SerializeField][Range(-100f, 100f)] protected float p, i, d;

    protected PID _hoverPidController;
    protected Rigidbody missleRigidBody;
    protected Transform spawnActualMisslePos;
    //some audio source
    //some particle system to show explosions

    // Start is called before the first frame update
    public void Start()
    {
        missleRigidBody = GetComponent<Rigidbody>();
        //finding the actual missle spawn
        _hoverPidController = new PID(p, i, d);
    }

    // Update is called once per frame
    public abstract void FixedUpdate();

    public abstract void Fire();

    public void OnDestroy()
    {
        //play some explosion animation idk
    }
}