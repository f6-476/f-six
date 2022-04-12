using UnityEngine;
using UnityEngine.UI;

public class MainMenu : UIMenu
{
    public override void Back()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
