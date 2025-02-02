using UnityEngine;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera virtualCamera;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float verticalSpeed = 3f;

    private bool isRightMouseHeld = false;
    private float yaw;
    private float pitch;

    private void Start()
    {
        if (virtualCamera == null)
        {
            return;
        }

        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    private void Update()
    {
        HandleMouseInput();
        if (isRightMouseHeld)
        {
            RotateCamera();
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isRightMouseHeld = true;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isRightMouseHeld = false;
        }
    }

    private void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * verticalSpeed;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -30f, 70f); // Ограничиваем угол наклона

        transform.rotation = Quaternion.Euler(0, yaw, 0);
        virtualCamera.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
    }
}