using UnityEngine;

[CreateAssetMenu(fileName = "Placement Config", menuName = "Placement Config")]
public class PlacementConfig : ScriptableObject
{
    [SerializeField] private GameObject prefab;
    public GameObject Prefab => prefab;
    [SerializeField] [Range(2, 100)] private int count = 20;
    public int Count => count;
    [SerializeField] [Range(2, 500)] private float minDistance = 200;
    public float MinDistance => minDistance;
}
