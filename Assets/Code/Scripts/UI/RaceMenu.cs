using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void Start()
    {
        joinPopup.Hide();
        hostPopup.Hide();
    }

    public override void Back()
    {
        LoadScene("MainMenu");
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
