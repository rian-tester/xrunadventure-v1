using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARLocation;
using TMPro;

// This class placed in independent Object called CoinManager
// Exist in hierarchy

// This class handle : 
// updating coin collected by player
// updating coin collected UI, text, sound and animation

// future
// fetching server coin amount
// sending to server coin amount

public class CoinManager : MonoBehaviour
{
    [Header("Updating")]
    
    AudioManager audioManager;

    [Space(5)]
    [Header("Listening")]
    [SerializeField]
    Player player;


    private void Awake()
    {
        if (audioManager == null)
        {
            audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        }

    }
    private void OnEnable()
    {

        player.OnPlayerCatchCoin += UpdateAvailableCoin;
        player.OnPlayerCatchCoin += ActionWhenCoinTaken;
    }
    private void OnDisable()
    {
        player.OnPlayerCatchCoin -= UpdateAvailableCoin;
        player.OnPlayerCatchCoin -= ActionWhenCoinTaken;
    }

    public void ActionWhenCoinTaken() // Called by every Coin 
    {
        // TO DO
    }

    public void UpdateAvailableCoin()
    {
        // TO DO
    }

}
