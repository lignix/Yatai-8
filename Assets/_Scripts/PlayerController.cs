using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    public static Transform InstanceTransform { get; private set; }
    public static PlayerController Instance { get; private set; }

    [Header("Player Movement")]
    public float moveSpeed = 4.0f;
    public float sprintSpeed = 6.0f;
    public float gravity = -15.0f;

    [Header("Camera & Look")]
    public Transform cameraTransform;
    public float lookSensitivity = 1.0f;
    public float sensitivityMultiplier = 10.0f;
    public float topClamp = 90.0f;
    public float bottomClamp = -90.0f;

    [Header("Noclip Mode (F1 to toggle)")]
    public bool enableNoclip = true;
    public float noclipSpeed = 10.0f;
    public float noclipFastMultiplier = 3.0f;
    public float noclipSmoothing = 5f;

    private CharacterController controller;
    private PlayerInput playerInput;
    private float verticalVelocity;
    private float cameraPitch = 0.0f;

    public bool isNoclipActive = false;
    private float noclipRotX = 0f;
    private float noclipRotY = 0f;
    private float currentNoclipRotX = 0f;
    private float currentNoclipRotY = 0f;

    private void Awake()
    {
        InstanceTransform = transform;
        Instance = this;

        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        if (playerInput != null && playerInput.actions != null)
        {
            playerInput.actions.Disable();
            playerInput.currentActionMap = playerInput.actions.FindActionMap("Player");
            if (playerInput.currentActionMap != null)
            {
                playerInput.currentActionMap.Enable();
            }
        }

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void OnEnable()
    {
        SettingsManager.OnSensitivityChanged += ApplySavedSensitivity;
        ApplySavedSensitivity(PlayerPrefs.GetFloat("SensitivityPref", 1f));

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        SettingsManager.OnSensitivityChanged -= ApplySavedSensitivity;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Start()
    {
        ApplySavedSensitivity(PlayerPrefs.GetFloat("SensitivityPref", 1f));
    }

    private void ApplySavedSensitivity(float newSensitivity)
    {
        lookSensitivity = newSensitivity;
    }

    private void Update()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (enableNoclip && Keyboard.current != null && Keyboard.current.f1Key.wasPressedThisFrame)
        {
            isNoclipActive = !isNoclipActive;
            verticalVelocity = 0f;
        }
#endif

        if (isNoclipActive)
        {
            HandleNoclipMovement();
        }
        else
        {
            HandleNormalMovement();
            HandleNormalLook();
        }
    }

    private void HandleNormalMovement()
    {
        bool isSprinting = playerInput != null && playerInput.actions != null && playerInput.actions["Sprint"].IsPressed();
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;

        Vector2 moveInput = Vector2.zero;
        if (playerInput != null && playerInput.actions != null)
        {
            InputAction moveAction = playerInput.actions["Move"];
            if (moveAction != null)
            {
                moveInput = moveAction.ReadValue<Vector2>();
            }
        }

        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
        moveDirection.Normalize();

        if (controller.isGrounded)
        {
            if (verticalVelocity < 0.0f) verticalVelocity = -2f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 finalMovement = moveDirection * currentSpeed + Vector3.up * verticalVelocity;
        controller.Move(finalMovement * Time.deltaTime);
    }

    private void HandleNormalLook()
    {
        Vector2 lookInput = Vector2.zero;
        if (playerInput != null && playerInput.actions != null)
        {
            InputAction lookAction = playerInput.actions["Look"];
            if (lookAction != null)
            {
                lookInput = lookAction.ReadValue<Vector2>();
            }
        }

        if (playerInput != null && playerInput.currentControlScheme == "Gamepad")
        {
            lookInput.y = -lookInput.y;
        }

        float finalSensitivity = lookSensitivity * sensitivityMultiplier * 0.1f;

        transform.Rotate(Vector3.up * lookInput.x * finalSensitivity);

        cameraPitch += lookInput.y * finalSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, bottomClamp, topClamp);

        if (cameraTransform != null)
        {
            cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0.0f, 0.0f);
        }
    }

    private void HandleNoclipMovement()
    {
        if (Mouse.current != null)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            noclipRotX += mouseDelta.x * lookSensitivity * 0.05f;
            noclipRotY -= mouseDelta.y * lookSensitivity * 0.05f;
            noclipRotY = Mathf.Clamp(noclipRotY, -90f, 90f);
        }

        currentNoclipRotX = Mathf.Lerp(currentNoclipRotX, noclipRotX, noclipSmoothing * Time.deltaTime);
        currentNoclipRotY = Mathf.Lerp(currentNoclipRotY, noclipRotY, noclipSmoothing * Time.deltaTime);

        transform.rotation = Quaternion.Euler(currentNoclipRotY, currentNoclipRotX, 0f);
        if (cameraTransform != null) cameraTransform.localRotation = Quaternion.identity;

        float speed = noclipSpeed;
        if (Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed) speed *= noclipFastMultiplier;

        Vector3 noclipDir = Vector3.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.zKey.isPressed) noclipDir += transform.forward;
            if (Keyboard.current.sKey.isPressed) noclipDir -= transform.forward;
            if (Keyboard.current.dKey.isPressed) noclipDir += transform.right;
            if (Keyboard.current.qKey.isPressed || Keyboard.current.aKey.isPressed) noclipDir -= transform.right;
            if (Keyboard.current.eKey.isPressed) noclipDir += Vector3.up;
            if (Keyboard.current.xKey.isPressed) noclipDir -= Vector3.up;
        }

        transform.position += noclipDir * speed * Time.deltaTime;
    }
}