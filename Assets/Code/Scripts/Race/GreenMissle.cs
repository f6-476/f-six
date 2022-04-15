using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenMissle : Missle
{
    public override void FixedUpdate()
    {
        if (_missileRigidbody.isKinematic) return;

        _missileRigidbody.AddForce(transform.up * (missleSpeed * 10 * thrustMultiplier), ForceMode.Acceleration);
        //var normal = Vector3.zero;
        if (tunePID)
        {
            _hoverPidController.Kp = p;
            _hoverPidController.Ki = i;
            _hoverPidController.Kd = d;
        }

        //var direction = Vector3.zero;
        Debug.DrawRay(transform.position, transform.forward, Color.red);
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 5, trackLayer))
        {
            //Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, 5)

            var normal = SampleCornersAverage();
            var direction = (transform.position - hit.point).normalized;

            Physics.Raycast(transform.position, -normal, out RaycastHit hit2, 5, trackLayer);

            var turn = Quaternion.AngleAxis(0 * 10f, -transform.forward);
            var align = Quaternion.FromToRotation(-transform.forward, normal);

            Debug.DrawLine(transform.position, hit.point, Color.green);

            _missileRigidbody.AddForce(-_gravity * direction, ForceMode.Acceleration);
            _missileRigidbody.AddForce(normal * _hoverPidController.GetOutput((hoverHeight - hit2.distance), Time.fixedDeltaTime));

            var rot = Quaternion.Slerp(_missileRigidbody.rotation, align * _missileRigidbody.rotation, .4f);
            rot = Quaternion.Slerp(rot, turn * rot, .4f);

            var quat = rot * Quaternion.Inverse(_missileRigidbody.rotation);

            _missileRigidbody.AddTorque(quat.x * 100, quat.y * 100, quat.z * 100, ForceMode.Acceleration);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public Vector3 SampleCornersAverage()
    {
        Vector3 norm = Vector3.zero;
        if (Time.time < 1) return transform.forward;
        List<RaycastHit> hits = new List<RaycastHit>();
        hits.Clear();

        //front right
        var direction = transform.forward + transform.up * 1 + transform.right * 1;
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit1, 10, trackLayer))
            hits.Add(hit1);

        //front left
        direction = transform.forward + transform.up * 1 - transform.right * 1;
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit2, 10, trackLayer))
            hits.Add(hit1);

        //back right
        direction = transform.forward - transform.up * 1 + transform.right * 1;
        if (Physics.Raycast(transform.position, direction, out RaycastHit hit3, 10, trackLayer))
            hits.Add(hit3);

        //back left
        direction = transform.forward - transform.up * 1 - transform.right * 1;
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
        transform.parent = null;
        _missileRigidbody.isKinematic = false;
        _missileCollider.enabled = true;
        _owner.Info.CurrentMissile = null;
        thrustersParticles.SetActive(true);
    }
}
