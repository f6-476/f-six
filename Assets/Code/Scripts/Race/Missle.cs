using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Missle : MonoBehaviour
{
    [SerializeField] protected float missleSpeed = 10;

    // Start is called before the first frame update
    public abstract void Start();

    // Update is called once per frame
    public abstract void FixedUpdate();

    public abstract void Fire();

    public void OnDestroy()
    {
        //play some explosion animation idk
    }
}