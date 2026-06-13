using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaBackground : MonoBehaviour
{
    RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;

        Vector2 anchorMin = new Vector2(safeArea.x / Screen.width, safeArea.y / Screen.height);
        Vector2 anchorMax = new Vector2((safeArea.x + safeArea.width) / Screen.width,
                                        (safeArea.y + safeArea.height) / Screen.height);

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }
}
