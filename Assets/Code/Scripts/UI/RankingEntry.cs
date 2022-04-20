using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingEntry : MonoBehaviour
{

    public string rank {
        get {
            if (!rankText)
            {
                rankText = gameObject.transform.Find("Rank").GetComponent<Text>();
            }

            return rankText.text;
        }
        set {
            if (!rankText)
            {
                rankText = gameObject.transform.Find("Rank").GetComponent<Text>();
            }
            rankText.text = value;
        }
    }

    public string playerName {
        get {
            if (!nameText)
            {
                nameText = gameObject.transform.Find("Name").GetComponent<Text>();
            }

            return nameText.text;
        }
        set {
            if (!nameText)
            {
                nameText = gameObject.transform.Find("Name").GetComponent<Text>();
            }
            nameText.text = value;
        }
    }

    private Text rankText;
    private Text nameText;

    void Awake()
    {
        rankText = gameObject.transform.Find("Rank").GetComponent<Text>();
        nameText = gameObject.transform.Find("Name").GetComponent<Text>();
    }
}
