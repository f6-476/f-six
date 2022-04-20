using System;
using UnityEngine;
using Unity.Netcode;

[
    RequireComponent(typeof(ShipMovement)),
    RequireComponent(typeof(ShipHover)),
    RequireComponent(typeof(ShipAudio)),
    RequireComponent(typeof(Rigidbody)),
    RequireComponent(typeof(ShipRace)),
    RequireComponent(typeof(ShipPowerUp)),
]
public class Ship : MonoBehaviour
{
    public static System.Action<Ship> OnLocal;

    [SerializeField] private Controller _controller;
    [SerializeField] private ShipMovement _movement;
    [SerializeField] private ShipHover _hover;
    [SerializeField] private ShipAudio _audio;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private ShipRace _race;
    [SerializeField] private ShipPowerUp _powerup;
    [SerializeField] private ShipMultiplayer _multiplayer;

    public ShipMovement Movement => _movement;
    public ShipHover Hover => _hover;
    public ShipAudio Audio => _audio;
    public ShipRace Race => _race;
    public ShipPowerUp PowerUp => _powerup;
    public ShipMultiplayer Multiplayer => _multiplayer;

    //TODO: Ship stats --> scriptable object to set values (speed, maneuverability, etc...)
    //TODO: State machine for animation?

    // Properties
    public Rigidbody Rigidbody => _rigidbody;
    public float RudderValue  => (_controller != null) ? _controller.GetRudderValue() : 0;
    public float ThrustValue => (_controller != null) ? _controller.GetThrustValue() : 0;
    public float ReverseValue => (_controller != null) ? _controller.GetReverseValue() : 0;
    public bool IsMultiplayer => _multiplayer != null && NetworkManager.Singleton != null;
    public bool IsServer => !IsMultiplayer || _multiplayer.IsServer;
    public bool IsOwner => !IsMultiplayer || _multiplayer.IsOwner;
    public bool IsAI => _controller is ShipAIController;

    private void Start()
    {
        if (RaceManager.Singleton != null) RaceManager.Singleton.AddShip(this);
    }

    private void Update()
    {
        this.Movement.enabled = !this.PowerUp.Disabled;
    }
}
