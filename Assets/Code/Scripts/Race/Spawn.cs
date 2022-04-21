using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{
    public int Index => transform.GetSiblingIndex();

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 1.0f);
        Gizmos.DrawRay(transform.position, transform.forward * 3.0f);
    }
}
