using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RaceMenu : MonoBehaviour
{

    [SerializeField]
    private GameObject lobbyEntryPrefab;
    private GameObject lobbiesContainer;
    private GameObject passwordPopup;
    private GameObject hostPopup;

    public LobbyEntry currentLobby
    {
        get
        {
            LobbyEntry entry = null;

            foreach (Toggle toggle in lobbiesContainer.GetComponent<ToggleGroup>().ActiveToggles())
            {
                entry = toggle.gameObject.GetComponent<LobbyEntry>();
            }

            return entry;
        }
    }

    private void Start()
    {
        lobbiesContainer = GameObject.Find("LobbiesContainer");

        passwordPopup = GameObject.Find("PW Popup");
        passwordPopup.SetActive(false);

        hostPopup = GameObject.Find("Host Popup");
        hostPopup.SetActive(false);

        StartCoroutine(ServerManager.Singleton.RefreshMainServer(UpdateEntries));
    }

    public void JoinPopup()
    {
        if (currentLobby != null)
        {
            passwordPopup.SetActive(true);
        }
    }

    public void HostPopup()
    {
        hostPopup.SetActive(true);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Refresh()
    {
        StartCoroutine(ServerManager.Singleton.RefreshServers(UpdateEntries));
    }

    private void UpdateEntries()
    {
        foreach(Transform child in lobbiesContainer.transform) 
        {
            Destroy(child.gameObject);
        }

        ToggleGroup toggleGroup = lobbiesContainer.GetComponent<ToggleGroup>();
        foreach(ServerManager.Server server in ServerManager.Singleton.Servers)
        {
            GameObject lobbyEntryGameObject = Instantiate(lobbyEntryPrefab, lobbiesContainer.transform);
            LobbyEntry entry = lobbyEntryGameObject.GetComponent<LobbyEntry>();
            entry.server = server;
            entry.group = toggleGroup;
        }
    }
}
