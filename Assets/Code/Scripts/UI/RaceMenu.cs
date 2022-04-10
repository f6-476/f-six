using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RaceMenu : MonoBehaviour
{

    [SerializeField]
    GameObject lobbyEntryPrefab;

    GameObject lobbiesContainer;

    GameObject passwordPopup;
    GameObject hostPopup;

    LobbyEntry currentLobby {
        get {
            LobbyEntry entry = null;

            foreach(Toggle toggle in lobbiesContainer.GetComponent<ToggleGroup>().ActiveToggles())
            {
                entry = toggle.gameObject.GetComponent<LobbyEntry>();
            }
            
            return entry;
        }
    }

    void Start()
    {
        lobbiesContainer = GameObject.Find("LobbiesContainer");
        
        passwordPopup = GameObject.Find("PW Popup");
        passwordPopup.SetActive(false);

        hostPopup = GameObject.Find("Host Popup");
        hostPopup.SetActive(false);
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

    public void Refresh()
    {
        AddEntry("Test");
    }

    void AddEntry(string hostname)
    {
        GameObject lobbyEntryGameObject = Instantiate(lobbyEntryPrefab, lobbiesContainer.transform);

        LobbyEntry entry = lobbyEntryGameObject.GetComponent<LobbyEntry>();
        StartCoroutine(SetupEntry(entry, hostname));
    }

    IEnumerator SetupEntry(LobbyEntry entry, string hostname)
    {
        yield return null;
        entry.hostname = hostname;
        entry.playerCount = "1/8";
        entry.ping = "50";
        entry.group = lobbiesContainer.GetComponent<ToggleGroup>();
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
