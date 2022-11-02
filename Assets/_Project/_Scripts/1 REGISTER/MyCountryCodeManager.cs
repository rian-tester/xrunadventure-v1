using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MyCountryCodeManager : MonoBehaviour
{
    [System.Serializable]
    public class MyData
    {
        public CodeData[] data;
    }

    [System.Serializable]
    public class MyCountryName
    {
        public List<string> Name;
    }

    public MyData code;
    new public MyCountryName name;
    public Sprite[] flags;
    public List<string> countryCodesList;
    
    public TextAsset jsonFile;
    public TMP_Dropdown MyCountryCodeDropdown;
    
    
    string countryCode;
    void Start()
    {
        //jsonFile = Resources.Load("Codes/CountryCode") as TextAsset;
        if (jsonFile != null)
            FileReader();
        countryCodesList = new List<string>();
        countryCodesList.Clear();
        SelectCountryBasedSystem();
    }

    void Update()
    {
        if (!string.IsNullOrEmpty(MyCountryCodeDropdown.captionText.text))
        {
            string[] country_code = MyCountryCodeDropdown.options[MyCountryCodeDropdown.value].text.Split(new[] { "   (+" }, StringSplitOptions.None);
            string code = country_code[1].Remove(country_code[1].Length - 1);
            countryCode = "" + code;
            MyCountryCodeDropdown.captionText.text = countryCode;
        }
    }

    void FileReader()
    {
        code = JsonUtility.FromJson<MyData>(jsonFile.text);
        name = JsonUtility.FromJson<MyCountryName>(jsonFile.text);
        flags = new Sprite[code.data.Length];
        populateCode();
    }

    void populateCode()
    {
        MyCountryCodeDropdown.ClearOptions();
        // Create the option list
        List<TMP_Dropdown.OptionData> flagItems = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < code.data.Length; i++)
        {
            Resources.Load<Sprite>("Sprites/my_sprite");
            flags[i] = Resources.Load<Sprite>("Flags/" + code.data[i].country_code.ToLower().ToString());
            string flagName = $"+{code.data[i].phone_code}";
            var flagOption = new TMP_Dropdown.OptionData(flagName, flags[i]);
            flagItems.Add(flagOption);
        }
        MyCountryCodeDropdown.AddOptions(flagItems);
    }

    void SelectCountryBasedSystem()
    {
        //string systemCountry = Application.systemLanguage.ToString();
        //int activeDropdownIndex;

        //for (int i = 0; i < MyCountryCodeDropdown.options.Count; i++)
        //{
        //    print(MyCountryCodeDropdown.options[0].text);
        //}
    }
}

