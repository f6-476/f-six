using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMenu : MonoBehaviour 
{
    public virtual void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public virtual void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
