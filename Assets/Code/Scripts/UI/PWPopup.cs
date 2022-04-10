using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PWPopup : MonoBehaviour
{
    protected string password {
        get => GetComponentInChildren<InputField>().text;
    }

    public void Join()
    {
        // TODO: Finish this function to Join Online Game
        Debug.Log($"Join game with pw: {password}");
        LevelManager.Instance.LoadScene("Lobby");
    }

    public void Cancel()
    {
        this.gameObject.SetActive(false);
    }
}
