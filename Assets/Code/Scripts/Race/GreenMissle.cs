using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenMissle : Missle
{ 

    public override void FixedUpdate()
    {
        //if (tunePID)
        //{
        //    _hoverPidController.Kp = p;
        //    _hoverPidController.Ki = i;
        //    _hoverPidController.Kd = d;
        //}


        ////var direction = Vector3.zero;
        //Debug.DrawRay(transform.position, -transform.up, Color.red);
        //if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, Mathf.Infinity))
        //{
        //    var normal = BarycentricCoordinateInterpolator.GetInterpolatedNormal(hit);
        //    var direction = (transform.position - hit.point).normalized;

        //    Debug.DrawLine(transform.position, hit.point, Color.green);

        //    missleRigidBody.AddForce(-_gravity * direction, ForceMode.Acceleration);
        //    missleRigidBody.AddForce(normal * _hoverPidController.GetOutput((hoverHeight - hit.distance), Time.fixedDeltaTime));
        //    missleRigidBody.MoveRotation(Quaternion.FromToRotation(transform.up, normal) * Quaternion.AngleAxis(_ship.RudderValue * 2f, transform.up) * _ship.Rigidbody.rotation);
        //}

    }

    public override void Fire()
    {
        throw new System.NotImplementedException();
    }

}
