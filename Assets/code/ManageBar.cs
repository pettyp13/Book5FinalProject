using UnityEngine;
using TMPro;   // For TextMeshProUGUI

public class ManageBar : MonoBehaviour
{
    int value = 0;
    public string label = "Health";

    RectTransform fillRect;
    TextMeshProUGUI valueText;
    TextMeshProUGUI labelText;

    void Start()
    {
        // Grab references at startup
        Transform fill = transform.Find("fill");
        if (fill != null)
        {
            fillRect = fill.GetComponent<RectTransform>();
        }

        Transform textObj = transform.Find("text");
        if (textObj != null)
        {
            valueText = textObj.GetComponent<TextMeshProUGUI>();
        }

        Transform labelObj = transform.Find("label");
        if (labelObj != null)
        {
            labelText = labelObj.GetComponent<TextMeshProUGUI>();
            if (labelText != null) labelText.text = label;
        }

        UpdateValue();
    }

    public void IncreaseValue(int amount)
    {
        value += amount;
        if (value > 100) value = 100;
        if (value < 0) value = 0;
        UpdateValue();
    }

    public void SetValue(int amount)
    {
        value = amount;
        if (value > 100) value = 100;
        if (value < 0) value = 0;
        UpdateValue();
    }

    void UpdateValue()
    {
        // ----- Resize green bar using anchors -----
        if (fillRect != null)
        {
            // Bar always starts at left (0) and ends at value%
            Vector2 min = fillRect.anchorMin;
            Vector2 max = fillRect.anchorMax;

            min.x = 0f;
            max.x = value / 100f;   // 0.5 when value = 50

            fillRect.anchorMin = min;
            fillRect.anchorMax = max;
        }

        // ----- Update the number text -----
        if (valueText != null)
        {
            valueText.text = value.ToString();
        }
    }

    void Update()
    {
        // Book test key: press B to increase by 10
        if (Input.GetKeyDown(KeyCode.B))
        {
            IncreaseValue(10);
        }
    }
}
