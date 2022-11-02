using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPhoneCodeManager : MonoBehaviour
{
    [Serializable]
    public class AllCodeData
    {
        public CodeData[] data;
    }

    [SerializeField] Image flag;
    [SerializeField] TMP_Text countryCode;
    [SerializeField] TMP_Text countryName;
    [SerializeField] TextAsset jsonFile;
    [SerializeField] Transform parentForData;
    [SerializeField] PhoneCodeItemGenerator phoneCodePrefab;
    [SerializeField] TMP_InputField searchCountryField;
    
    public AllCodeData allCodeData = new AllCodeData();

    private void Awake()
    {
        ReadJsonFile();
        UpdatePhoneCodeField();
        PopulatePhoneCodeItem();
        searchCountryField.onValueChanged.AddListener(delegate { FilterPhoneCodeItem(searchCountryField.text); });
    }
    public void GoBackButtonClicked()
    {
        StartCoroutine(SceneLoader.LoadScene(1, 0.5f));
    }
    void ReadJsonFile()
    {
        allCodeData = JsonUtility.FromJson<AllCodeData>(jsonFile.text);

        if (PhoneCodeStatic.CountryName == null)
        {
            return;
        }
    }
    public void UpdatePhoneCodeField()
    {
        flag.sprite = PhoneCodeStatic.FlagImage;
        countryCode.text = PhoneCodeStatic.PhoneCode;
        countryName.text = PhoneCodeStatic.CountryName;
    }

    void PopulatePhoneCodeItem()
    {
        foreach(Transform child in parentForData.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < allCodeData.data.Length; i++)
        {
            PhoneCodeItemGenerator instance = Instantiate(phoneCodePrefab, parentForData);
            instance.SetData(allCodeData.data[i]);
            instance.Setup();
        }
    }

    public void FilterPhoneCodeItem(string filterResult)
    {
        foreach (Transform child in parentForData.transform)
        {
            Destroy(child.gameObject);
        }

        Debug.Log(filterResult);
        ReadJsonFile();

        for (int i = 0; i < allCodeData.data.Length; i++)
        {
            string s = allCodeData.data[i].country_en;
            if (s.Contains(filterResult) == true)
            {
                PhoneCodeItemGenerator instance = Instantiate(phoneCodePrefab, parentForData);
                instance.SetData(allCodeData.data[i]);
                instance.Setup();
            }
        }
    }
}
