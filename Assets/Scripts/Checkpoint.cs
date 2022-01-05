using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // if this checkpoint is the "next" checkpoint, increment lastCheckpoint on the ship
        // If this checkpoint is valid and also the last checkpoint the the lap, increment currentLap on the ship
    }
}
