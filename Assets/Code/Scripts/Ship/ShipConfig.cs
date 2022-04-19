using UnityEngine;

[CreateAssetMenu(fileName = "Ship Config", menuName = "FSix/Config/Ship")]
public class ShipConfig : ScriptableObject
{
    public string displayName;
    public int prefabIndex;
    public float speedRatio;
    public float handlingRatio;
    public float shieldRatio;
}
