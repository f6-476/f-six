using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPowerUp : AbstractPowerUp
{
    [SerializeField]
    private GameObject prefab;
    private GameObject shield;
    private bool _isActive;
    public bool Active => _isActive;
    private Vector3 _originalScale;

    public static readonly float SHIELD_DURATION = 10.0f;
    public static readonly int MAX_HIT_COUNT = 3;
    private int _hitCount = MAX_HIT_COUNT;
    public int HitCount => _hitCount;

    private Vector3 Scale
    {
        get => shield.transform.localScale;
        set => shield.transform.localScale = value;
    }

    public override void OnEnter(Ship ship)
    {
        base.OnEnter(ship);

        Transform spawn = ship.transform.Find("ShieldSpawnPos");
        shield = Instantiate(prefab, spawn.position, spawn.rotation, ship.transform);

        _isActive = false;
        //keeps track of the OG scale of shields prefab. It's a visual aid on shield status
        _originalScale = Scale;
        Scale = Vector3.zero;
    }

    public override void OnExit()
    {
        Destroy(shield);

        base.OnExit();
    }

    public override void OnActivate()
    {
        if (_isActive) return;

        _isActive = true;
        Scale = _originalScale;

        //Destroy this shields instance in SHIELD_DURATION sec
        StartCoroutine(DelayedDestroy());
    }

    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(SHIELD_DURATION);
        Exit();
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (!_isActive) return;
        if (_hitCount <= 0) return;

        /*
         *layer 30 and 31 is a missle (30 green, 31 red homing missle)
          i kind of imagine this like mario kart were the shield of shells block one attack
          and after each consecutive hit the shells circling the player decreases. To mimic that here
          i reduce the scale of the sphere to signify to the player that their shields
          are depleting. 7 is for obstacles, and when the player is shielded he is immune to getting
          derailed(BUMPED) from them.
         */

        int otherLayer = other.gameObject.layer;

        if (otherLayer == 7 || otherLayer == 30)
        {
            this._hitCount--;
        }
        else if (otherLayer == 31)
        {
            //insta take down ship shields!
            _hitCount = 0;
        }

        Scale = _originalScale * ((float)_hitCount / (float)MAX_HIT_COUNT);

        if (_hitCount <= 0) Exit();
    }
}
