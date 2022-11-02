using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

[System.Serializable]
public class CardGenerator : MonoBehaviour
{
    
    [SerializeField] TMP_Text cardName;
    [SerializeField] Button showQrButton;
    [SerializeField] TMP_Text currency1Amount;
    [SerializeField] TMP_Text currency2Amount;
    [SerializeField] TMP_Text address;
    [SerializeField] RawImage qrCode;


    public CardData thisCardData;

    public void CopyAddressToClipboard()
    {
        MyXrunManager myXrunManager = GameObject.FindGameObjectWithTag("MyXrunManager").GetComponent<MyXrunManager>();
        if (!string.IsNullOrEmpty(thisCardData.address))
        {
            UniClipboard.SetText(thisCardData.address);
            myXrunManager.ActionOnAdressCopied();
        }
    }
    public void ShowQRCodeButtonClicked()
    {
        MyXrunManager myXrunManager = GameObject.FindGameObjectWithTag("MyXrunManager").GetComponent<MyXrunManager>();
        myXrunManager.ShowQrCodePanel();
    }

    public void Setup(CardData cardData)
    {
        this.cardName.text = cardData.displaystr;
        this.address.text = cardData.address;
        decimal currency1Final = decimal.Parse(cardData.amount);
        this.currency1Amount.text = $"{Math.Round(currency1Final, 2)} {cardData.symbol}";
        decimal currency2Final = decimal.Parse(cardData.eamount);
        this.currency2Amount.text = $"{cardData.countrySymbol} : {Math.Round(currency2Final, 2).ToString()}";
        this.thisCardData = cardData;

        CardQRCreator thisCardQRCreator = GetComponent<CardQRCreator>();
        thisCardQRCreator.CreateCode();
        //this.CardBackground.color = GetCardColor(allCardData.Data[i].file);
        //OnCardSetupFinished();
    }

    
}
