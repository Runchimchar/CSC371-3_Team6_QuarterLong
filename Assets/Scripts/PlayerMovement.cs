using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using System;

public class PlayerMovement : MonoBehaviour
{
    [Header("Relations")]
    [Tooltip("Layers that the movement controller uses for ground check")]
    [SerializeField] LayerMask GroundLayer;
    [Tooltip("Layers that the player can aim at (gun can focus on such objects)")]
    [SerializeField] LayerMask aimColliderMask = new LayerMask();

    [Header("Movement")]
    [SerializeField] private float normalLookSensitivity, aimLookSensitivity;
    private float currentLookSensitivity;
    public float RotationSmoothTime = 0.12f;
    public float movementSpeed = 4f;
    public float maxSpeed = 50f;
    public float movementMultiplier = 10f;
    public float airMultiplier = 0.15f;

    [Header("Sprinting")]
    [SerializeField] float walkSpeed = 4f;
    [SerializeField] float sprintSpeed = 8f;
    [SerializeField] float acceleration = 10f;

    [Header("Drag")]
    public float groundDrag = 6f;
    public float airDrag = 0.5f;
    public float grappleDrag = 0.2f;

    [Header("Jumping")]
    public float jumpForce = 10f;
    public float jumpSecs = 0.2f;
    public float jumpCooldownSecs = 0.1f;

    GameObject CinemachineCameraTarget;
    GameObject player;
    Grapple grappleInstance;
    Transform orientation;

    Transform groundCheck;
    CinemachineVirtualCamera aimVirtualCamera;

    private Vector3 hitPoint = Vector3.zero;
    private RaycastHit hitPointHit;

    float mayJump = 0f;
    float jumpCooldown = 0f;
    float movementX, movementY;
    float lookX, lookY;
    float multiplier = 0.01f;
    float xRotation, yRotation;
    float playerHeight = 2f;
    float rotationVelocity;
    GameObject mainCamera;

    bool isGrounded;
    float groundDistance = 0.2f;
    float maxAimDistance = 999f;

    bool sprinting;
    bool rotateOnMove = true;
    bool aiming = false;
    bool grappling = false;
    bool yoinking = false;

    Vector3 moveDirection;
    Vector3 slopeMoveDirection;

    Rigidbody rb;
    RaycastHit slopeHit;

    // Event for player pressing interact key
    public event Action InteractEvent = delegate { };

    // Event for player pressing pause key
    public event Action PauseEvent = delegate { };

    private void Awake()
    {
        // get a reference to our main camera
        if (mainCamera == null)
        {
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        CinemachineCameraTarget = transform.Find("PlayerCameraRoot").gameObject;
        player = transform.Find("Capsule").gameObject;
        grappleInstance = GetComponent<Grapple>();
        orientation = transform.Find("Orientation");
        
        groundCheck = transform.Find("Capsule/GroundCheck");
        aimVirtualCamera = Array.Find(FindObjectsOfType<CinemachineVirtualCamera>(true), x => x.name.Equals("PlayerAimCamera"));

        SetLookSensitivity(normalLookSensitivity);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    void OnLook(InputValue lookValue)
    {
        Vector2 lookVector = lookValue.Get<Vector2>();

        lookX = lookVector.x;
        lookY = lookVector.y;

        yRotation += lookX * currentLookSensitivity * multiplier;
        xRotation += lookY * currentLookSensitivity * multiplier;

        xRotation = Mathf.Clamp(xRotation, -89.99f, 89.99f);
    }

    void OnJump(InputValue value)
    {
        if (value.isPressed) Jump();
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, GroundLayer, QueryTriggerInteraction.Ignore);
        if (isGrounded && jumpCooldown <= 0)
            mayJump = jumpSecs;
        else
        {
            mayJump -= Time.deltaTime;
            mayJump = Mathf.Clamp(mayJump, 0f, jumpSecs);
        }

        jumpCooldown -= Time.deltaTime;
        Mathf.Clamp(jumpCooldown, 0f, jumpCooldownSecs);
        
        ControlDrag();
        ControlSpeed();

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void LateUpdate()
    {
        RotatePlayer();
        AimAngles();
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    void AimAngles()
    {
        Vector3 mouseWorldPosition = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, maxAimDistance, aimColliderMask, QueryTriggerInteraction.Ignore))
        {
            hitPoint = raycastHit.point;
            hitPointHit = raycastHit;
        }
        else hitPoint = ray.GetPoint(maxAimDistance);
        if (aiming || grappling || yoinking)
        {
            if (aiming)
            {
                aimVirtualCamera.gameObject.SetActive(true);
                SetLookSensitivity(aimLookSensitivity);
            }
            else aimVirtualCamera.gameObject.SetActive(false);

            SetRotateOnMove(false);

            Vector3 worldAimTarget = hitPoint;
            worldAimTarget.y = player.transform.position.y;
            Vector3 aimDirection = (worldAimTarget - player.transform.position).normalized;

            player.transform.forward = Vector3.Lerp(player.transform.forward, aimDirection, Time.deltaTime * 20f);
        } else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            SetLookSensitivity(normalLookSensitivity);
            SetRotateOnMove(true);
        }
    }

