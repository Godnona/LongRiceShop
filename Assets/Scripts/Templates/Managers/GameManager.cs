using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-100)] // Đảm bảo GameManager chạy trước các script khác
public class GameManager : MonoBehaviour
{
    // Biến Singleton (Instance duy nhất)
    public static GameManager Instance { get; private set; }

    private const string MAIN_GAME = "MainGame";
    private const string MAIN_MENU = "MainMenu";

    private const int TOTAL_LEVEL_STRING = 6;
    private const string MAIN_MENU_STRING = "MainMenu";


    // Get Level In Map
    int mapClick = 0;
    public int MapClick
    {
        get { return mapClick; }
        set { mapClick = value; }
    }

    // Dữ liệu Game (Bắt buộc phải công khai hoặc có thuộc tính SerializeField)


    [HideInInspector] public GameData gameData;

    // Event để thông báo cho UI và các đối tượng khác biết dữ liệu đã thay đổi
    public event Action OnGameDataChanged;


    private void Awake()
    {
        // Kiểm tra và thiết lập Singleton
        if (Instance == null)
        {
            Instance = this;
            // Giữ lại Manager khi chuyển Scene
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Nếu đã tồn tại, hủy bản sao này
            Destroy(gameObject);
            return;
        }

        // Tải dữ liệu khi game bắt đầu. Nếu file không tồn tại, nó trả về GameData() mới
        gameData = SaveSystem.LoadGame();
    }

    void Start()
    {
        StartCoroutine(WaitForTimer());
    }

    IEnumerator WaitForTimer()
    {
        while (GameplayTimer.Instance == null)
            yield return null;

        GameplayTimer.Instance.OnTimeUp += HandleTimeUp;
    }


    private void OnEnable()
    {
        if (GameplayTimer.Instance != null)
            GameplayTimer.Instance.OnTimeUp += HandleTimeUp;
    }

    private void OnDisable()
    {
        if (GameplayTimer.Instance != null)
            GameplayTimer.Instance.OnTimeUp -= HandleTimeUp;
    }

    private void OnApplicationQuit()
    {
        // Tự động lưu game khi ứng dụng bị đóng (An toàn tuyệt đối)
        SaveSystem.SaveGame(gameData);
    }

    // --- HÀM LƯU TẬP TRUNG ---
    // Chỉ gọi hàm này sau khi MỌI thay đổi dữ liệu hoàn tất
    public void SaveGameData()
    {
        // 1. Thông báo cho UI cập nhật
        OnGameDataChanged?.Invoke();

        // 2. Lưu vào file
        SaveSystem.SaveGame(gameData);
    }

    // -- HÀM LAY DỮ LIỆU --
    public int GetCurrentLevel()
    {
        if (mapClick == 0)
            return gameData.currentLevel;
        else
            return mapClick;
    }

    int GetGoalByLevel()
    {
        switch (GetCurrentLevel())
        {
            case 1: return 300000;
            case 2: return 500000;
            case 3: return 700000;
            case 4: return 900000;
            case 5: return 1200000;
            default: return int.MaxValue;
        }
    }

    // --- CÁC HÀM LOGIC GAMEPLAY ---

    // Vào chơi

    bool IsInfiniteLevel()
    {
        return GetCurrentLevel() >= 6;
    }   

    // START COUNTDOWN
    public void HandleTimeUp()                                          
    {
        if (IsInfiniteLevel())
            return;

        var progress = GameplayProgressManager.Instance;

        // STOP GAME LOGIC
        GameplayTimer.Instance.StopTimer();
        progress.StopProgress();

        // CHỐT DATA
        progress.FinalizeProgress();

        int goal = GetGoalByLevel();

        Dog dog = FindAnyObjectByType<Dog>();
        if (dog != null)
            dog.startDelay = 100000;

        if (progress.FinalCoin >= goal)
            GameWin();
        else
            GameOver();
    }

    public void PlayGame()
    {
        PopupManager.Instance.ShowPopup(PopupName.PopupMaps);
    }
    public void PlayContinue()
    {
        int level = GetCurrentLevel();
        string sceneName = "Test" + (level <= 5 ? level.ToString() : "6");
        SceneManager.LoadScene(sceneName);

        // Start countdown
        SceneManager.sceneLoaded += OnGameplaySceneLoaded;
        // SceneManager.LoadScene(MAIN_GAME);
    }

