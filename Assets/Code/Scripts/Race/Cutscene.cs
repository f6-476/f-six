using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System.Collections;

public class Cutscene : NetworkBehaviour
{
    [SerializeField] private Countdown countdown;
    [SerializeField] private Camera cutsceneCamera;
    [SerializeField] private Camera gameCamera;
    [SerializeField] private CutsceneSegment[] segments;
    [SerializeField] private Image fade;
    private int segmentIndex = -1;
    private float segmentStartTime = 0;
    private bool fadeOn = false;
    private float fadeStartTime = 0;
    private float fadeDuration = 0.5f;
    private float countdownWait = 1.0f;

    private void Start()
    {
        SetCameras(false);
        fadeStartTime = Time.time;
        StartCutscene();
    }

    private void SetCameras(bool isGame)
    {
        cutsceneCamera.enabled = !isGame;
        cutsceneCamera.GetComponent<AudioListener>().enabled = !isGame;
        gameCamera.enabled = isGame;
        gameCamera.GetComponent<AudioListener>().enabled = isGame;
    }

    private void Update()
    {
        UpdateFade();
        UpdateCamera();
    }

    private void UpdateFade()
    {
        if (!IsClient) return;

        fade.fillOrigin = fadeOn ? 0 : 1;
        fade.fillAmount = Mathf.Lerp(fadeOn ? 0.0f : 1.0f, fadeOn ? 1.0f : 0.0f, (Time.time - fadeStartTime) / fadeDuration);
    }

    private void UpdateCamera()
    {
        if (!IsClient) return;
        if (segmentIndex < 0) return;
        
        CutsceneSegment segment = segments[segmentIndex];
        float t = (Time.time - segmentStartTime) / segment.duration;

        cutsceneCamera.transform.position = Vector3.Lerp(segment.from.position, segment.to.position, t);
        cutsceneCamera.transform.rotation = Quaternion.Lerp(segment.from.rotation, segment.to.rotation, t);
    }

    private void StartCutscene()
    {
        if (NetworkManager.Singleton != null && !IsServer) return;
        StartCoroutine(StartServerCutscene());
    }

    private IEnumerator StartServerCutscene()
    {
        for (int i = 0; i < segments.Length; i++)
        {
            CutsceneSegment segment = segments[i];
            PlaySegmentClientRpc(i);
            yield return new WaitForSeconds(segment.duration - ((i == segments.Length - 1) ? fadeDuration : 0));
        }
        PlayCountdownClientRpc();
        yield return new WaitForSeconds(countdown.duration + fadeDuration * 2.0f + countdownWait);
        StartRaceClientRpc();
    }

    [ClientRpc]
    private void PlaySegmentClientRpc(int index)
    {
        segmentIndex = index;
        segmentStartTime = Time.time;
    }

    [ClientRpc]
    private void PlayCountdownClientRpc()
    {
        StartCoroutine(PlayCountdownAsync());
    }

    private void SetFade(bool state)
    {
        fadeOn = state;
        fadeStartTime = Time.time;
    }

    private IEnumerator PlayCountdownAsync() 
    {
        SetFade(true);
        yield return new WaitForSeconds(fadeDuration);
        SetCameras(true);
        SetFade(false);
        yield return new WaitForSeconds(fadeDuration + countdownWait);
        countdown.StartCountdown();
    }

    [ClientRpc]
    private void StartRaceClientRpc()
    {
        RaceManager.Singleton.OnRaceStart();
    }
}
