using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CountdownTimer : MonoBehaviour
{
    [SerializeField]
    Text countdownText;
    int countdownTime;

    IEnumerator CountdownToStart()
    {
        while(countdownTime > 0)
        {
            countdownText.text = countdownTime + "";

            yield return new WaitForSeconds(1f);

            countdownTime--;
        }

        countdownText.text = "GO!";
        
        // START RACE CALLBACK HERE?

        yield return new WaitForSeconds(1f);

        countdownText.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        countdownTime = 3;
    }

    public void StartCountdown()
    {
        StartCoroutine(CountdownToStart());
    }
}
