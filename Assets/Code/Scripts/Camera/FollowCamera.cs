using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField]
    private Transform _target;
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
        transform.position += ((_target.position - (_target.forward * depthoffset) + (_target.up * heightoffset)) - transform.position) * 0.05f;
        transform.rotation = Quaternion.Lerp(transform.rotation,_target.rotation, 0.05f);
    }
}
