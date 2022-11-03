using ARLocation;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class CoinAvailableUI : MonoBehaviour
{

    [SerializeField] GameController gameController; 

    [SerializeField] TMP_Text coinAvailableText;

    GameController.AllCoinData thisServerData = new GameController.AllCoinData();
    private void Awake()
    {
        if (coinAvailableText == null)
        {
            coinAvailableText.GetComponent<CoinAvailableUI>();
        }
        gameController.OnAllCoinDataRetreived += UpdateText;
    }
    private void OnDestroy()
    {
        gameController.OnAllCoinDataRetreived -= UpdateText;
    }

    void UpdateText(GameController.AllCoinData serverData, Location playerLocation)
    {
        thisServerData = serverData;
        if (serverData != null)
        {
            coinAvailableText.text = $"Terdapat {serverData.data[0].Advertisement} Merek Advertisement dan {serverData.data.Count} coin untuk dikumpulkan";
        }
    }

}
