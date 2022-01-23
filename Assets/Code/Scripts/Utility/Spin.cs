using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    void Update()
    {
        transform.rotation = Quaternion.Euler(60.0f * Time.deltaTime, 40.0f * Time.deltaTime, 0.0f)  * transform.rotation;
    }
}
