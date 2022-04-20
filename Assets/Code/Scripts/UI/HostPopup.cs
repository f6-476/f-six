using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HostPopup : UIPopup
{
    [SerializeField]
    private UIField passwordField;

    [SerializeField]
    private UIField onlineField;

    [SerializeField]
    private UIField twitchField;

    [SerializeField]
    private GameObject hostButton;

    [SerializeField]
    private GameObject spectateButton;

    void Start()
    {
        if (AuthManager.Singleton != null)
        {
            twitchField.text = AuthManager.Singleton.TwitchChannel;
            twitchField.GetComponentInChildren<InputField>().onValueChanged.AddListener(UpdateTwitchSetting); 
        }

        SetButtons(true);
    }

    private void SetButtons(bool isHost)
    {
        hostButton.SetActive(isHost);
        spectateButton.SetActive(!isHost);
    }

    void UpdateTwitchSetting(string channel)
    {
        AuthManager.Singleton.TwitchChannel = channel;
        SetButtons(channel.Length == 0);
    }

    public override void Show()
    {
        if (RegistryManager.Singleton.IsConnected)
        {
            onlineField.Show();
        }
        else
        {
            onlineField.Hide();
        }

        base.Show();
    }

    public void HostSpectate()
    {
        // Use twitchField.text as well
        ServerManager.Singleton.HostServer(passwordField.text, onlineField.isOn, ClientMode.SPECTATOR);
    }

    public void Host()
    {
        ServerManager.Singleton.HostServer(passwordField.text, onlineField.isOn);
    }
}
