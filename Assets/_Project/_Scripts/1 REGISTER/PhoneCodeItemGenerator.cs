using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhoneCodeItemGenerator : MonoBehaviour
{
    [SerializeField] Image flag;
    [SerializeField] TMP_Text phoneCode;
    [SerializeField] TMP_Text countryName;
    public CodeData ThisObjectData;
    public string InstanceName;
    public void Setup()
    {
        gameObject.name = ThisObjectData.country_en;
        InstanceName = ThisObjectData.country_en;
        flag.sprite = Resources.Load<Sprite>($"Flags/{ThisObjectData.country_code}");
        phoneCode.text = $"+ {ThisObjectData.phone_code}";
        countryName.text = ThisObjectData.country_en;
    }
    public void SetData(CodeData data)
    {
        ThisObjectData = data;
    }
    public void ChangeCurrentPosition()
    {
        PhoneCodeStatic.SetFlagImage(Resources.Load<Sprite>($"Flags/{ThisObjectData.country_code}"));
        PhoneCodeStatic.SetPhoneCode(ThisObjectData.phone_code);
        PhoneCodeStatic.SetCountryName(ThisObjectData.country_en);

        var rgtrPhnCodeManager = GameObject.FindGameObjectWithTag("CurrentPhoneCode").GetComponent<RegisterPhoneCodeManager>();
        rgtrPhnCodeManager.UpdatePhoneCodeField();

        Debug.Log(gameObject.name);
    }
}
