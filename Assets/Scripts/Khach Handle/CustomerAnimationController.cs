using UnityEngine;
using UnityEngine.AI;

public class CustomerAnimationController : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        float speed = agent.velocity.magnitude;
        animator.SetFloat(SpeedHash, speed);
    }
}
