using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField]
    private Transform _target;
    [SerializeField]
    private float offset;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
        transform.position += ((_target.position - _target.forward * offset) - transform.position) * 0.2f;
        transform.rotation = Quaternion.Slerp(transform.rotation,_target.rotation, 0.2f);
    }
}
