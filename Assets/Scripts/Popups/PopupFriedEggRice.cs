using UnityEngine;

public class PopupFriedEggRice : BasePopup
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override void Hide()
    {
        AudioManager.Instance.PlaySFX(AudioClipName.UIClick);

        base.Hide();

        PopupManager.Instance.ShowPopup(PopupName.PopupGrilledPorkChopRice);
    }
}
