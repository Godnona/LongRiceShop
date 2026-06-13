using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Dog : MonoBehaviour
{
    public enum DogState
    {
        Idle,
        Running,
        Carried,
        Resetting,
        Finished
    }

    [Header("Movement")]
    public Transform targetPoint;
    public float moveSpeed = 3f;
    public float startDelay = 3f;
    public Rigidbody rb;
    public Collider pickupCollider;

    [Header("Animation")]
    public Animator animator;
    private static readonly int WalkHash = Animator.StringToHash("Walk");

    [Header("Video")]
    public VideoPlayer catchDogVideo;

    [Header("Video UI")]
    public RawImage videoRawImage;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform originalParent;

    private bool isStopped;

    public DogState State { get; private set; }

    // ======================
    // INIT
    // ======================
    void Start()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalParent = transform.parent;

        State = DogState.Idle;

        SetPickupState(false); // chưa chạy thì chưa bắt được

        if (catchDogVideo != null)
        {
            catchDogVideo.playOnAwake = false;
            catchDogVideo.Stop();
        }

        if (videoRawImage != null)
            videoRawImage.gameObject.SetActive(false);

        StartCoroutine(StartAfterDelay());
    }

    IEnumerator StartAfterDelay()
    {
        yield return new WaitForSeconds(startDelay);
        StartRun();
    }

    void StartRun()
    {
        if (State == DogState.Finished) return;

        State = DogState.Running;
        isStopped = false;

        SetPickupState(true); // 🖐 cho phép bắt chó

        if (animator != null)
            animator.SetBool(WalkHash, true);
    }

    // ======================
    // UPDATE MOVE
    // ======================
    void Update()
    {
        if (State != DogState.Running || isStopped) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPoint.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.05f)
        {
            ReachTarget();
        }
    }

    // ======================
    // END POINT
    // ======================
    void ReachTarget()
    {
        if (State == DogState.Finished) return;

        Debug.Log("🐶 Dog reached EndDogPoint");

        State = DogState.Finished;
        isStopped = true;

        SetPickupState(false); // ❌ không cho bắt nữa

        if (animator != null)
            animator.SetBool(WalkHash, false);

        PauseGameplay();

        if (catchDogVideo != null && videoRawImage != null)
        {
            videoRawImage.gameObject.SetActive(true);

            catchDogVideo.loopPointReached -= OnVideoFinished;
            catchDogVideo.loopPointReached += OnVideoFinished;

            catchDogVideo.Stop();
            catchDogVideo.Play();
        }
        else
        {
            GameManager.Instance.GameOver2();
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        vp.loopPointReached -= OnVideoFinished;

        if (videoRawImage != null)
            videoRawImage.gameObject.SetActive(false);

        ResumeGameplay();
        GameManager.Instance.GameOver2();
    }

    // ======================
    // PLAYER INTERACT
    // ======================
    public void CatchDog(Transform bowlPoint)
    {
        if (State != DogState.Running) return;

        Debug.Log("🖐 Player caught dog");

        isStopped = true;
        State = DogState.Carried;

        if (animator != null)
            animator.SetBool(WalkHash, false);

        SetPickupState(false); // ❌ tắt collider + vật lý

        transform.SetParent(bowlPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void PlaceDog()
    {
        if (State != DogState.Carried) return;

        Debug.Log("📍 Dog placed");

        State = DogState.Resetting;
        StartCoroutine(ResetRoutine());
    }

    IEnumerator ResetRoutine()
    {
        transform.SetParent(originalParent);
        transform.position = originalPosition;
        transform.rotation = originalRotation;

        yield return null;

        State = DogState.Idle;
        StartCoroutine(StartAfterDelay());
    }

    // ======================
    // UTILS
    // ======================
    void PauseGameplay()
    {
        Time.timeScale = 0f;
    }

    void ResumeGameplay()
    {
        Time.timeScale = 1f;
    }

    void SetPickupState(bool canPickup)
    {
        if (rb != null)
        {
            rb.isKinematic = canPickup;
            rb.useGravity = false;
        }

        if (pickupCollider != null)
        {
            pickupCollider.enabled = canPickup;
            pickupCollider.isTrigger = true;
        }
    }
}
