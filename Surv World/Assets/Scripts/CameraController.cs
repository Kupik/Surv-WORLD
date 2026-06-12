using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
  
    [Header("Mișcare")]
    public float walkSpeed = 6f;
    public float runSpeed = 10f;
    public float jumpHeight = 2f;
    public float gravity = 20f;

    [Header("Uitare cu Mouse - Cinemachine")]
    public float mouseSensitivity = 2f;
    public CinemachineVirtualCamera virtualCamera;

    private CharacterController controller;
    private Vector3 velocity;
    private float verticalRotation = 0f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (virtualCamera == null)
            Debug.LogError("Asignează Cinemachine Virtual Camera în Inspector!");
    }

    private void Update()
    {
        HandleMovement();
        HandleMouseLook();
    }

    private void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move.Normalize();

        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        controller.Move(move * currentSpeed * Time.deltaTime);

        // Gravitație + Jump
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        if (Input.GetButtonDown("Jump") && controller.isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotire orizontală player
        transform.Rotate(Vector3.up * mouseX);

        // Rotire verticală prin Cinemachine
        verticalRotation += -mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -85f, 85f);

        if (virtualCamera != null)
        {
            virtualCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }
    }
}