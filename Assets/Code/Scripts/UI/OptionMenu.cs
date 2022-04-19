using UnityEngine;
using UnityEngine.UI;

public class OptionMenu : UIMenu
{
    [SerializeField] private UIField nameField;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider gameVolumeSlider;

    private void Start() 
    {
        if (AuthManager.Singleton != null) nameField.text = AuthManager.Singleton.Username;
        if (MusicManager.Singleton != null) musicVolumeSlider.value = MusicManager.Singleton.MusicVolume;
        gameVolumeSlider.value = AudioListener.volume;
    }

    private void Update()
    {
        if (MusicManager.Singleton != null) MusicManager.Singleton.MusicVolume = musicVolumeSlider.value;
        AudioListener.volume = gameVolumeSlider.value;
    }

    public override void Back()
    {
        if (AuthManager.Singleton != null) AuthManager.Singleton.Username = nameField.text;
        LoadScene("MainMenu");
    }
}
