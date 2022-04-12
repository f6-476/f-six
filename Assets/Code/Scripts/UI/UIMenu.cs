using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class UIMenu : MonoBehaviour 
{
    public abstract void Back();

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
