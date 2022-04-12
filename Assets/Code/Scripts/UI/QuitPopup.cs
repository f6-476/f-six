using UnityEngine.SceneManagement;

public class QuitPopup : UIPopup
{
    private void Update() 
    {
        
    }

    public void Quit()
    {
        if (LobbyManager.Singleton != null)
        {
            LobbyManager.Singleton.Disconnect();
        }
        else
        {
            SceneManager.LoadScene("RaceMenu");
        }
    }
}
