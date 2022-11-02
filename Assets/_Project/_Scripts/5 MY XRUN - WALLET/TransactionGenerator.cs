using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TransactionGenerator : MonoBehaviour
{
    [SerializeField] TMP_Text dateAndTimetext;
    [SerializeField] TMP_Text amountText;
    [SerializeField] TMP_Text symbolText;

    public TransactionData thisItemTransaction;

    public void Setup(TransactionData transactionData)
    {
        thisItemTransaction = transactionData;
        dateAndTimetext.text = transactionData.datetimefull;
        amountText.text = $"{transactionData.amount} {transactionData.symbol}";
        symbolText.text = transactionData.symbol;
    }
}
