using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;

public class RegionDropdownHandler : MonoBehaviour
{
    public TextAsset jsonRegion;
    RegionDataCollection regionDataCollection;
    [SerializeField]
    TMP_Dropdown regionDropdown;
    [SerializeField]
    Dropdown regionDropdownUnity;
    int regionCode;
    private void Awake()
    {
        regionDropdownUnity.ClearOptions();
    }
    private void Start()
    {
        FileReader();
    }
    private void Update()
    {
        //string regionName = regionDropdown.options[regionDropdown.value].text;
        //regionDropdown.captionText.text = regionName;
    }
    void FileReader()
    {
        regionDataCollection = JsonConvert.DeserializeObject<RegionDataCollection>(jsonRegion.text);
        PopulateCode();
    }
    void PopulateCode()
    {
        regionDropdownUnity.ClearOptions();

        List<Dropdown.OptionData> regionsOption = new List<Dropdown.OptionData>();
        foreach (var region in regionDataCollection.data)
        {
            string regionNamewithRegionCode = $"{region.RegionName} ({region.RegionCode})";
            var option = new Dropdown.OptionData(regionNamewithRegionCode);
            regionsOption.Add(option);
            Debug.Log(option);
        }

        regionDropdownUnity.AddOptions(regionsOption);

    }
    void changeValue()
    {
        string regionName = regionDropdownUnity.options[regionDropdown.value].text;
        regionDropdownUnity.captionText.text = regionName;
    }

}
public class RegionDataCollection
{
    public List<RegionData> data;
}
public struct RegionData
{
    public string RegionCode { get; set; }
    public string RegionName { get; set; }
}
