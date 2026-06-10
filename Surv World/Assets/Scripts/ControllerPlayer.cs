using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("=== Movement ===")]
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float acceleration = 25f;
    public float turnSmoothTime = 0.08f;

    [Header("=== Jump & Gravity ===")]
    public float jumpHeight = 2.8f;
    public float gravity = 32f;
    public float fallMultiplier = 2.5f;        // am mărit puțin pentru un feeling și mai bun

    [Header("=== Ground Check ===")]
    public Transform groundCheck;
    public float groundRadius = 0.25f;
    public LayerMask groundMask;

    [Header("=== Input ===")]
    public InputActionAsset inputAsset;

    [Header("=== Animator ===")]
    public Animator animator;

    // Input references
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    // Cache
    private CharacterController cc;
    private Transform cam;
    private float turnSmoothVelocity;
    private Vector3 velocity;
    private bool isGrounded;
    private bool canJump = true;           // ←←← Nou: controlează saltul doar o dată

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        cam = Camera.main.transform;

        moveAction = inputAsset.FindAction("Player/Move");
        jumpAction = inputAsset.FindAction("Player/Jump");
        sprintAction = inputAsset.FindAction("Player/Sprint");

        moveAction.Enable();
        jumpAction.Enable();
        sprintAction.Enable();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundMask);

        // Resetăm canJump când atingem pământul
        if (isGrounded)
        {
            canJump = true;
            if (velocity.y < 0)
                velocity.y = -2f;
        }

        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        bool isSprinting = sprintAction.IsPressed();

        float targetSpeed = isSprinting ? runSpeed : walkSpeed;

        Vector3 inputDir = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        float currentSpeed = 0f;

        if (inputDir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            transform.rotation = Quaternion.Euler(0, smoothAngle, 0);

            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            Vector3 targetVelocity = moveDir * targetSpeed;

            Vector3 currentHorizontal = new Vector3(cc.velocity.x, 0, cc.velocity.z);
            Vector3 smoothedHorizontal = Vector3.MoveTowards(currentHorizontal, targetVelocity, acceleration * Time.deltaTime);

            velocity.x = smoothedHorizontal.x;
            velocity.z = smoothedHorizontal.z;

            currentSpeed = isSprinting ? 1f : 0.5f;
        }
        else
        {
            velocity.x = 0;
            velocity.z = 0;
        }

        // ====================== JUMP - DOAR O SINGURĂ DATĂ ======================
        if (jumpAction.triggered && isGrounded && canJump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * 2f * gravity);
            animator.SetTrigger("Jump");
            canJump = false;               // ← Blochează saltul următor până revii pe pământ
        }

        // ====================== GRAVITAȚIE REALISTĂ ======================
        if (velocity.y < 0)
        {
            velocity.y -= gravity * fallMultiplier * Time.deltaTime;
        }
        else
        {
            velocity.y -= gravity * Time.deltaTime;
        }

        // ====================== ANIMAȚII ======================
        animator.SetFloat("Speed", currentSpeed, 0.15f, Time.deltaTime);
        animator.SetBool("IsGrounded", isGrounded);

        cc.Move(velocity * Time.deltaTime);
    }

    private void OnEnable()
    {
        moveAction?.Enable();
        jumpAction?.Enable();
        sprintAction?.Enable();
    }

    private void OnDisable()
    {
        moveAction?.Disable();
        jumpAction?.Disable();
        sprintAction?.Disable();
    }
}