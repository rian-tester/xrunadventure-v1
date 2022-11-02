using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZXing;

public class QrCodeReaderCostum : Panel
{
    [SerializeField] RawImage rawImageBackground;
    [SerializeField] AspectRatioFitter aspectRatioFitter;
    [SerializeField] TMP_Text resultText;
    [SerializeField] RectTransform scanZone;

    string resultPrompt = null;
    bool isCamAvailable;
    bool isScanDone = false;
    WebCamTexture cameraTexture;

    private void Start()
    {
        resultText.text = "";

        currentAlpha = 0;
        desiredAlpha = 0;
        promptPanel.gameObject.SetActive(false);

        SetupCamera();
    }
    private void Update()
    {
        UpdateCameraRender();
        if (isScanDone == false)
        {
            Scan();
        }

        UpdatePromptTextPanelAlpha();
    }
    public void GoBackButtonClicked()
    {
        StartCoroutine(SceneLoader.LoadScene(17, 0.25f));
    }
    public void OnClickResultText()
    {
        if (resultPrompt != null)
        {
            ShowPromptTextPanelForSeconds(resultPrompt, 1f);
        }
        else
        {
            string noResult = $"There is no available scan result to copy";
            ShowPromptTextPanelForSeconds(noResult, 1f);
        }
    }

    void SetupCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            isCamAvailable = false;
            return;
        }

        for(int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isFrontFacing == false)
            {
                cameraTexture = new WebCamTexture(devices[i].name, (int )scanZone.rect.width, (int)scanZone.rect.height);   
            }
        }
        
        cameraTexture.Play();
        rawImageBackground.texture = cameraTexture;
        isCamAvailable = true;  
    }
    void UpdateCameraRender()
    {
        if (isCamAvailable == false)
        {
            return;
        }

        float ratio = (float)cameraTexture.width / (float)cameraTexture.height;
        aspectRatioFitter.aspectRatio = ratio;
        int orientation = -cameraTexture.videoRotationAngle;
        rawImageBackground.rectTransform.localEulerAngles = new Vector3(0, 0, orientation);
    }
    void Scan()
    {
        try
        {
            IBarcodeReader barcodeReader = new BarcodeReader();
            var snap = new Texture2D(cameraTexture.width, cameraTexture.height, TextureFormat.ARGB32, false);
            snap.SetPixels32(cameraTexture.GetPixels32());
            Result result = barcodeReader.Decode(snap.GetRawTextureData(), cameraTexture.width, cameraTexture.height, RGBLuminanceSource.BitmapFormat.RGB32);

            if (result != null)
            {
                resultText.text = result.Text;
                isScanDone = true;
                resultPrompt = $"{resultText.text} copied to clipboard";
                ShowPromptTextPanelForSeconds(resultPrompt, 1f);
                UniClipboard.SetText(resultText.text);
            }
            else
            {
                resultText.text = "Failed to scan QR Code, please make sure QR code inside the scan zone";
            }
        }
        catch
        {
            resultText.text = "Please scan the code";
        }
    }
}
