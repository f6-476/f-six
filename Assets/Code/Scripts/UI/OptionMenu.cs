using UnityEngine;
using UnityEngine.UI;

public class OptionMenu : UIMenu
{
    [SerializeField] private InputField nameInput;

    private void Start() 
    {
        nameInput.text = AuthManager.Singleton.Username;    
    }

    public override void BackToMainMenu()
    {
        AuthManager.Singleton.Username = nameInput.text;
        base.BackToMainMenu();
    }
}
