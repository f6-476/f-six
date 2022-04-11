using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PWPopup : MonoBehaviour
{
    public LobbyEntry entry;

    private InputField passwordInput;
    protected string password
    {
        get => passwordInput.text;
    }

    private void Start()
    {
        passwordInput = GetComponentInChildren<InputField>();
    }

    private IEnumerator JoinAsync()
    {
        yield return ServerManager.Singleton.JoinServer(entry.server.id, password);
    }

    public void Join()
    {
        StartCoroutine(JoinAsync());
    }

    public void Cancel()
    {
        this.gameObject.SetActive(false);
    }
}
