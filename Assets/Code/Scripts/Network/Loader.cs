using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    private void Start() 
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
