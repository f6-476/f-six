using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : AbstractManager<MusicManager>
{
    private enum State
    {
        MENU, GAME
    }

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float maxMusicVolume = 0.05f;
    [SerializeField] private AudioClip[] lobbyMusic;
    public float MusicVolume 
    {
        get => audioSource.volume / maxMusicVolume;
        set => audioSource.volume = value * maxMusicVolume;
    }
    private State state = State.MENU;

    private void Start()
    {
        audioSource.volume = maxMusicVolume;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Update()
    {
        if (!audioSource.isPlaying) UpdateMusic();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        State nextState = State.MENU;
        if (scene.name.StartsWith("Map")) nextState = State.GAME;

        if (state != nextState)
        {
            state = nextState;
            UpdateMusic();
        }
    }

    private void UpdateMusic()
    {
        AudioClip music = null;
        switch (state)
        {
            case State.MENU:
                music = lobbyMusic[Random.Range(0, lobbyMusic.Length - 1)];
                break;
            case State.GAME:
                music = LobbyManager.Singleton.MapConfig.music;
                break;
        }

        audioSource.Stop();
        audioSource.clip = music;
        audioSource.Play();
    }
}
