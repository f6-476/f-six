using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinPopup : UIPopup
{
    [HideInInspector]
    public LobbyEntry entry = null;

    [SerializeField]
    private UIField hostField;

    [SerializeField]
    private UIField passwordField;

    private void Start()
    {}

    public override void Show()
    {
        base.Show();

        if (entry == null)
        {
            hostField.Show();
        }
        else
        {
            hostField.Hide();
        }
    }

    public void Join()
    {
        if (entry)
        {
            ServerManager.Singleton.JoinServer(entry.server.id, passwordField.text);
        }
        else
        {
            ServerManager.Singleton.JoinUnlistedServer(hostField.text, passwordField.text);
        }
    }
}
