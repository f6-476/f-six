using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissleController : MonoBehaviour
{
    [SerializeField] private GameObject _missle;
    private Ship _ship;
    private ShipView _shipView;
    private Vector3 _missleSpawnPos;

    private void Awake()
    {
        //Destroy this shields instance in 10 sec
        Invoke(nameof(DestroyMe), 10.0f);
    }

    public void InitializeController(Ship owner)
    {
        _ship = owner;
        _shipView = owner.View;
        _missleSpawnPos = owner.gameObject.transform.Find("MissleSpawnPos").position;
    }

    // Update is called once per frame
    void Update()
    {
        //if user presses space
        /*
         * instantiate a missle on missle spawn pos
         */
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }

}
