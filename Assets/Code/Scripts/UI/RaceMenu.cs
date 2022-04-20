using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaceMenu : UIMenu
{
    [SerializeField]
    private GameObject lobbyEntryPrefab;

    [SerializeField]
    private ServerListContainer serverList;

    [SerializeField]
    private JoinPopup joinPopup;
    
    [SerializeField]
    private HostPopup hostPopup;
    
    [SerializeField]
    private UIField nameField;

    private void Start()
    {
        if (AuthManager.Singleton != null) {
            nameField.text = AuthManager.Singleton.Username;
            nameField.GetComponentInChildren<InputField>().onValueChanged.AddListener(UpdateUsername);
        }
        
        joinPopup.Hide();
        hostPopup.Hide();
    }

    public override void Back()
    {
        LoadScene("MainMenu");
    }

    void UpdateUsername(string username)
    {
        AuthManager.Singleton.Username = username;
    }

    public void JoinPopup()
    {
        LobbyEntry entry = serverList.current;

        if (entry == null)
        {
            joinPopup.entry = null;
        }
        else
        {
            joinPopup.entry = entry;
        }

        joinPopup.Show();
    }

    public void HostPopup()
    {
        hostPopup.Show();
    }

    public void Refresh()
    {
        serverList.Refresh();
    }
}
