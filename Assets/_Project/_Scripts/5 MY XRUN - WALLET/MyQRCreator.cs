using System.Collections; 
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VoxelBusters.EssentialKit;
using System;

public class MyQRCreator : MonoBehaviour
{
    [SerializeField] ActiveCardManager activeCardManager;

    CardGenerator activeCard;

    [Header("QR Creator")] 
    public CodeWriter codeWtr;
    [SerializeField] RawImage previewImg;
    [SerializeField] CodeWriter.CodeType codeType;
    [SerializeField] Texture2D targetTex;
    [SerializeField] Text errorText;

    public event Action OnQrImageSaved;

    void OnEnable()
    {
        CodeWriter.onCodeEncodeFinished += GetCodeImage;
        CodeWriter.onCodeEncodeError += errorInfo;
        activeCardManager.OnActiveCardSet += Create_Code;

    }
    private void OnDestroy()
    {
        CodeWriter.onCodeEncodeFinished -= GetCodeImage;
        CodeWriter.onCodeEncodeError -= errorInfo;
        activeCardManager.OnActiveCardSet -= Create_Code;
    }

    public void Create_Code()
    {
        if (codeWtr != null)
        {
            activeCard = activeCardManager.GetActiveCard();
            codeWtr.CreateCode(codeType, activeCard.thisCardData.address);
        }
    }
    public void GetCodeImage(Texture2D tex)
    {
        if (targetTex != null)
        {
            DestroyImmediate(targetTex, true);
        }
        targetTex = tex;
        RectTransform component = this.previewImg.GetComponent<RectTransform>();
        float y = component.sizeDelta.x * (float)tex.height / (float)tex.width;
        component.sizeDelta = new Vector2(component.sizeDelta.x, y);
        previewImg.texture = targetTex;
    }
 
    public void errorInfo(string str)
    {
        errorText.text = str;
    }
    public void SaveIamgeToGallery()
    {
        if (targetTex != null)
        {
            MediaController.SaveImageToGallery(targetTex);
            if (OnQrImageSaved != null)
            {
                OnQrImageSaved();
            }
        }
    }
    public void ShareImage()
    {
        if (targetTex != null)
        {
            Texture2D texture = targetTex;

            ShareSheet shareSheet = ShareSheet.CreateInstance();
            shareSheet.AddImage(texture);
            shareSheet.SetCompletionCallback((result, error) => {
                Debug.Log("Share Sheet was closed. Result code: " + result.ResultCode);
            });
            shareSheet.Show();
        }

    }

    public Texture2D GetCodeTexture()
    {
        return targetTex;
    }
}
