using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField]
    private Transform _followTransform;
    
    [SerializeField]
    private Transform _lookTransform;

    [SerializeField]
    private float depthoffset;

    [SerializeField] private float heightoffset;
    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        transform.position += ((_followTransform.position - (_followTransform.forward * depthoffset) + (_followTransform.up * heightoffset)) - transform.position) * 0.05f;

        var look = Quaternion.LookRotation(((_lookTransform.forward * 10 + _lookTransform.position) - transform.position).normalized, _lookTransform.up);
        transform.rotation = Quaternion.Slerp(transform.rotation,look,0.1f);
    }
}
