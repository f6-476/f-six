using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerListContainer : MonoBehaviour
{
    [SerializeField]
    private Transform entryParent;

    [SerializeField]
    private GameObject entryPrefab;

    public LobbyEntry current
    {
        get
        {
            LobbyEntry entry = null;

            foreach (Toggle toggle in entryParent.GetComponent<ToggleGroup>().ActiveToggles())
            {
                entry = toggle.gameObject.GetComponent<LobbyEntry>();
            }

            return entry;
        }
    }

    private void Start() 
    {
        RegistryManager.Singleton.RefreshRegistry(UpdateEntries);
    }

    public void Refresh()
    {
        RegistryManager.Singleton.RefreshServers(UpdateEntries);
    }

    private void UpdateEntries()
    {
        foreach (Transform child in entryParent)
        {
            Destroy(child.gameObject);
        }

        ToggleGroup toggleGroup = entryParent.GetComponent<ToggleGroup>();

        foreach (RegistryManager.Server server in RegistryManager.Singleton.Servers)
        {
            GameObject entryGameObject = Instantiate(entryPrefab, entryParent);
            LobbyEntry entry = entryGameObject.GetComponent<LobbyEntry>();
            entry.server = server;
            entry.group = toggleGroup;
        }
    }
}
