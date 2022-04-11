using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HostPopup : MonoBehaviour
{
    private InputField passwordInput;
    protected string password
    {
        get => passwordInput.text;
    }

    private Toggle onlineToggle;
    bool online
    {
        get => onlineToggle.isOn;
    }

    private void Start()
    {
        passwordInput = GetComponentInChildren<InputField>();
        onlineToggle = GetComponentInChildren<Toggle>();
    }

    public void Host()
    {
        if (online)
        {
            StartCoroutine(ServerManager.Singleton.HostServer(password));
        }
        else
        {
            LobbyManager.Singleton.HostLobby(password);
        }
    }
}
