using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

[RequireComponent(typeof(ShipAIAgentController))]
public class ShipAgent : Agent
{
   private ShipAIAgentController agentController;
   private Ship _ship;
   private Vector3 startPosition;
   private Quaternion startRotation;

   public void Awake()
   {
      agentController = GetComponent<ShipAIAgentController>();
      _ship = GetComponent<Ship>();
      startPosition = transform.position;
      startRotation = transform.rotation;
   }

   private void FixedUpdate()
   {
      AddReward(-0.01f * Time.fixedDeltaTime);
      // not moving bad
      if (_ship.Movement.VelocityPercent < 0.5f) AddReward(-0.1f * Time.fixedDeltaTime);
      else
      {
         //facing right direction good
         if (Vector3.Dot(_ship.Rigidbody.velocity.normalized, transform.forward) > 0.5f)
         {
            if (_ship.Movement.VelocityPercent < 0.7f) AddReward(0.2f * Time.fixedDeltaTime); // moving good
            else AddReward(0.1f * Time.fixedDeltaTime);
         }
      }
   }

   public override void OnEpisodeBegin()
   {
      transform.position = startPosition;
      transform.rotation = startRotation;
      _ship.Race.ResetCheckpointIndex();
   }

   public override void OnActionReceived(ActionBuffers actions)
   {
      agentController.SetThrust(actions.DiscreteActions[0]);
      agentController.SetReverse(actions.DiscreteActions[1]);
      
      agentController.SetRudder(actions.ContinuousActions[0]);
   }

   public override void CollectObservations(VectorSensor sensor)
   {
      var checkpoint = _ship.Race.GetNextCheckpoint();
      float directionDot = Vector3.Dot(transform.forward, checkpoint.transform.forward);
      sensor.AddObservation(directionDot);


   }

   private float elapsedCollisionTime = 0f;
   private void OnCollisionEnter(Collision other)
   {
      if(other.gameObject.CompareTag("Obstacle"))
      {
         AddReward(-1f);
         elapsedCollisionTime = 0f;
      }
   }

   private void OnCollisionStay(Collision other)
   {
      if(other.gameObject.CompareTag("Obstacle"))
      {
         AddReward(-0.2f);
         elapsedCollisionTime += Time.fixedDeltaTime;
         if (elapsedCollisionTime > 3.5f)
         {
            SetReward(-1f);
            EndEpisode();
         }
      }
   }
}
