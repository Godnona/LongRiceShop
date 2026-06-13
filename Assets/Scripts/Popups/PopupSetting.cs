using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PopupSetting : BasePopup
{
    [Header("Volume Bars")]
    [SerializeField] private Image musicFill;
    [SerializeField] private Image sfxFill;
    [SerializeField] private RectTransform musicBarRect;
    [SerializeField] private RectTransform sfxBarRect;

    private const float MAX_VOLUME = 0.1f;
    private const int SORT_ORDER_IN_SETTING = 5;

    protected override void Awake()
    {
        // Test Music
        
        base.Awake();
    }

    // Thêm nhạc đóng popup
    public override void Hide()
    {
        AudioManager.Instance.PlaySFX(AudioClipName.UIClick);

        base.Hide();
    }

    private void Start()
    {
        // Lấy volume thật và quy đổi về fill (0-1)
        musicFill.fillAmount = AudioManager.Instance._MusicSource.volume / MAX_VOLUME;
        sfxFill.fillAmount = AudioManager.Instance._SfxSource.volume / MAX_VOLUME;
        gameObject.GetComponent<Canvas>().sortingOrder = SORT_ORDER_IN_SETTING;
    }

    // Cả click và drag đều gọi vào đây
    public void OnClickMusicBar(BaseEventData data) => UpdateVolume((PointerEventData)data, musicBarRect, musicFill, true);
    public void OnClickSfxBar(BaseEventData data) => UpdateVolume((PointerEventData)data, sfxBarRect, sfxFill, false);

    private void UpdateVolume(PointerEventData eventData, RectTransform rect, Image fill, bool isMusic)
    {
        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, eventData.position, eventData.pressEventCamera, out localPos);
        float width = rect.rect.width;
        float percent = Mathf.Clamp01((localPos.x / width) + 0.5f);

        fill.fillAmount = percent;

        float volume = percent * MAX_VOLUME;
        if (isMusic)
            AudioManager.Instance.SetMusicVolume(volume);
        else
            AudioManager.Instance.SetSFXVolume(volume);

        // Play Click Music
        AudioManager.Instance.PlaySFX(AudioClipName.UIClick);
    }
}
