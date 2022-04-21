using UnityEngine;
using UnityEngine.UI;

public class ShipPreview : MonoBehaviour
{
    [SerializeField] private Text shipText;
    [SerializeField] private Image speedBar;
    [SerializeField] private Image handlingBar;
    [SerializeField] private Image shieldBar;
    [SerializeField] private Transform shipContainer;
    private LobbyPlayer localPlayer = null;

    private void Start()
    {
        if (LobbyManager.Singleton.LocalPlayer != null) OnLocal(LobbyManager.Singleton.LocalPlayer);
        LobbyManager.Singleton.OnLocal += OnLocal;
    }

    private void OnDestroy()
    {
        LobbyManager.Singleton.OnLocal -= OnLocal;
        if (localPlayer != null) localPlayer.modelIndex.OnValueChanged -= OnModelChange;
    }

    private void OnLocal(LobbyPlayer player)
    {
        if (localPlayer != null) return;
        localPlayer = player;

        localPlayer.modelIndex.OnValueChanged += OnModelChange;
        RefreshPreview();
    }

    private void OnModelChange(int previous, int next)
    {
        if (previous == next) return;

        RefreshPreview();
    }

    private void RefreshPreview()
    {
        if (localPlayer == null) return;
        ShipConfig config = RaceManager.Singleton.ShipConfigs[localPlayer.ModelIndex];

        int i = 0;
        foreach(Transform child in shipContainer)
        {
            child.gameObject.SetActive(i++ == config.prefabIndex);
        }

        shipText.text = config.displayName;
        speedBar.fillAmount = config.speedRatio;
        handlingBar.fillAmount = config.handlingRatio;
        shieldBar.fillAmount = config.shieldRatio;
    }
}
