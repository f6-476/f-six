using UnityEngine;
using UnityEngine.UI;

public class MapPreview : MonoBehaviour
{
    [SerializeField] private Text mapText;
    [SerializeField] private Text lapText;
    [SerializeField] private Image mapImage;

    private void Update()
    {
        if (LobbyManager.Singleton == null) return;
        MapConfig config = LobbyManager.Singleton.MapConfig;

        mapText.text = config.displayName;
        lapText.text = $"{config.lapCount} Laps";
        mapImage.sprite = config.preview;
    }
}
