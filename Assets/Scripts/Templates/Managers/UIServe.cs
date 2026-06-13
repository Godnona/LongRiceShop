using TMPro;
using UnityEngine;

public class UIServe : MonoBehaviour
{
    [SerializeField] TMP_Text serveText;

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

    void OnProgressUpdated(int _, int serve)
    {
        serveText.text = serve.ToString();
    }

    void Refresh()
    {
        serveText.text = progress.GetCurrentServe().ToString();
    }
}
