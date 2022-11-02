using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhoneCodeManager : MonoBehaviour
{
    [Serializable]
    public class AllCodeData
    {
        public CodeData[] data;
    }

    [SerializeField] Image flag;
    [SerializeField] TMP_Text countryCode;
    [SerializeField] TextAsset jsonFile;

    public AllCodeData allCodeData = new AllCodeData();

    private void Awake()
    {
        if (flag.sprite == null && PhoneCodeStatic.CountryName == null)
        {
            ReadJsonFile();
            SaveDataToStatic(0);
        }
        UpdatePhoneCodeField();
    }

    void UpdatePhoneCodeField()
    {
        flag.sprite = PhoneCodeStatic.FlagImage;
        countryCode.text = PhoneCodeStatic.PhoneCode;
    }
    void ReadJsonFile()
    {
        allCodeData = JsonUtility.FromJson<AllCodeData>(jsonFile.text);
    }

    void SaveDataToStatic(int indexPhoneCodeData)
    {
        PhoneCodeStatic.SetPhoneCode($"+ {allCodeData.data[indexPhoneCodeData].phone_code}");
        PhoneCodeStatic.SetCountryName(allCodeData.data[indexPhoneCodeData].country_en);
        Sprite flagSprite = Resources.Load<Sprite>($"Flags/{allCodeData.data[indexPhoneCodeData].country_code}");
        PhoneCodeStatic.SetFlagImage(flagSprite);
    }
}
