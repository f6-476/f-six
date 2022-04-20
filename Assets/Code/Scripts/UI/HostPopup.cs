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
    private Button hostButton;

    [SerializeField]
    private Button spectateButton;

    public void OnTwitchChannelChanged(string channel)
    {
        Debug.Log(channel);
    }

    void Start()
    {
        twitchField.GetComponentInChildren<InputField>().onValueChanged.AddListener(UpdateTwitchSetting); 
        
        hostButton.interactable = true;
        spectateButton.interactable = false;
    }

    void UpdateTwitchSetting(string channel)
    {
        if (channel.Length > 0)
        {
            hostButton.interactable = false;
            spectateButton.interactable = true;
        }
        else
        {
            hostButton.interactable = true;
            spectateButton.interactable = false;
        }
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
