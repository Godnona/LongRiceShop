using TMPro;
using UnityEngine;
using System.Collections;

public class UITimer : MonoBehaviour
{
    [SerializeField] TMP_Text timeText;

    void OnEnable()
    {
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        // Đợi GameplayTimer tồn tại (quan trọng khi build)
        while (GameplayTimer.Instance == null)
            yield return null;
        if (GameManager.Instance.GetCurrentLevel() >= 6)
        {
            gameObject.SetActive(false); // Ẩn luôn UI timer
            yield break;
        }

        GameplayTimer.Instance.OnTimeChanged += UpdateUI;
        // PULL DATA NGAY
        UpdateUI(GameplayTimer.Instance.GetTime());
    }

    void OnDisable()
    {
        if (GameplayTimer.Instance != null)
            GameplayTimer.Instance.OnTimeChanged -= UpdateUI;
    }

    void UpdateUI(float time)
    {
        int min = Mathf.FloorToInt(time / 60);
        int sec = Mathf.FloorToInt(time % 60);
        timeText.text = $"{min:00}:{sec:00}";
    }
}
