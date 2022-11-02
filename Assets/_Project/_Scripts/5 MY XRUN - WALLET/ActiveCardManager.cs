using System;
using UnityEngine;
using UnityEngine.UI;
using AsPerSpec;

public class ActiveCardManager : MonoBehaviour
{
    [SerializeField] GameObject cardsParent;
    [SerializeField] CardGenerator activeCard;
    [SerializeField] CarouselToggler carouselToggler;
    [SerializeField] CardFetcher cardFetcher;

    public event Action OnActiveCardSet;

    private void Awake()
    {
        carouselToggler.OnSnapEnded += SetActiveCard;
        cardFetcher.OnFirstCardsFetched += SetActiveCard;
    }
    private void OnDestroy()
    {
        carouselToggler.OnSnapEnded -= SetActiveCard;
        cardFetcher.OnFirstCardsFetched -= SetActiveCard;
    }

    public CardGenerator GetActiveCard()
    {
        return activeCard;
    }
    void SetActiveCard(Toggle isOnToggle)
    {
        activeCard = isOnToggle.gameObject.GetComponent<CardGenerator>();
        
        if (OnActiveCardSet != null)
        {
            OnActiveCardSet();
        }

        StoreActiveCardData(activeCard);
    }

    void StoreActiveCardData(CardGenerator activeCard)
    {
        ActiveCardDataStatic.Eamount = activeCard.thisCardData.eamount;
        ActiveCardDataStatic.Krw = activeCard.thisCardData.krw;
        ActiveCardDataStatic.CountrySymbol = activeCard.thisCardData.countrySymbol;
        ActiveCardDataStatic.Samount = activeCard.thisCardData.samount; 
        ActiveCardDataStatic.Examount = activeCard.thisCardData.examount;
        ActiveCardDataStatic.Amount = activeCard.thisCardData.amount;
        ActiveCardDataStatic.Wamount = activeCard.thisCardData.wamount;
        ActiveCardDataStatic.Camount = activeCard.thisCardData.camount; 
        ActiveCardDataStatic.Wallet = activeCard.thisCardData.wallet;
        ActiveCardDataStatic.Address = activeCard.thisCardData.address; 
        ActiveCardDataStatic.Currency = activeCard.thisCardData.currency;
        ActiveCardDataStatic.Subcurrency = activeCard.thisCardData.subcurrency;
        ActiveCardDataStatic.CurrencyName = activeCard.thisCardData.currencyname;
        ActiveCardDataStatic.Symbol = activeCard.thisCardData.symbol;
        ActiveCardDataStatic.Ratio = activeCard.thisCardData.ratio;
        ActiveCardDataStatic.File = activeCard.thisCardData.file;
        ActiveCardDataStatic.Name = activeCard.thisCardData.name;
        ActiveCardDataStatic.Size = activeCard.thisCardData.size;
        ActiveCardDataStatic.Type = activeCard.thisCardData.type;
        ActiveCardDataStatic.Datetime = activeCard.thisCardData.datetime;
        ActiveCardDataStatic.Minchange = activeCard.thisCardData.minchange;
        ActiveCardDataStatic.Displaystr = activeCard.thisCardData.displaystr;
    }
}
