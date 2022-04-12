using UnityEngine;
using UnityEngine.UI;

public class OptionMenu : UIMenu
{
    [SerializeField] 
    private UIField nameField;

    private void Start() 
    {
        nameField.text = AuthManager.Singleton.Username;    
    }

    public override void Back()
    {
        AuthManager.Singleton.Username = nameField.text;
        LoadScene("MainMenu");
    }
}
