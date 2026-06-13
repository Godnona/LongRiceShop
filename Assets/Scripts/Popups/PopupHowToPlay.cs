using UnityEngine;

public class PopupHowToPlay : BasePopup
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override void Hide()
    {
        AudioManager.Instance.PlaySFX(AudioClipName.UIClick);

        base.Hide();

        GameManager.Instance.PlayContinue();
    }
}
