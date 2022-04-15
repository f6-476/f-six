using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissleController : MonoBehaviour
{
    [SerializeField] private GameObject missle;
    private Transform ship;
    private Vector3 missleSpawnPos;

    // Start is called before the first frame update
    void Start()
    {
        ship = transform.parent;
        missleSpawnPos = ship.Find("MissleSpawnPos").position;
    }

    // Update is called once per frame
    void Update()
    {
        //if user presses space
        /*
         * instantiate a missle on missle spawn pos
         */
    }
}
