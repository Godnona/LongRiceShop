using UnityEngine.SceneManagement;

public class PopupClose : BasePopup
{
    const string MAIN_MENU_STRING = "MainMenu";
    protected override void Awake()
    {
        base.Awake();
    }

    public override void Hide()
    {
        AudioManager.Instance.PlaySFX(AudioClipName.UIClick);

        base.Hide();

        SceneManager.LoadScene(MAIN_MENU_STRING);
    }
}
