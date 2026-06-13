using TMPro;
using UnityEngine;

public class UICoin : MonoBehaviour
{
    [SerializeField] TMP_Text coinText;

    GameplayProgressManager progress;

    void Awake()
    {
        
    }

    void OnEnable()
    {
        progress = GameplayProgressManager.Instance;
        if (progress == null) return;

        progress.OnProgressUpdated += OnProgressUpdated;

        // pull data ngay
        Refresh();
    }

    void OnDisable()
    {
        if (progress != null)
            progress.OnProgressUpdated -= OnProgressUpdated;
    }

    void OnProgressUpdated(int coin, int _)
    {
        coinText.text = coin.ToString();
    }

    void Refresh()
    {
        coinText.text = progress.GetCurrentCoin().ToString();
    }
}
