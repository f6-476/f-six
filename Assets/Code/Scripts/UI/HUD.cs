using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HUD : MonoBehaviour
{
    [SerializeField]
    private QuitPopup quitPopup;

    void Start()
    {
        quitPopup.Hide();
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        quitPopup.Show();
    }
}
