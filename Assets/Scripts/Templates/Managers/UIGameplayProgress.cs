using TMPro;
using UnityEngine;
using System.Collections;

public class UIGameplayProgress : MonoBehaviour
{
    [SerializeField] TMP_Text coinText;
    [SerializeField] TMP_Text serveText;

    void OnEnable()
    {
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        while (GameplayProgressManager.Instance == null)
            yield return null;

        GameplayProgressManager.Instance.OnProgressUpdated += UpdateUI;

        // pull data ngay
        GameplayProgressManager.Instance.ForceRefreshUI();
    }

    void OnDisable()
    {
        if (GameplayProgressManager.Instance != null)
            GameplayProgressManager.Instance.OnProgressUpdated -= UpdateUI;
    }

    void UpdateUI(int coin, int serve)
    {
        coinText.text = coin.ToString();
        serveText.text = serve.ToString();
    }
}
