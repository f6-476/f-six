using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIField : MonoBehaviour
{
    [SerializeField]
    private InputField inputField;

    [SerializeField]
    private Toggle toggle;

    public string text 
    {
        get 
        {
            if (inputField) return inputField.text;
            else if (toggle) return toggle.isOn ? "On" : "Off";
            else return "";
        }
        set
        {
            if (inputField) inputField.text = value;
        }
    }

    public bool isOn
    {
        get
        {
            if (inputField) return inputField.text.Trim().Length > 0;
            else if (toggle) return toggle.isOn;
            else return false;
        }
        set
        {
            if (toggle) toggle.isOn = value;
        }
    }

    public virtual void Show()
    {
        this.gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
