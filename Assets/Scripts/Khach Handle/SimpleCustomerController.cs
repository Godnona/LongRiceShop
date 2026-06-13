using UnityEngine;

public class SimpleCustomerController : MonoBehaviour
{
    private const string TRANSITION_BOOL_NAME = "isWalking";
    [Header("Start Point")]
    public Transform pointA;
    [Header("End Point")]
    public Transform pointB;
    public float moveSpeed = 2f;
    public float stopDistance = 0.2f;

    private Animator animator;
    private bool movingToB = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        // Optional: start at point A, if start at current position, comment out the line below
        // transform.position = pointA.position; 
    }

    void Update()
    {
        if (movingToB)
        {
            MoveTowards(pointB.position);
        }
    }

    void MoveTowards(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        float distance = direction.magnitude;

        if (distance > stopDistance)
        {
            // Move towards target
            transform.position += direction.normalized * moveSpeed * Time.deltaTime;
            transform.forward = direction.normalized;

            // Set walking animation
            animator.SetBool(TRANSITION_BOOL_NAME, true);
        }
        else
        {
            // Reached target
            animator.SetBool(TRANSITION_BOOL_NAME, false);
            movingToB = false;
            OnArrived();
        }
    }

    void OnArrived()
    {
        Debug.Log("Arrived at point B");

        // TODO: something when arrived at point B
    }
}
