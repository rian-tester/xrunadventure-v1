using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MissionItemGenerator : MonoBehaviour
{
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text dateAndTimeText;
    [SerializeField] TMP_Text amountText;

    public MissionData thisItemMissionData;

    public void Setup(MissionData missionData)
    {
        thisItemMissionData = missionData;
        titleText.text = missionData.title;
        dateAndTimeText.text = missionData.datetime;
        amountText.text = $"{missionData.amount} {missionData.symbol}";
    }
}
