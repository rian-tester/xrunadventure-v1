using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AdvertisementItemGenerator : MonoBehaviour
{
    [SerializeField] Button thisItemButton;
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text amount;
    [SerializeField] TMP_Text dateAndTime;
    [SerializeField] TMP_Text advNumber;

    public AdvertisementData thisAdvertisementData;

    private void OnDestroy()
    {
        thisItemButton.onClick.RemoveAllListeners();
    }
    public void Setup(AdvertisementData advertisementData)
    {
        thisAdvertisementData = advertisementData;
        title.text = advertisementData.title;
        amount.text = $"{advertisementData.amount} {advertisementData.symbol}";
        dateAndTime.text = advertisementData.datetime;
        advNumber.text = advertisementData.coin;
        thisItemButton.onClick.AddListener(this.CallAdPromptPanel);
    }

    void CallAdPromptPanel()
    {
        MyAdsManager managerInstance = GameObject.FindGameObjectWithTag("MyAdsManager").GetComponent<MyAdsManager>();

        if (managerInstance != null)
        {
            managerInstance.ShowAdsPanel(this.thisAdvertisementData);
        }
    }
}
