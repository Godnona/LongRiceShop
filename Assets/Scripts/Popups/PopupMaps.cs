using System;
using DG.Tweening;
using UnityEngine;

public class PopupMaps : BasePopup
{
    [SerializeField] GameObject[] levelButtons;
    [SerializeField] GameObject[] levelDoneImages;

    [SerializeField] private float scaleUp = 1.5f;   // tỉ lệ phóng to
    [SerializeField] private float duration = 0.5f;  // thời gian mỗi tween

    protected override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        UpdateUI();
    }

    private void OnEnable()
    {
        GameManager.Instance.OnGameDataChanged += UpdateUI;
        UpdateUI(); // update ngay khi mở popup
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnGameDataChanged -= UpdateUI;
    }

    void UpdateUI()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i < GameManager.Instance.gameData.currentLevel - 1)
            {
                levelButtons[i].SetActive(true);
                levelDoneImages[i].SetActive(true);
            }
            else if (i == GameManager.Instance.gameData.currentLevel - 1)
            {
                levelButtons[i].SetActive(true);
                levelDoneImages[i].SetActive(false);
            }
            else
            {
                levelButtons[i].SetActive(false);
                levelDoneImages[i].SetActive(false);
            }
        }
    }

    public void OnClickLevel(int level)
    {
        GameManager.Instance.MapClick = level;

        PlayClickEffect(levelButtons[level - 1].transform);

        if (levelDoneImages[level - 1].activeSelf)
            PlayClickEffect(levelDoneImages[level - 1].transform);

        base.Hide();

        // LEVEL 6 → VÀO THẲNG GAME
        if (level >= 6)
        {
            GameManager.Instance.PlayContinue();
            return;
        }
        // LEVEL 1–5 → popup như cũ
        GameManager.Instance.PlayContinue();
        PopupManager.Instance.ShowPopup(PopupName.PopupLevelGoal);
    }

    void PlayClickEffect(Transform target)
    {
       target.DOKill(); // Hủy mọi tween đang chạy trên target tránh spam
       target.DOScale(scaleUp, duration).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
}
