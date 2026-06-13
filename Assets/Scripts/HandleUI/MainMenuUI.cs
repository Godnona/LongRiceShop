// mainmenuUI
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    void Start()
    {
        // Nhạc nền Main Mennu khi mới vào
        AudioManager.Instance.PlayMusic(AudioClipName.MainMenu);
    }
    public void OnClickSetting()
    {
        AudioManager.Instance.PlaySFX(AudioClipName.UIClick);

        PopupManager.Instance.ShowPopup(PopupName.PopupSetting);
    }
    public void OnClickGuild()
    {
        AudioManager.Instance.PlaySFX(AudioClipName.UIClick);

        PopupManager.Instance.ShowPopup(PopupName.PopupHowToPlay);
    }

    // Vào mene level
    public void OnClickPlay()
    {
        AudioManager.Instance.PlaySFX(AudioClipName.UIClick);

        GameManager.Instance.PlayGame();
    }

    // Tiếp tục
    public void OnClickContinue()
    {
        AudioManager.Instance.PlaySFX(AudioClipName.UIClick);

        GameManager.Instance.PlayContinue();
    }

    public void OnClickHome()
    {
        AudioManager.Instance.PlaySFX(AudioClipName.UIClick);
        GameManager.Instance.Home();
    }    
}