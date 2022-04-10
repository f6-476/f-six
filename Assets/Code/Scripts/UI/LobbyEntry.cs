using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyEntry : MonoBehaviour
{
    string _hostname;
    public string hostname {
        get => _hostname;
        set {
            _hostname = value;
            hostnameComponent.text = value;
        }
    }

    string _ping;
    public string ping {
        get => _ping;
        set {
            _ping = value;
            pingComponent.text = value;
            pingComponent.color = PingColor(value);
        }
    }

    string _playerCount;
    public string playerCount {
        get => _playerCount;
        set {
            _playerCount = value;
            playerCountComponent.text = value;
            
            if (value == "8/8")
            {
                toggleComponent.interactable = false;
                toggleComponent.isOn = false;
            }
            else
            {
                toggleComponent.interactable = true;
            }
        }
    }

    ToggleGroup _group;
    public ToggleGroup group {
        get => _group;
        set {
            _group = value;
            toggleComponent.group = value;
        }
    }

    Text hostnameComponent;
    Text playerCountComponent;
    Text pingComponent;
    Toggle toggleComponent;

    void Start()
    {
        hostnameComponent = gameObject.transform.Find("HostName").GetComponent<Text>();
        playerCountComponent = gameObject.transform.Find("PlayerCount").GetComponent<Text>();
        pingComponent = gameObject.transform.Find("Ping").GetComponent<Text>();
        toggleComponent = GetComponent<Toggle>();

        InvokeRepeating("PingHost", 1.0f, 5.0f);
    }

    void PingHost()
    {
        // TODO Ping Host
        string pingResponse = UnityEngine.Random.Range(15, 1000) + "";
        ping = pingResponse;

        string playerCountResponse = UnityEngine.Random.Range(1, 9) + "/8";
        playerCount = playerCountResponse;
    }

    Color PingColor(string value)
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
