using UnityEngine;
using UnityEngine.UI;

public class OptionMenu : UIMenu
{
    [SerializeField] private UIField nameField;
    [SerializeField] private Slider musicSlider;

    private void Start() 
    {
        if (AuthManager.Singleton != null) nameField.text = AuthManager.Singleton.Username;
        if (MusicManager.Singleton != null) musicSlider.value = MusicManager.Singleton.Volume;
    }

    private void Update()
    {
        if (MusicManager.Singleton != null) MusicManager.Singleton.Volume = musicSlider.value; 
    }

    public override void Back()
    {
        if (AuthManager.Singleton != null) AuthManager.Singleton.Username = nameField.text;
        LoadScene("MainMenu");
    }
}
