using UnityEngine;

public class CustomerOrder : MonoBehaviour
{
    public BowlState expectedState;
    public CustomerSpawner Spawner { get; set; }

    [Header("Payment")]
    public int payAmount = 30000; // tiền khách trả
    [HideInInspector] public bool isCorrectDish = false;

    [Header("Waiting Time")]
    public float maxWaitingTime = 30f;

    private float waitTimer;
    private bool isWaiting;

    public void StartWaiting()
    {
        waitTimer = maxWaitingTime;
        isWaiting = true;
    }

    public void StopWaiting()
    {
        isWaiting = false;
    }

    void Update()
    {
        if (!isWaiting) return;

        waitTimer -= Time.deltaTime;

        if (waitTimer <= 0f)
        {
            isWaiting = false;
            Debug.Log("Khách chờ quá lâu → bỏ đi");
            Leave(false);
        }
    }

    public bool CheckDish(BowlState dishState)
    {
        return dishState == expectedState;
    }

    public void Leave(bool leaveNormally = true)
    {
        if (Spawner != null)
            Spawner.CustomerLeave(this, leaveNormally);
    }
}
