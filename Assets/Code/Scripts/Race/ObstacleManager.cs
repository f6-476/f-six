using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : ObjectPlacement
{
    private static ObstacleManager _instance;

    public static ObstacleManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ObstacleManager>();
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
