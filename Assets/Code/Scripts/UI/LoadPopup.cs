using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadPopup : UIPopup
{
    private void Awake()
    {
        DontDestroyOnLoad(transform.parent.gameObject);
    }
}
