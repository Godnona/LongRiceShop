using TMPro;
using UnityEngine;

public class PopupDailySumary : BasePopup
{
    [SerializeField] TMP_Text coinText;
    [SerializeField] TMP_Text serveText;

    private void OnEnable()
    {
        UpdateUI();
    }

    void UpdateUI() 
    {
        var progress = GameplayProgressManager.Instance; 
        coinText.text = progress.FinalCoin.ToString("N0"); 
        serveText.text = progress.FinalServe.ToString(); 
    }

    public override void Show()
    {
        base.Show();

        var progress = GameplayProgressManager.Instance;

        Debug.Log($"SUMMARY → Coin:{progress.FinalCoin} | Serve:{progress.FinalServe}");

        coinText.text = progress.FinalCoin.ToString("N0");
        serveText.text = progress.FinalServe.ToString();
    }

    public override void Hide()
    {
        AudioManager.Instance.PlaySFX(AudioClipName.UIClick);
        base.Hide();
        GameManager.Instance.OnClickNextLevel();
    }
}
