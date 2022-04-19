using UnityEngine;
using UnityEngine.UI;

public class ShipPreview : MonoBehaviour
{
    [SerializeField] private Text shipText;
    [SerializeField] private Image speedBar;
    [SerializeField] private Image handlingBar;
    [SerializeField] private Image shieldBar;
    [SerializeField] private ShipConfig config;
    [SerializeField] private Transform shipContainer;

    private void Start()
    {
        RefreshShipModel();
    }

    private void RefreshShipModel()
    {
        if (config == null) return;

        int i = 0;
        foreach(Transform child in shipContainer)
        {
            child.gameObject.SetActive(i++ == config.prefabIndex);
        }
    }

    private void Update()
    {
        if (config == null) return;

        shipText.text = config.displayName;
        speedBar.fillAmount = config.speedRatio;
        handlingBar.fillAmount = config.handlingRatio;
        shieldBar.fillAmount = config.shieldRatio;
    }
}
