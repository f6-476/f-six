using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HostPopup : PWPopup
{
    bool online {
        get => GetComponentInChildren<Toggle>().isOn;
    }
    public void Host()
    {
        //TODO
        Debug.Log($"Host an {(online?"online":"offline")} game with pw: {base.password}");
    }
}
