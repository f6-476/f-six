using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : ObjectPlacement
{
    private static PowerupManager _instance;

    public static PowerupManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PowerupManager>();
            }

            return _instance;
        }
    }
    void Start()
    {
        InitializeObjectPlacementVariables();
        prefabRadius = objectPrefab.transform.localScale.y;
        InitializeObjects();
    }

    public override void InitializeObjects()
    {
        for (int i = 0; i < objectCount; i++)
        {
            GameObject p = PlaceOneObject(this.transform);
        }
    }

}
