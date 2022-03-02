using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public PowerUpType type;
}
public enum PowerUpType { SHIELD, BOOST, RED_MISSILE, GREEN_MISSILE };