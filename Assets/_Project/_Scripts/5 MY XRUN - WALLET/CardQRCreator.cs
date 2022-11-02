using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardQRCreator : MonoBehaviour
{
    [SerializeField] CodeWriter codeWriter;
    [SerializeField] RawImage previewImage;
    [SerializeField] CardGenerator thisCardGenerator;
    
    Texture2D targetTex;

    private void OnEnable()
    {
        CodeWriter.onCodeEncodeFinished += GetCodeImage;
    }
    private void OnDisable()
    {
        CodeWriter.onCodeEncodeFinished -= GetCodeImage;
    }

    public void CreateCode()
    {
        if (codeWriter != null)
        {
            codeWriter.CreateCode(CodeWriter.CodeType.QRCode, thisCardGenerator.thisCardData.address);
        }
    }
    public void GetCodeImage(Texture2D tex)
    {
        if (targetTex != null)
        {
            DestroyImmediate(targetTex, true);
        }
        targetTex = tex;
        RectTransform component = this.previewImage.GetComponent<RectTransform>();
        float y = component.sizeDelta.x * (float)tex.height / (float)tex.width;
        component.sizeDelta = new Vector2(component.sizeDelta.x, y);
        this.previewImage.texture = targetTex;
    }
}
