using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Missle : MonoBehaviour
{
    [SerializeField] protected float missleSpeed = 10;
    [SerializeField] protected float _gravity = 9.8f;
    [SerializeField][Range(-100f, 100f)] protected float p, i, d;
    protected PID _hoverPidController;
    protected Rigidbody missleRigidBody;
    protected Transform spawnActualMisslePos;
    //some audio source
    //some particle system to show explosions

    public float hoverHeight = 4;
    public bool tunePID = false;   

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