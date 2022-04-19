using UnityEngine;
using UnityEngine.UI;

public class OptionMenu : UIMenu
{
    [SerializeField] 
    private UIField nameField;

    private void Start() 
    {
        if (AuthManager.Singleton != null) nameField.text = AuthManager.Singleton.Username;    
    }

    public override void Back()
    {
        if (AuthManager.Singleton != null) AuthManager.Singleton.Username = nameField.text;
        LoadScene("MainMenu");
    }
}