    private void OnGameplaySceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!scene.name.StartsWith("Test"))
            return;

        GameplayProgressManager.Instance.BeginLevel();

        if (IsInfiniteLevel())
            GameplayTimer.Instance.StopTimer();
        else
            GameplayTimer.Instance.StartTimer(GetTimeByLevel());

        //GameplayProgressManager.Instance.InitByLevel();
        //GameplayProgressManager.Instance.StartProgress();

        SceneManager.sceneLoaded -= OnGameplaySceneLoaded;
    }

    float GetTimeByLevel()
    {
        switch (GetCurrentLevel())
        {
            case 1: return 300f;
            case 2: return 300f;
            case 3: return 300f;
            case 4: return 300f;
            case 5: return 300f;
            case 6: return 300f;
            default: return int.MaxValue;
        }
    }


    public void Home()
    {
        SceneManager.LoadScene(MAIN_MENU);
    }

    public void Pause()
    {
        Time.timeScale = 0;
        AudioManager.Instance.PauseMusic();
    }

    public void Resume()
    {
        Time.timeScale = 1;
        AudioManager.Instance.ResumeMusic();
    }

    public void GameWin()
    {
        AudioManager.Instance.StopMusic();

        int playingLevel = GetCurrentLevel();
        int savedLevel = gameData.currentLevel;

        // CHỈ UNLOCK KHI CHƠI ĐÚNG LEVEL CAO NHẤT
        if (playingLevel == savedLevel && savedLevel < TOTAL_LEVEL_STRING)
        {
            gameData.currentLevel++;
            SaveGameData();
        }

        // reset mapClick sau khi xong game
        mapClick = 0;

        PopupManager.Instance.ShowPopup(PopupName.PopupDailySummary);
    }

    public void OnClickNextLevel()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(MAIN_MENU_STRING);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == MAIN_MENU_STRING)
        {
            PopupManager.Instance.ShowPopup(PopupName.PopupMaps);
            // Hủy đăng ký để tránh gọi nhiều lần
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    public void GameOver()
    {
        //Time.timeScale = 0;
        // ... (Logic dừng nhạc, hiển thị UI Game Over) ...
        Debug.Log("Game Over");
        AudioManager.Instance.StopMusic();

        PopupManager.Instance.ShowPopup(PopupName.PopupClose2);
    }

    public void GameOver2()
    {
        //Time.timeScale = 0;
        // ... (Logic dừng nhạc, hiển thị UI Game Over) ...
        Debug.Log("Game Over");
        AudioManager.Instance.StopMusic();

        PopupManager.Instance.ShowPopup(PopupName.PopupLose);
    }

    public void OnWinFinalLevel()
    {
        StartCoroutine(LoadMainMenuAndTrigger());
    }

    IEnumerator LoadMainMenuAndTrigger()
    {
        SceneManager.LoadScene(MAIN_MENU_STRING);
        yield return null; // đợi 1 frame để scene load

        PopupManager.Instance.ShowPopup(PopupName.PopupMaps);
    }

    // Debug
    public void UnlockAllLevels()
    {
        gameData.currentLevel = TOTAL_LEVEL_STRING;

        SaveGameData(); // vừa lưu vừa update UI

        // Force refresh popup map nếu đang mở
        PopupManager.Instance.ShowPopup(PopupName.PopupMaps);

        Debug.Log("All levels unlocked!");
    }

    public void ResetProgress()
    {
        gameData.currentLevel = 1;
        mapClick = 0;

        SaveGameData();

        // Force refresh popup map nếu đang mở
        PopupManager.Instance.ShowPopup(PopupName.PopupMaps);

        Debug.Log("Progress reset!");
    }

    // Cách sử dụng OnDataChange (tự động cập nhật UI khi data thay đổi): 
    // private void OnEnable()
    // {
    //     // 1. Lắng nghe (Subscribe) sự kiện
    //     // Khi GameManager.OnGameDataChanged được gọi, hàm UpdateUI sẽ tự động chạy
    //     GameManager.Instance.OnGameDataChanged += UpdateUI;

    //     // Cập nhật UI ngay lần đầu tiên sau khi subscribe
    //     UpdateUI();
    // }

    // private void OnDisable()
    // {
    //     // 2. Hủy đăng ký (Unsubscribe) sự kiện
    //     // ĐIỀU NÀY RẤT QUAN TRỌNG để tránh lỗi và Memory Leak (rò rỉ bộ nhớ)
    //     if (GameManager.Instance != null)
    //     {
    //         GameManager.Instance.OnGameDataChanged -= UpdateUI;
    //     }
    // }

    // // Hàm này sẽ được gọi mỗi khi dữ liệu game thay đổi
    // private void UpdateUI()
    // {
    //     // Lấy giá trị ngọc trai mới nhất và cập nhật Text
    //     int currentPearls = GameManager.Instance.GetPearl();
    //     pearlText.text = currentPearls.ToString();

    //     Debug.Log("UI Updated: Pearl count is now " + currentPearls);
    // }
}