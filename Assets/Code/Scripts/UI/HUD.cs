using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HUD : MonoBehaviour
{
    GameObject quitPopupComponent;

    // Start is called before the first frame update
    void Start()
    {
        quitPopupComponent = GameObject.Find("Quit Popup");
        quitPopupComponent.SetActive(false);
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        quitPopupComponent.SetActive(true);
    }
}
