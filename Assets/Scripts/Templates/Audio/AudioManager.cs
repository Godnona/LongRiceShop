using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    

    [Header("Audio Sources")]
    [SerializeField] private AudioSource _musicSource; // Dùng cho nhạc nền
    [SerializeField] private AudioSource _sfxSource;   // Dùng cho hiệu ứng âm thanh

    public AudioSource _MusicSource => _musicSource;
    public AudioSource _SfxSource => _sfxSource;

    [Header("Audio Clips (Kéo TẤT CẢ Audio Clips vào đây)")]
    // Sử dụng một danh sách chung để kéo thả tất cả các AudioClip vào Inspector.
    // Tên của AudioClip phải khớp với tên trong enum AudioClipName.
    [SerializeField] private List<AudioClip> _allAudioClips;

    // Dictionary để ánh xạ từ Enum (tên Audio Clip) sang AudioClip thực tế
    private Dictionary<AudioClipName, AudioClip> _audioClipDictionary;

    // Biến lưu trạng thái tắt tiếng của từng loại âm thanh
    private bool _isMusicMuted = false;
    private bool _isSfxMuted = false;

    // Biến lưu trữ âm lượng gốc (trước khi tắt tiếng) để khôi phục
    private float _originalMusicVolume = 0.1f; // Âm lượng mặc định cho nhạc nền
    private float _originalSfxVolume = 0.1f;   // Âm lượng mặc định cho SFX

    // Keys để lưu trạng thái vào PlayerPrefs
    private const string MUSIC_MUTE_KEY = "IsMusicMuted";
    private const string SFX_MUTE_KEY = "IsSfxMuted";
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SfxVolume";
    private float _masterVolume = 1f; // fake volumn để điều chỉnh âm thanh chung, Mặc định 100%
    private const string MASTER_VOLUME_KEY = "MasterVolume";



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ AudioManager tồn tại xuyên scene
        }

        InitializeAudioSources();
        InitializeAudioClipDictionary(); // Khởi tạo dictionary từ _allAudioClips
        LoadVolumeStates(); // Tải trạng thái âm lượng và áp dụng
    }

    private void InitializeAudioSources()
    {
        if (_musicSource == null)
        {
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.loop = true;
        }
        if (_sfxSource == null)
        {
            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.loop = false;
        }

        // Đặt âm lượng mặc định hoặc tải từ PlayerPrefs
        _originalMusicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.1f);
        _originalSfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 0.1f);
        _masterVolume = PlayerPrefs.GetFloat(MASTER_VOLUME_KEY, 1f);

        _musicSource.volume = _originalMusicVolume;
        _sfxSource.volume = _originalSfxVolume;
    }

    private void InitializeAudioClipDictionary()
    {
        _audioClipDictionary = new Dictionary<AudioClipName, AudioClip>();
        foreach (AudioClip clip in _allAudioClips)
        {
            if (clip != null)
            {
                if (System.Enum.TryParse(clip.name, out AudioClipName clipNameEnum))
                {
                    if (!_audioClipDictionary.ContainsKey(clipNameEnum))
                    {
                        _audioClipDictionary.Add(clipNameEnum, clip);
                    }
                    else
                    {
                        Debug.LogWarning($"AudioManager: Đã có AudioClip '{clip.name}' trong dictionary. Bỏ qua bản sao.");
                    }
                }
                else
                {
                    Debug.LogWarning($"AudioManager: Tên AudioClip '{clip.name}' không khớp với bất kỳ giá trị nào trong enum AudioClipName. Clip này sẽ không được thêm vào dictionary.");
                }
            }
            else
            {
                Debug.LogWarning("AudioManager: Có một phần tử null trong danh sách _allAudioClips. Vui lòng kiểm tra lại Inspector.");
            }
        }
        Debug.Log($"AudioManager: Đã tải {_audioClipDictionary.Count} Audio Clips vào dictionary.");
    }

    private void LoadVolumeStates()
    {
        _isMusicMuted = PlayerPrefs.GetInt(MUSIC_MUTE_KEY, 0) == 1; // 1 = muted, 0 = unmuted
        _isSfxMuted = PlayerPrefs.GetInt(SFX_MUTE_KEY, 0) == 1;

        // Áp dụng trạng thái tắt tiếng khi tải
        _musicSource.mute = _isMusicMuted;
        _sfxSource.mute = _isSfxMuted;

        Debug.Log($"AudioManager: Trạng thái âm lượng đã tải - Music Muted: {_isMusicMuted}, SFX Muted: {_isSfxMuted}");
    }

    private void SaveVolumeStates()
    {
        PlayerPrefs.SetInt(MUSIC_MUTE_KEY, _isMusicMuted ? 1 : 0);
        PlayerPrefs.SetInt(SFX_MUTE_KEY, _isSfxMuted ? 1 : 0);
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, _originalMusicVolume); // Lưu cả âm lượng gốc
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, _originalSfxVolume);     // để khôi phục sau
        PlayerPrefs.SetFloat(MASTER_VOLUME_KEY, _masterVolume);
        PlayerPrefs.Save();
        Debug.Log("AudioManager: Đã lưu trạng thái âm lượng.");
    }

    // --- Phương thức phát nhạc nền (sử dụng Enum và Dictionary) ---
    public void PlayMusic(AudioClipName musicName)
    {
        if (_audioClipDictionary == null)
        {
            Debug.LogError("AudioManager: _audioClipDictionary is null! Khởi tạo dictionary thất bại.");
            return;
        }

        if (!_audioClipDictionary.TryGetValue(musicName, out AudioClip clipToPlay))
        {
            Debug.LogWarning($"AudioManager: Không tìm thấy nhạc nền '{musicName}'.");
            return;
        }

        if (_musicSource != null)
        {
            if (_musicSource.clip == clipToPlay && _musicSource.isPlaying)
            {
                return;
            }
            _musicSource.clip = clipToPlay;
            _musicSource.Play();
            Debug.Log($"Playing Music: {musicName}");
        }
        else
        {
            Debug.LogWarning("AudioManager: Music AudioSource is null.");
        }
    }

    public void StopMusic()
    {
        if (_musicSource != null && _musicSource.isPlaying)
        {
            _musicSource.Stop();
            Debug.Log("Music Stopped.");
        }
    }

    // --- Phương thức phát hiệu ứng âm thanh (sử dụng Enum và Dictionary) ---
    public void PlaySFX(AudioClipName sfxName)
    {
        if (_sfxSource == null)
        {
            Debug.LogWarning("AudioManager: SFX AudioSource is null.");
            return;
        }
        if (_audioClipDictionary == null)
        {
            Debug.LogError("AudioManager: _audioClipDictionary is null! Khởi tạo dictionary thất bại.");
            return;
        }

        // Lỗi của bạn ở đây: sfxName đã là AudioClipName (enum), không cần .ToString() khi dùng với dictionary
        if (_audioClipDictionary.TryGetValue(sfxName, out AudioClip clipToPlay))
        {
            // _sfxSource.volume = 0.1f; // Không đặt cứng âm lượng ở đây nữa
            _sfxSource.PlayOneShot(clipToPlay);
            Debug.Log($"Playing SFX: {sfxName}");
        }
        else
        {
            Debug.LogWarning($"AudioManager: Không tìm thấy SFX clip '{sfxName}' trong dictionary. Vui lòng kiểm tra enum và tên file.");
        }
    }

    // --- Phương thức thay đổi âm lượng (giữ nguyên) ---
    public void SetMusicVolume(float volume)
    {
        _originalMusicVolume = volume; // Lưu lại âm lượng gốc
        if (_musicSource != null && !_isMusicMuted)
            _musicSource.volume = _originalMusicVolume * _masterVolume;
        SaveVolumeStates();
    }

    public void SetSFXVolume(float volume)
    {
        _originalSfxVolume = volume; // Lưu lại âm lượng gốc
        if (_sfxSource != null && !_isSfxMuted)
            _sfxSource.volume = _originalSfxVolume * _masterVolume;
        SaveVolumeStates();
    }

    public void SetMasterVolume(float volume)
    {
        _masterVolume = Mathf.Clamp01(volume);

        if (_musicSource != null && !_isMusicMuted)
            _musicSource.volume = _originalMusicVolume * _masterVolume;

        if (_sfxSource != null && !_isSfxMuted)
            _sfxSource.volume = _originalSfxVolume * _masterVolume;

        SaveVolumeStates();
    }


    // --- Phương thức bật/tắt nhạc nền ---
    public void ToggleMusic()
    {
        _isMusicMuted = !_isMusicMuted; // Đảo ngược trạng thái
        if (_musicSource != null)
        {
            _musicSource.mute = _isMusicMuted; // Sử dụng .mute để tắt tiếng
            // Nếu bạn muốn điều chỉnh volume về 0 thay vì mute
            // _musicSource.volume = _isMusicMuted ? 0 : _originalMusicVolume;
        }
        SaveVolumeStates();
        Debug.Log($"AudioManager: Nhạc nền đã {(_isMusicMuted ? "tắt" : "bật")}.");
        // PlaySFX(AudioClipName.TapSound); // Phát SFX khi toggle (nếu có)
    }

    // --- Phương thức bật/tắt hiệu ứng âm thanh ---
    public void ToggleSfx()
    {
        _isSfxMuted = !_isSfxMuted; // Đảo ngược trạng thái
        if (_sfxSource != null)
        {
            _sfxSource.mute = _isSfxMuted; // Sử dụng .mute để tắt tiếng
                                           // Nếu bạn muốn điều chỉnh volume về 0 thay vì mute
                                           // _sfxSource.volume = _isSfxMuted ? 0 : _originalSfxVolume;

        }
        SaveVolumeStates();
        Debug.Log($"AudioManager: SFX đã {(_isSfxMuted ? "tắt" : "bật")}.");
        // PlaySFX(AudioClipName.TapSound); // Phát SFX khi toggle (nếu có)
    }

        // --- Phương thức tạm dừng nhạc nền ---
    public void PauseMusic()
    {
        if (_musicSource != null && _musicSource.isPlaying)
        {
            _musicSource.Pause();
            Debug.Log("Music Paused.");
        }
    }

    // --- Phương thức tiếp tục nhạc nền ---
    public void ResumeMusic()
    {
        if (_musicSource != null && _musicSource.clip != null && !_musicSource.isPlaying)
        {
            _musicSource.UnPause();
            Debug.Log("Music Resumed.");
        }
    }


    // Các thuộc tính để kiểm tra trạng thái tắt tiếng từ bên ngoài (UI)
    public bool IsMusicMuted => _isMusicMuted;
    public bool IsSfxMuted => _isSfxMuted;
    public float MasterVolume => _masterVolume;
}
