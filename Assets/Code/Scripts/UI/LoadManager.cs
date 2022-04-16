using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : AbstractManager<LoadManager>
{
    [SerializeField] private UIPopup popup;
    private Scene? loadingScene = null;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
        SceneManager.activeSceneChanged += OnActiveSceneChanged;    
    }

    private void OnActiveSceneChanged(Scene sceneFrom, Scene sceneTo)
    {
        loadingScene = sceneTo;
        popup.Show();
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene != loadingScene) return;

        loadingScene = null;
        popup.Hide();
    }
}
