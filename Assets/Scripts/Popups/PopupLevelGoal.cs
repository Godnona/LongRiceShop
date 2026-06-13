using System;
using TMPro;
using UnityEngine;

public class PopupLevelGoal : BasePopup
{
    [SerializeField] TMP_Text levelGoalText;
    [SerializeField] TMP_Text levelText;
    protected override void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        UpdateUIGoal();
    }

    public override void Show()
    {
        base.Show();
        UpdateUIGoal();
    }

    void UpdateUIGoal()
    {
        int level = GameManager.Instance.GetCurrentLevel();
        levelText.text = $"LEVEL {level}";

        switch (level)
        {
            case 1:
                levelGoalText.text = "300.000";
                break;
            case 2:
                levelGoalText.text = "500.000";
                break;
            case 3:
                levelGoalText.text = "700.000";
                break;
            case 4:
                levelGoalText.text = "900.000";
                break;
            default:
                levelGoalText.text = "1.200.000";
                break;
        } 
    }

    public override void Hide()
    {
        AudioManager.Instance.PlaySFX(AudioClipName.UIClick);

        base.Hide();

        // Tùy vào level hiện tại mà show popup new item phù hợp
        int level = GameManager.Instance.GetCurrentLevel();

        // LEVEL 6 → KHÔNG SHOW GÌ
        if (level >= 6) return;

        // ===== SHOW POPUP MÓN MỚI =====
        switch (level)
        {
            case 1:
                PopupManager.Instance.ShowPopup(PopupName.PopupFriedEggRice);
                break;
            case 2:
                PopupManager.Instance.ShowPopup(PopupName.PopupRiceWithFriedFish);
                break;
            case 3:
                PopupManager.Instance.ShowPopup(PopupName.PopupBraisedPorkRice);
                break;
            case 4:
                PopupManager.Instance.ShowPopup(PopupName.PopupChickenRice);
                break;
            case 5:
                PopupManager.Instance.ShowPopup(PopupName.PopupSeafoodDeluxeRice);
                break;

        }
    }
}
