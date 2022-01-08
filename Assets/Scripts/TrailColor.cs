using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailColor : MonoBehaviour
{
    // Start is called before the first frame update
    public TrailRenderer trail;
    private Rigidbody _rigidbody;
    void Start()
    {
        trail = GetComponent<TrailRenderer>();
        _rigidbody = GetComponentInParent<Rigidbody>();
    }
    
    
    // Update is called once per frame
    void Update()
    {
      trail.startColor = Color.Lerp(Color.red, Color.cyan, _rigidbody.velocity.sqrMagnitude/6400);  
    }
}
