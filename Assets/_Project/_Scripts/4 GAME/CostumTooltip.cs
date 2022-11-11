using UnityEngine;

public class CostumTooltip : MonoBehaviour
{
    private void Start()
    {
        // Subscribe to the event preparation of tooltip style.
        OnlineMapsGUITooltipDrawer.OnPrepareTooltipStyle += OnPrepareTooltipStyle;
    }

    private void OnPrepareTooltipStyle(ref GUIStyle style)
    {
        // Change the style settings.
        style.fontSize = 20;
        style.fontStyle = FontStyle.Bold;
        style.fixedHeight = 100f;
        style.fixedWidth = 350f;
    }
}

