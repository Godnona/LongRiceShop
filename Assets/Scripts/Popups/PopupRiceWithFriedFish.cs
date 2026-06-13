using UnityEngine;

public class PopupRiceWithFriedFish : BasePopup
{
    protected override void Awake()
    {
        base.Awake();

        Debug.Log("Dừng spawn Người khi hiện popup");
    }

    public override void Hide()
    {
        AudioManager.Instance.PlaySFX(AudioClipName.UIClick);

        base.Hide();

        Debug.Log("Bắt đầu spawn Người lại khi ẩn popup");
    }
}
