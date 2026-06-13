using System;
using UnityEngine;

public class GameplayTimer : MonoBehaviour
{
    public static GameplayTimer Instance;

    private float totalTime = 300f;
    private float currentTime;
    private bool isRunning;

    public Action<float> OnTimeChanged;
    public Action OnTimeUp;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.unscaledDeltaTime;
        currentTime = Mathf.Max(0, currentTime);

        OnTimeChanged?.Invoke(currentTime);

        if (currentTime <= 0)
        {
            isRunning = false;
            OnTimeUp?.Invoke();
        }
    }

    public void StartTimer(float time)
    {
        currentTime = time;
        isRunning = true;
        OnTimeChanged?.Invoke(currentTime);
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public float GetTime() => currentTime;
}
