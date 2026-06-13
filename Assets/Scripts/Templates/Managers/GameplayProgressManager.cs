using System;
using UnityEngine;

public class GameplayProgressManager : MonoBehaviour
{
    public static GameplayProgressManager Instance { get; private set; }

    // ===== EVENT CHUẨN CHO UI =====
    public event Action<int, int> OnProgressUpdated;

    // ===== DATA =====
    int currentCoin;
    int currentServe;
    bool isRunning;

    // ===== BACKWARD COMPATIBILITY =====
    public int FinalCoin { get; private set; }
    public int FinalServe { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return; // 🔥 BẮT BUỘC
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ================= LEVEL FLOW =================

    // 🔥 GỌI KHI LOAD LEVEL
    public void BeginLevel()
    {
        currentCoin = 0;
        currentServe = 0;
        isRunning = true;

        PushToUI();
    }

    // 🔥 API CŨ – GameManager đang dùng
    public void StopProgress()
    {
        isRunning = false;
    }

    // 🔥 API CŨ – GameManager đang dùng
    public void FinalizeProgress()
    {
        FinalCoin = currentCoin;
        FinalServe = currentServe;

        Debug.Log($"FINALIZED → Coin:{FinalCoin} | Serve:{FinalServe}");
    }

    // ================= ADD DATA =================

    public void AddCoin(int amount)
    {
        if (!isRunning) return;

        currentCoin += amount;
        PushToUI();
    }

    public void AddServe()
    {
        if (!isRunning) return;

        currentServe++;
        PushToUI();
    }

    // ================= UI =================

    // 🔥 UI gọi khi mới xuất hiện
    public void ForceRefreshUI()
    {
        PushToUI();
    }

    void PushToUI()
    {
        OnProgressUpdated?.Invoke(currentCoin, currentServe);
    }

    // ================= GETTER (AN TOÀN) =================
    public int GetCurrentCoin() => currentCoin;
    public int GetCurrentServe() => currentServe;
}
