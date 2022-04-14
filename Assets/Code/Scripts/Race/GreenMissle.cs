﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenMissle : Missle
{ 

    public override void FixedUpdate()
    {
        //var normal = Vector3.zero;
        if (tunePID)
        {
            _hoverPidController.Kp = p;
            _hoverPidController.Ki = i;
            _hoverPidController.Kd = d;
        }

        //var direction = Vector3.zero;
        Debug.DrawRay(transform.position, -transform.up, Color.red);
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 5, trackLayer))
        {
            //Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 5)

            var normal = SampleCornersAverage();
            var direction = (transform.position - hit.point).normalized;

            Physics.Raycast(transform.position, -normal, out RaycastHit hit2, 5, trackLayer);

            var turn = Quaternion.AngleAxis(0 * 10f, transform.up);
            var align = Quaternion.FromToRotation(transform.up, normal);

            Debug.DrawLine(transform.position, hit.point, Color.green);

            missleRigidBody.AddForce(-_gravity * direction, ForceMode.Acceleration);
            missleRigidBody.AddForce(normal * _hoverPidController.GetOutput((hoverHeight - hit2.distance), Time.fixedDeltaTime));

            var rot = Quaternion.Slerp(missleRigidBody.rotation, align * missleRigidBody.rotation, .4f);
            rot = Quaternion.Slerp(rot, turn * rot, .4f);

            var quat = rot * Quaternion.Inverse(missleRigidBody.rotation);

            missleRigidBody.AddTorque(quat.x * 100, quat.y * 100, quat.z * 100, ForceMode.Acceleration);
        }

    }

    public Vector3 SampleCornersAverage()
    {
        Vector3 norm = Vector3.zero;
        if (Time.time < 1) return transform.up;
        List<RaycastHit> hits = new List<RaycastHit>();
        hits.Clear();

        //front right
        var direction = -transform.up + transform.forward * 1 + transform.right * 1;
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit1, 10, trackLayer))
            hits.Add(hit1);

        //front left
        direction = -transform.up + transform.forward * 1 - transform.right * 1;
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit2, 10, trackLayer))
            hits.Add(hit1);

        //back right
        direction = -transform.up - transform.forward * 1 + transform.right * 1;
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit3, 10, trackLayer))
            hits.Add(hit3);

        //back left
        direction = -transform.up - transform.forward * 1 - transform.right * 1;
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit4, 10, trackLayer))
            hits.Add(hit4);


        if (hits.Count > 0)
        {
            foreach (var hit in hits)
            {
                norm += (BarycentricCoordinateInterpolator.GetInterpolatedNormal(hit));
                Debug.DrawRay(transform.position, hit.point - transform.position);
            }
            norm = norm / hits.Count;
        }
        else
            norm = transform.up;

        return norm;

    }

    public override void Fire()
    {
        throw new System.NotImplementedException();
    }

}
