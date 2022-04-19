using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Map Config", menuName = "FSix/Config/Map")]
public class MapConfig : ScriptableObject
{
    public string sceneName;
    public string displayName;
    public int lapCount;
    public Sprite preview;
    public AudioClip music;
}