    public RaycastHit GetHit()
    {
        return hitPointHit;
    }

    public Vector3 GetAimPoint()
    {
        return hitPoint;
    }

    void OnInteract(InputValue value)
    {
        if (value.isPressed)
        {
            // Notifies all listeners that interact key was pressed
            InteractEvent();
        }
    }

    void OnPause(InputValue value) {
        if (value.isPressed) {
            // Notifies all listeners that pause key was pressed
            PauseEvent();
        }
    }
    void OnRespawn(InputValue value) {
        if (value.isPressed) {
            // Notifies all listeners that pause key was pressed
            RespawnController.instance.Respawn();
        }
    }
    void OnSpawnNext(InputValue value) {
        if (value.isPressed) {
            // Notifies all listeners that key was pressed
            RespawnController.instance.SpawnNext();
        }
    }
    void OnSpawnPrev(InputValue value) {
        if (value.isPressed) {
            // Notifies all listeners that key was pressed
            RespawnController.instance.SpawnPrev();
        }
    }

    void Jump()
    {
        if (mayJump > 0f)
        {
            mayJump = 0f;
            jumpCooldown = jumpCooldownSecs;
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void GrappleJump(Vector3 norm)
    {
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce((transform.up + (orientation.forward / 2 + norm.normalized / 2)) * jumpForce, ForceMode.Impulse);
    }

    void ControlDrag()
    {
        if (isGrounded)
            rb.drag = groundDrag;
        else if (grappling)
            rb.drag = grappleDrag;
        else
            rb.drag = airDrag;
    }

    void OnSprint(InputValue value)
    {
        sprinting = value.isPressed;
    }

    void ControlSpeed()
    {
        if (sprinting && isGrounded)
            movementSpeed = Mathf.Lerp(movementSpeed, sprintSpeed, acceleration * Time.deltaTime);
        else
            movementSpeed = Mathf.Lerp(movementSpeed, walkSpeed, acceleration * Time.deltaTime);
    }

    void MovePlayer()
    {
        bool onSlope = OnSlope();
        moveDirection = orientation.forward * movementY + orientation.right * movementX;

        if (isGrounded && !onSlope)
            rb.AddForce(moveDirection.normalized * movementSpeed * movementMultiplier, ForceMode.Acceleration);
        else if (isGrounded && onSlope)
            rb.AddForce(slopeMoveDirection.normalized * movementSpeed * movementMultiplier, ForceMode.Acceleration);
        else
            rb.AddForce(moveDirection.normalized * movementSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);

        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
    }

    void RotatePlayer()
    {
        if (movementX != 0 || movementY != 0)
        {
            Vector3 inputDirection = new Vector3(movementX, 0.0f, movementY).normalized;
            float targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(player.transform.eulerAngles.y, targetRotation, ref rotationVelocity, RotationSmoothTime);

            if (rotateOnMove)
            {
                // rotate to face input direction relative to camera position
                player.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
        }
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.5f))
        {
            if (slopeHit.normal != Vector3.up)
                return true;
            return false;
        }
        return false;
    }

    public bool GetIsGrounded()
    {
        return isGrounded;
    }

    public bool GetAiming()
    {
        return aiming;
    }

    public void SetLookSensitivity(float newLookSensitivity)
    {
        currentLookSensitivity = newLookSensitivity;
    }

    public void SetRotateOnMove(bool newRotateOnMove)
    {
        rotateOnMove = newRotateOnMove;
    }

    public void SetGrapple(bool newGrapple)
    {
        grappling = newGrapple;
    }

    public void SetYoink(bool newYoink)
    {
        yoinking = newYoink;
    }

    public void SetAiming(bool newAim)
    {
        aiming = newAim;
    }

    public void RespawnAt(Vector3 location, Vector2 lookRotation)
    {
        grappleInstance.StopGrapple();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = location;
        player.transform.rotation = Quaternion.Euler(lookRotation.x, lookRotation.y, 0);
        xRotation = lookRotation.x;
        yRotation = lookRotation.y;
    }

    public LayerMask GetAimColliderMask()
    {
        return aimColliderMask;
    }
}