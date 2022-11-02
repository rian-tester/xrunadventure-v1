using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingAnimationColor : MonoBehaviour
{
    [SerializeField] Image targetImage;
    [SerializeField][Range(0f, 3f)] float lerpTime;
    [SerializeField] Color[] myColors;

    int colorIndex = 0;
    float t = 0f;
    int len;

    private void Start()
    {
        len = myColors.Length;
    }
    private void Update()
    {
        targetImage.color = Color.Lerp(targetImage.color, myColors[colorIndex], lerpTime * Time.deltaTime);

        t = Mathf.Lerp(t, 1f, lerpTime * Time.deltaTime);
        if (t > 0.9f)
        {
            t = 0f;
            colorIndex++;
            colorIndex = (colorIndex >= len) ? 0 : colorIndex;
        }
    }
}
