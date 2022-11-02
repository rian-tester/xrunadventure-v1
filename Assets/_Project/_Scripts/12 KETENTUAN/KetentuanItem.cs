using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

public class KetentuanItemData
{
    public string C { get; set; }   
}
public class KetentuanItem : MonoBehaviour
{
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text contentText;
    public string c { get; set; }

public string GetFirstLine(string content)
    {
        using (var reader = new StringReader(content))
        {
            string first = reader.ReadLine();
            Debug.Log(first);
            return first;
        }
    }
    public string GetAfterFirstLine(string content)
    {
        var lines = content.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        string restOfTheText = "";
        foreach (var item in lines)
        {
            restOfTheText = restOfTheText + item;
        }
        return restOfTheText;
    }

    public void Setup()
    {
        titleText.text = GetFirstLine(c);
        contentText.text = GetAfterFirstLine(c);
    }
}
