using TMPro;
using UnityEngine;

public class UIProgressPresenter : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TMP_Text coinText;
    [SerializeField] TMP_Text serveText;

    GameplayProgressManager progress;

    void Awake()
    {
        progress = GameplayProgressManager.Instance;
    }

    void OnEnable()
    {
        if (progress == null) return;

        // Lắng nghe
        progress.OnProgressUpdated += OnProgressUpdated;

        // 🔥 Pull data NGAY LẬP TỨC (KEY BUILD-SAFE)
        Refresh();
    }

    void OnDisable()
    {
        if (progress != null)
            progress.OnProgressUpdated -= OnProgressUpdated;
    }

    void OnProgressUpdated(int coin, int serve)
    {
        SetUI(coin, serve);
    }

    void Refresh()
    {
        SetUI(
            progress.GetCurrentCoin(),
            progress.GetCurrentServe()
        );
    }

    void SetUI(int coin, int serve)
    {
        coinText.text = coin.ToString();
        serveText.text = serve.ToString();
    }
}
