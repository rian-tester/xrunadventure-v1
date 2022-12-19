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
    [SerializeField] MapController mapController;   
    [SerializeField] TMP_Text coinAvailableText;

    private void Awake()
    {
        if (coinAvailableText == null)
        {
            coinAvailableText.GetComponent<CoinAvailableUI>();
        }
        mapController.OnCoinMarkerPopulated += UpdateText;
    }
    private void OnDestroy()
    {
        mapController.OnCoinMarkerPopulated -= UpdateText;
    }

    void UpdateText(string advertisement, string coinAmount)
    {
       coinAvailableText.text = $"Terdapat {advertisement} Merek Advertisement dan {coinAmount} coin untuk dikumpulkan";
    }
}
