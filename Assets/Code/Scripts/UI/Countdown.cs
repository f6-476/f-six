using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{
    [SerializeField] public Text number;
    public float duration = 3.0f;

    private void Awake()
    {
        number.text = "";
    }

    public void StartCountdown()
    {
        StartCoroutine(StartCountdownAsync());
    }

    private IEnumerator StartCountdownAsync()
    {
        number.text = "3";
        yield return new WaitForSeconds(duration / 3.0f);
        number.text = "2";
        yield return new WaitForSeconds(duration / 3.0f);
        number.text = "1";
        yield return new WaitForSeconds(duration / 3.0f);
        number.text = "GO!";
        yield return new WaitForSeconds(1.0f);
        number.text = "";
    }
}
