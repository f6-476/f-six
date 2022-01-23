using UnityEngine;

[RequireComponent(typeof(Ship))]
public class ShipAIController : Controller
{
    private void Update()
    {
        thrustValue = 1f;
    }
}
