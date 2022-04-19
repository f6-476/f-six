using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;

[RequireComponent(typeof(ShipAIController))]
public class ShipAgent : Agent
{
   private ShipAIController controller;

   public void Awake()
   {
      controller = GetComponent<ShipAIController>();
   }

   public override void OnActionReceived(float[] vectorAction)
   {
      base.OnActionReceived(vectorAction);
      
      if (vectorAction[0] < 0) controller.SetThrust(vectorAction[0]);
      else controller.SetReverse(Mathf.Abs(vectorAction[0]));
      
      controller.SetRudder(vectorAction[1]);
   }

   public override void CollectObservations(VectorSensor sensor)
   {
      
   }
}
