using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyEntry : MonoBehaviour
{
    public RegistryManager.Server server;

    private ToggleGroup _group;
    public ToggleGroup group {
        get => _group;
        set {
            _group = value;
            toggleComponent.group = value;
        }
    }

    private Text hostnameText;
    private Text playerCountText;
    private Text pingText;
    private Toggle toggleComponent;

    private void Awake()
    {
        hostnameText = gameObject.transform.Find("HostName").GetComponent<Text>();
        playerCountText = gameObject.transform.Find("PlayerCount").GetComponent<Text>();
        pingText = gameObject.transform.Find("Ping").GetComponent<Text>();
        toggleComponent = GetComponent<Toggle>();

        InvokeRepeating("PingHost", 1.0f, 5.0f);
    }

    private void PingHost()
    {
        // TODO Ping Host
        string pingResponse = UnityEngine.Random.Range(15, 1000) + "";
        pingText.color = PingColor(pingResponse);
        pingText.text = pingResponse;
    }

    private void Update()
    {
        hostnameText.text = server.name;
        playerCountText.text = $"{server.count}/8";
    }

    private Color PingColor(string value)
    {
        Color color = Color.black;

        try
        {
            int result = Int32.Parse(value);

            if (result < 100)
            {
                ColorUtility.TryParseHtmlString("#45BF55", out color);
            }
            else if (result < 150)
            {
                ColorUtility.TryParseHtmlString("#FFBD00", out color);
            }
            else if (result < 200)
            {
                ColorUtility.TryParseHtmlString("#FF5400", out color);
            }
            else if (result < 300)
            {
                ColorUtility.TryParseHtmlString("#E00000", out color);
            }
            else
            {
                ColorUtility.TryParseHtmlString("#850035", out color);
            }
        }
        catch (FormatException)
        {
            ColorUtility.TryParseHtmlString("#0A0A0A", out color);
        }

        return color;
    }
}
