using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{
    [SerializeField] public Text number;
    [SerializeField] public AudioSource audioSource;
    public float duration = 3.0f;
    public int startNumber = 3;

    private void Awake()
    {
        number.text = "";
    }

    public void StartCountdown()
    {
        audioSource.Play();
        StartCoroutine(StartCountdownAsync());
    }

    private IEnumerator StartCountdownAsync()
    {
        for (int i = startNumber; i > 0; i--)
        {
            number.text = $"{i}";
            yield return new WaitForSeconds(duration / (float)startNumber);
        }

        number.text = "GO!";
        yield return new WaitForSeconds(1.0f);
        number.text = "";
    }
}
