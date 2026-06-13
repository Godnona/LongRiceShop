using UnityEngine;
using DG.Tweening; // nếu bạn dùng DOTween cho hiệu ứng

public class BasePopup : MonoBehaviour
{
    [Header("Popup Root")]
    [SerializeField] protected CanvasGroup canvasGroup;

    protected virtual void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.25f);
        transform.localScale = Vector3.one * 0.8f;
        transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack);

    }

    public virtual void Hide()
    {
        canvasGroup.DOFade(0f, 0.2f);
        transform.DOScale(0.8f, 0.2f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });

    }
}
