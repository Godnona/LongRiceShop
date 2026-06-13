using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Input")]
    public FixedJoystick moveJoystick;

    [Header("Movement")]
    public float moveSpeed = 4f;

    private Rigidbody rb;
    private Vector3 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (Time.timeScale == 0f)
            return;

        ReadInput();
    }

    void FixedUpdate()
    {
        Move();
        CheckFall();
    }

    void ReadInput()
    {
        if (moveJoystick == null)
        {
            moveInput = Vector3.zero;
            return;
        }

        float h = moveJoystick.Horizontal;
        float v = moveJoystick.Vertical;

        moveInput = new Vector3(h, 0f, v);
        if (moveInput.sqrMagnitude > 1f)
            moveInput.Normalize();
    }

    void Move()
    {
        Vector3 moveDir = transform.right * moveInput.x + transform.forward * moveInput.z;

        Vector3 targetPos = rb.position + moveDir * moveSpeed * Time.fixedDeltaTime;

        rb.MovePosition(targetPos);
    }

    void CheckFall()
    {
        if (transform.position.y < -20f)
        {
            Vector3 pos = new Vector3(-0.19f, 1.06f, -3.25f);
            pos.y = 10f;
            transform.position = pos;
        }
    }
}

