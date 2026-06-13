using UnityEngine;

public class PlayerLookAround : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;

    public Vector2 LockAxis;
    public float sensitivity = 40f;
    public float minPitch = -30f;
    public float maxPitch = 65f;

    private float pitch;

    void Update()
    {
        if (Time.timeScale == 0f)
            return;

        float mouseX = LockAxis.x * sensitivity * Time.deltaTime;
        float mouseY = LockAxis.y * sensitivity * Time.deltaTime;

        // Yaw
        transform.Rotate(Vector3.up * mouseX);

        // Pitch
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        cameraTransform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}
