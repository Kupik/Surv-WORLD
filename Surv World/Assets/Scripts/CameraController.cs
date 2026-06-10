using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float mouseSensitivity = 180f;
    public float distance = 5.5f;
    public float heightOffset = 1.8f;
    public Vector2 pitchMinMax = new Vector2(-35, 65);

    [Header("=== Input ===")]
    public InputActionAsset inputAsset;        // ←←← Trage același PlayerControls.inputactions aici

    private InputAction lookAction;
    private float yaw;
    private float pitch;

    private void Awake()
    {
        lookAction = inputAsset.FindAction("Player/Look");
        lookAction.Enable();
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        yaw += lookInput.x * mouseSensitivity * Time.deltaTime;
        pitch -= lookInput.y * mouseSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPosition = target.position + rotation * new Vector3(0, heightOffset, -distance);

        transform.position = desiredPosition;
        transform.LookAt(target.position + Vector3.up * heightOffset);
    }

    private void OnEnable() => lookAction?.Enable();
    private void OnDisable() => lookAction?.Disable();
}