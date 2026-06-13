using UnityEngine;
using TMPro;
using System.Collections;

public class NotificationManager : MonoBehaviour
{
    [SerializeField] private TMP_Text notificationText;
    [SerializeField] private float showDuration = 2f;   // thời gian hiển thị
    [SerializeField] private float fadeDuration = 1f;   // thời gian mờ dần

    private Coroutine currentRoutine;

    public void ShowNotification(string message)
    {
        ShowNotification(message, Color.white); // mặc định màu trắng
    }

    public void ShowNotificationRed(string message)
    {
        ShowNotification(message, Color.red);
    }

    public void ShowNotificationGreen(string message)
    {
        ShowNotification(message, Color.green);
    }

    private void ShowNotification(string message, Color color)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowAndFade(message, color));
    }

    private IEnumerator ShowAndFade(string message, Color color)
    {
        notificationText.text = message;
        notificationText.color = color;
        notificationText.alpha = 1f;

        // Giữ nguyên trong showDuration
        yield return new WaitForSeconds(showDuration);

        // Mờ dần
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            notificationText.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        notificationText.alpha = 0f;
        currentRoutine = null;
    }

    public void HideInstant()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        notificationText.alpha = 0f;
        currentRoutine = null;
    }
}
