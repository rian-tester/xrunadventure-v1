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

    [SerializeField] MultipleCoinPlacement multipleCoinPlacement;

    [SerializeField] TMP_Text coinAvailableText;

    MultipleCoinPlacement.ServerCoinData serverData = new MultipleCoinPlacement.ServerCoinData();
    private void Awake()
    {
        if (coinAvailableText == null)
        {
            coinAvailableText.GetComponent<CoinAvailableUI>();
        }
        multipleCoinPlacement.OnFinishCallServer += UpdateText;
    }
    private void OnDestroy()
    {
        multipleCoinPlacement.OnFinishCallServer -= UpdateText;
    }

    void UpdateText()
    {
        serverData = multipleCoinPlacement.GetRawServerData();
        if (serverData != null)
        {
            coinAvailableText.text = $"Terdapat {serverData.data[0].Advertisement} Merek Advertisement dan {serverData.data.Count} coin untuk dikumpulkan";
        }
    }

}
