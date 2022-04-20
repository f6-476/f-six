using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public int Index => transform.GetSiblingIndex();
    public float maxThrust = 1.0f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 1.0f);
    }
}
