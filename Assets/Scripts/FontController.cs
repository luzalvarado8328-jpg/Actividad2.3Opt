using UnityEngine;
using UnityEngine.UI;

public class FontController : MonoBehaviour
{
    public Text targetText;
    public int fontSizeStep = 4;

    private Color[] colors = new Color[]
    {
        Color.black,
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow,
        Color.white
    };

    private int currentColorIndex = 0;
    private bool isBold = false;
    private bool isItalic = false;

    public void IncreaseFontSize()
    {
        if (targetText != null)
        {
            targetText.fontSize += fontSizeStep;
        }
    }

    public void DecreaseFontSize()
    {
        if (targetText != null && targetText.fontSize > fontSizeStep)
        {
            targetText.fontSize -= fontSizeStep;
        }
    }

    public void ChangeFontColor(int colorIndex)
    {
        if (targetText != null && colorIndex >= 0 && colorIndex < colors.Length)
        {
            currentColorIndex = colorIndex;
            targetText.color = colors[currentColorIndex];
        }
    }

    public void CycleColor()
    {
        currentColorIndex = (currentColorIndex + 1) % colors.Length;
        if (targetText != null)
        {
            targetText.color = colors[currentColorIndex];
        }
    }

    public void ToggleBold()
    {
        if (targetText != null)
        {
            isBold = !isBold;
            UpdateFontStyle();
        }
    }

    public void ToggleItalic()
    {
        if (targetText != null)
        {
            isItalic = !isItalic;
            UpdateFontStyle();
        }
    }

    private void UpdateFontStyle()
    {
        if (isBold && isItalic)
            targetText.fontStyle = FontStyle.BoldAndItalic;
        else if (isBold)
            targetText.fontStyle = FontStyle.Bold;
        else if (isItalic)
            targetText.fontStyle = FontStyle.Italic;
        else
            targetText.fontStyle = FontStyle.Normal;
    }
}
