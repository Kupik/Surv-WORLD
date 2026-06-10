using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Input")]
    public InputActionAsset inputAsset;
    private InputAction lookAction;

    [Header("Sensitivity")]
    public float mouseSensitivity = 25f;     // mai mare decât la third person
    public float pitchMinMax = 80f;          // sus/jos
    public Transform eyesTransform;    
    private float yaw;
    private float pitch;

    private void Awake()
    {
        lookAction = inputAsset.FindAction("Player/Look");
        lookAction.Enable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        yaw += lookInput.x * mouseSensitivity * Time.deltaTime;
        pitch -= lookInput.y * mouseSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, -pitchMinMax, pitchMinMax);

        // Rotește player-ul pe orizontală
        transform.rotation = Quaternion.Euler(0, yaw, 0);

        // Rotește capul/ochii pe verticală (dacă ai un Eyes child)
        if (eyesTransform != null)
            eyesTransform.localRotation = Quaternion.Euler(pitch, 0, 0);
    }

    private void OnEnable() => lookAction?.Enable();
    private void OnDisable() => lookAction?.Disable();
}
 