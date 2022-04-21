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
    private UIField discordField;

    [SerializeField]
    private Button hostButton;

    [SerializeField]
    private Button spectateButton;

    void Start()
    {
        if (AuthManager.Singleton != null)
        {
            twitchField.text = AuthManager.Singleton.TwitchChannel;
            twitchField.GetComponentInChildren<InputField>().onValueChanged.AddListener(UpdateTwitchSetting);

            discordField.text = AuthManager.Singleton.DiscordChannel;
            discordField.GetComponentInChildren<InputField>().onValueChanged.AddListener(UpdateDiscordSetting);
        }

        SetButtons(true);
    }

    private void SetButtons(bool isHost)
    {
        hostButton.gameObject.SetActive(isHost);
        spectateButton.gameObject.SetActive(!isHost);
    }

    private void UpdateCommon()
    {
        SetButtons(AuthManager.Singleton.TwitchChannel.Length == 0 && AuthManager.Singleton.DiscordChannel.Length == 0);
    }

    private void UpdateTwitchSetting(string channel)
    {
        AuthManager.Singleton.TwitchChannel = channel;
        UpdateCommon();
    }

    private void UpdateDiscordSetting(string channel)
    {
        AuthManager.Singleton.DiscordChannel = channel;
        UpdateCommon();
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
