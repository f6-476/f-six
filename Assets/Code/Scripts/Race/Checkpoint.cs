using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] public bool UseRespawn = false;
    [SerializeField] public Vector3 RespawnPosition = Vector3.zero;
    [SerializeField] public Quaternion RespawnRotation = Quaternion.identity;
    public int Index => transform.GetSiblingIndex();
    public Vector3 Position => (UseRespawn) ? RespawnPosition : transform.position;
    public Quaternion Rotation => (UseRespawn) ? RespawnRotation : transform.rotation;

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(Position, 1.0f);
        Gizmos.DrawRay(Position, (Rotation * Vector3.forward) * 3.0f);
    }
}
