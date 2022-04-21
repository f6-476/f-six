using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankingPopup : UIPopup
{
    [SerializeField] GameObject namesContainer;
    [SerializeField] private GameObject entryPrefab;

    public void SetPlayers(List<string> players)
    {
        foreach (Transform child in namesContainer.transform)
        {
            Destroy(child.gameObject);
        }

        int rank = 1;

        foreach (string player in players)
        {
            GameObject entryGameObject = Instantiate(entryPrefab, namesContainer.transform);
            RankingEntry entry = entryGameObject.GetComponent<RankingEntry>();
            entry.rank = $"{rank++}.";
            entry.playerName = player;
        }
    }
}
