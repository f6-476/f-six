using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ShipAIController))]
public class ShipAgent : Agent
{
   private ShipAIController controller;
   private Ship _ship;
   private Vector3 startPosition;
   private Quaternion startRotation;

   private float heuristicRudderValue = 0;
   private float heuristicReverseValue = 0;
   private float heuristicThrustValue = 0;

   public void Awake()
   {
      controller = GetComponent<ShipAIController>();
      _ship = GetComponent<Ship>();
      startPosition = transform.position;
      startRotation = transform.rotation;
   }

   private void FixedUpdate()
   {
      
   }

   public override void OnEpisodeBegin()
   {
      transform.position = startPosition;
      transform.rotation = startRotation;
      _ship.Rigidbody.velocity = Vector3.zero;
      _ship.Race.ResetCheckpointIndex();
   }

   public override void OnActionReceived(ActionBuffers actions)
   {
<<<<<<< Updated upstream
      controller.SetThrust(actions.DiscreteActions[0]);
      controller.SetReverse(actions.DiscreteActions[1]);
      
      controller.SetRudder(actions.ContinuousActions[0]);
=======
      agentController.SetThrust(actions.DiscreteActions[0]);
      agentController.SetReverse(actions.DiscreteActions[1]);
      agentController.SetRudder(actions.ContinuousActions[0]);
      
      AddReward(-0.02f);
      // not moving bad
      if (_ship.Movement.VelocityPercent < 0.5f) AddReward(-0.1f);
      else
      {
         var checkpoint = _ship.Race.GetNextCheckpoint();
         //facing right direction good
         if (Vector3.Dot(_ship.Rigidbody.velocity.normalized, checkpoint.transform.forward ) > 0.7f)
         {
            if (_ship.Movement.VelocityPercent < 0.7f) AddReward(0.02f); // moving good
            else AddReward(0.01f);
         }
      }
>>>>>>> Stashed changes
   }

   public override void CollectObservations(VectorSensor sensor)
   {
      var checkpoint = _ship.Race.GetNextCheckpoint();
      float directionDot = Vector3.Dot(transform.forward, checkpoint.transform.forward);
      sensor.AddObservation(directionDot);
      sensor.AddObservation((checkpoint.transform.position - transform.position).sqrMagnitude);
   }

   public override void Heuristic(in ActionBuffers actionsOut)
   {
      ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
      ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

      discreteActions[0] = (int)(heuristicThrustValue);
      discreteActions[1] = (int)(heuristicReverseValue);
      continuousActions[0] = heuristicRudderValue;
      
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
         if (elapsedCollisionTime > 5f)
         {
            AddReward(-1f);
            EndEpisode();
         }
      }
   }
   
   public void GetRudder(InputAction.CallbackContext ctx)
   {
      heuristicRudderValue = ctx.ReadValue<float>();
   }

   public void GetThrust(InputAction.CallbackContext ctx)
   {
      heuristicThrustValue = ctx.ReadValue<float>();
   }
    
   public void GetReverse(InputAction.CallbackContext ctx)
   {
      heuristicReverseValue = ctx.ReadValue<float>();
   }
   
   
}
