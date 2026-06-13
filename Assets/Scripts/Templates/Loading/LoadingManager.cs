using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingManager : MonoBehaviour
{
    public static string sceneToLoad = "MainMenu";

    [SerializeField] private Image loadingFillImage;
    // [SerializeField] private TextMeshProUGUI loadingText;

    private void Start()
    {
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("No scene to load was specified!");
            return;
        }

        StartCoroutine(LoadAsyncOperation());
    }

    IEnumerator LoadAsyncOperation()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);
        asyncOperation.allowSceneActivation = false;

        float progress = 0f;

        while (!asyncOperation.isDone)
        {
            // Unity trả progress từ 0 → 0.9, phần 0.9 → 1 là khi scene kích hoạt
            float targetProgress = Mathf.Clamp01(asyncOperation.progress / 0.9f);

            // Lerp để thanh load mượt hơn
            progress = Mathf.MoveTowards(progress, targetProgress, Time.deltaTime * 0.5f);

            if (loadingFillImage)
                loadingFillImage.fillAmount = progress;

            // if (loadingText)
            //     loadingText.text = $"{Mathf.RoundToInt(progress * 100)}%";

            // Khi đạt 100%, cho phép load scene
            if (progress >= 1f && asyncOperation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.3f); // chờ 1 chút cho thanh load đầy
                asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
