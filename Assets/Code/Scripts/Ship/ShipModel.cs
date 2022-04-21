using UnityEngine;

[RequireComponent(typeof(Ship))]
public class ShipModel : MonoBehaviour
{
    [SerializeField] private GameObject[] models;
    public SyncVariable<int> ModelIndex = new SyncVariable<int>(0);

    private void Awake()
    {
        ModelIndex.OnSync += OnModelUpdate;
    }

    private void OnModelUpdate(int next)
    {
        for (int i = 0; i < models.Length; i++)
        {
            models[i].SetActive(i == next);
        }
    }
}
