using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PhotonAnimatorView))]
public class PlayerMovement : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("References")]
    public Camera playerCamera;
    public GameObject mobileControlsCanvas;
    public Button jumpButton;
    public Joystick moveJoystick;

    [Header("Movement Settings")]
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float rotationSpeed = 1f;

    [Header("Jump Settings")]
    public float jumpPower = 10f;
    public float gravity = 10f;
    public float coyoteTime = 0.2f;
    public float jumpBufferTime = 0.2f;

    [Header("Camera Settings")]
    public float lookSpeed = 2f;
    public float lookXLimit = 45f;

    [Header("Crouch Settings")]
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;

    // Private movement variables
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private CharacterController characterController;
    private Animator animator;
    private PhotonAnimatorView photonAnimatorView;
    private float currentSpeed;
    private bool canMove = true;
    private float inputx, inputz;

    // Jump-related variables
    private bool isJumping = false;
    private float verticalVelocity = 0f;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool jumpInputReleased = true;
    private bool jumpRequested = false;

    // Networking variables
    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private float lagDistance = 10f;

    public bool isLocalPlayer;

    private Vector2 joystickInputVector;
    private float joystickMagnitude;

    // void HandleMobileInput()
    // {
    //     if (moveJoystick != null)
    //     {
    //         // Capture joystick input
    //         inputx = moveJoystick.Horizontal;
    //         inputz = moveJoystick.Vertical;

    //         // Calculate joystick magnitude more precisely
    //         joystickInputVector = new Vector2(0, inputz);
    //         joystickMagnitude = joystickInputVector.magnitude;

    //         // Improved input handling with smoother transitions
    //         if (joystickMagnitude > 0.05f) // Smaller, more responsive deadzone
    //         {
    //             // Normalize input to ensure consistent direction
    //             Vector3 normalizedDirection = new Vector3(
    //                 joystickInputVector.normalized.x,
    //                 0,
    //                 joystickInputVector.normalized.y
    //             );

    //             // Transform direction to world space
    //             moveDirection = transform.TransformDirection(normalizedDirection);

    //             // Smooth speed transition based on joystick magnitude
    //             currentSpeed = Mathf.Lerp(walkSpeed, runSpeed,
    //                 Mathf.Clamp01(joystickMagnitude * 1.5f)); // Adjust multiplier as needed
    //         }
    //         else
    //         {
    //             // Zero out movement when joystick is near center
    //             moveDirection = Vector3.zero;
    //             currentSpeed = 0f;
    //         }

    //         // Handle jump with new button script
    //         HandleMobileJump();

    //         // Apply movement
    //         moveDirection.y = verticalVelocity;
    //         characterController.Move(moveDirection * Time.deltaTime * currentSpeed);
    //     }
    // }

    void HandleMobileInput()
{
    if (moveJoystick != null)
    {
        // Capture both horizontal and vertical joystick input
        inputx = moveJoystick.Horizontal;
        inputz = moveJoystick.Vertical;

        // Calculate joystick magnitude more precisely
        joystickInputVector = new Vector2(inputx, inputz);
        joystickMagnitude = joystickInputVector.magnitude;

        // Improved input handling with smoother transitions
        if (joystickMagnitude > 0.05f) // Smaller, more responsive deadzone
        {
            // Normalize input to ensure consistent direction
            Vector3 normalizedDirection = new Vector3(
                joystickInputVector.normalized.x,
                0,
                joystickInputVector.normalized.y
            );

            // Transform direction to world space
            moveDirection = transform.TransformDirection(normalizedDirection);

            // Smooth speed transition based on joystick magnitude
            currentSpeed = Mathf.Lerp(walkSpeed, runSpeed,
                Mathf.Clamp01(joystickMagnitude * 1.5f)); // Adjust multiplier as needed
        }
        else
        {
            // Zero out movement when joystick is near center
            moveDirection = Vector3.zero;
            currentSpeed = 0f;
        }

        // Handle jump with new button script
        HandleMobileJump();

        // Apply movement
        moveDirection.y = verticalVelocity;
        characterController.Move(moveDirection * Time.deltaTime * currentSpeed);
    }
}

    void HandleMobileJump()
    {
        // Check for jump button using the new MobileJumpButton script
        MobileJumpButton mobileJumpButton = jumpButton.GetComponent<MobileJumpButton>();

        if (mobileJumpButton != null)
        {
            // Update coyote time
            if (characterController.isGrounded)
            {
                coyoteTimeCounter = coyoteTime;
                isJumping = false;
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
            }

            // Jump input detection
            if (mobileJumpButton.IsJumpPressed())
            {
                // Trigger jump if conditions are met
                if (coyoteTimeCounter > 0f && !isJumping && canMove)
                {
                    verticalVelocity = jumpPower;
                    isJumping = true;
                    coyoteTimeCounter = 0f;

                    // Trigger jump animation
                    if (animator != null)
                    {
                        animator.SetBool("isJump", true);
                    }
                }
            }
            else
            {
                // Apply gravity
                if (characterController.isGrounded && verticalVelocity < 0)
                {
                    verticalVelocity = -0.5f; // Small downward force when grounded
                }
                else
                {
                    verticalVelocity -= gravity * Time.deltaTime;
                }

                // Update animation state
                if (animator != null)
                {
                    animator.SetBool("isJump", !characterController.isGrounded);
                }
            }
        }
    }

    void Start()
    {
        if (jumpButton == null)
        {
            Debug.LogError("JUMP is not assigned.");
        }
        else
        {
            Debug.Log("JUMP is assigned correctly.");
        }

        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        photonAnimatorView = GetComponent<PhotonAnimatorView>();

        // Configure the animator's root motion
        if (animator != null)
        {
            animator.applyRootMotion = false;
        }

        // Only enable camera and cursor lock for the local player
        if (photonView.IsMine)
        {
            playerCamera.gameObject.SetActive(true);

            // Only lock cursor on non-mobile platforms
            if (!(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        else
        {
            if (playerCamera != null)
                playerCamera.gameObject.SetActive(false);
        }

        // Initialize network variables
        networkPosition = transform.position;
        networkRotation = transform.rotation;

        // Show/hide mobile controls based on platform
        if (mobileControlsCanvas != null)
        {
            mobileControlsCanvas.SetActive(Application.platform == RuntimePlatform.Android ||
                                         Application.platform == RuntimePlatform.IPhonePlayer);
        }
        if (jumpButton != null && jumpButton.GetComponent<MobileJumpButton>() == null)
        {
            jumpButton.gameObject.AddComponent<MobileJumpButton>();
        }
    }

    // void Update()
    // {
    //     if (photonView.IsMine)
    //     {
    //         if (Application.platform == RuntimePlatform.Android ||
    //             Application.platform == RuntimePlatform.IPhonePlayer)
    //         {
    //             HandleMobileInput();
    //         }
    //         else
    //         {
    //             HandleKeyboardInput();
    //         }
    //         HandleAnimations();
    //     }
    //     else
    //     {
    //         UpdateRemotePlayer();
    //     }

    //     // Debug respawn key
    //     if (Input.GetKeyDown(KeyCode.X))
    //     {
    //         // Always use RoomManager's instance to respawn
    //         if (RoomManager.instance != null)
    //         {
    //             RoomManager.instance.RespawnPlayer();
    //         }
    //         else
    //         {
    //             Debug.LogError("RoomManager instance is null");
    //         }
    //     }
    // }

    void Update()
{
    if (photonView.IsMine)
    {
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            HandleMobileInput();
            HandleMobileCameraRotation(); // New method for mobile camera rotation
        }
        else
        {
            HandleKeyboardInput();
        }
        HandleAnimations();
    }
    else
    {
        UpdateRemotePlayer();
    }

    // Debug respawn key
    if (Input.GetKeyDown(KeyCode.X))
    {
        if (RoomManager.instance != null)
        {
            RoomManager.instance.RespawnPlayer();
        }
        else
        {
            Debug.LogError("RoomManager instance is null");
        }
    }
}

// void HandleMobileCameraRotation()
// {
//     // Check if there are any touches
//     if (Input.touchCount > 0)
//     {
//         // Get the first touch
//         Touch touch = Input.GetTouch(0);

//         // Ignore touch on UI elements
//         if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
//         {
//             // Check if this is not the jump button touch
//             if (jumpButton != null && 
//                 EventSystem.current.currentSelectedGameObject != jumpButton.gameObject)
//             {
//                 // Camera rotation logic
//                 if (touch.phase == TouchPhase.Moved)
//                 {
//                     // Rotate horizontally based on touch delta
//                     float rotationY = touch.deltaPosition.x * lookSpeed * 0.1f;
//                     transform.rotation *= Quaternion.Euler(0, rotationY, 0);

//                     // Vertical camera rotation
//                     rotationX += -touch.deltaPosition.y * lookSpeed * 0.1f;
//                     rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
//                     playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
//                 }
//             }
//         }
//     }
// }

// bool isInteractingWithUI = false;

// void HandleMobileCameraRotation()
// {
//     // Check if there are any touches
//     if (Input.touchCount > 0)
//     {
//         // Get the first touch
//         Touch touch = Input.GetTouch(0);

//         // Check if the touch is on a UI element
//         if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
//         {
//             // Set the UI interaction flag
//             isInteractingWithUI = true;
//         }
//         else
//         {
//             // Reset the UI interaction flag
//             isInteractingWithUI = false;

//             // Camera rotation logic
//             if (touch.phase == TouchPhase.Moved)
//             {
//                 // Rotate horizontally based on touch delta
//                 float rotationY = touch.deltaPosition.x * lookSpeed * 0.1f;
//                 transform.rotation *= Quaternion.Euler(0, rotationY, 0);

//                 // Vertical camera rotation
//                 rotationX += -touch.deltaPosition.y * lookSpeed * 0.1f;
//                 rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
//                 playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
//             }
//         }
//     }
//     else
//     {
//         // Reset the UI interaction flag
//         isInteractingWithUI = false;
//     }
// }
bool isInteractingWithUI = false;
bool isJoystickDragging = false;

// void HandleMobileCameraRotation()
// {
//     // Check if there are any touches
//      Touch touch = Input.GetTouch(0);
//     if (Input.touchCount > 0)
//     {
//         // Get the first touch
       

//         // Check if the touch is on a UI element
//         if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
//         {
//             // Check if the touch is on the joystick
//             if (moveJoystick != null && EventSystem.current.currentSelectedGameObject == moveJoystick.gameObject)
//             {
//                 // Set the joystick dragging flag
//                 isJoystickDragging = true;
//             }
//             else
//             {
//                 // Set the UI interaction flag
//                 isInteractingWithUI = true;
//             }
//         }
//         else
//         {
//             // Reset the UI and joystick interaction flags
//             isInteractingWithUI = false;
//             isJoystickDragging = false;

//             // Camera rotation logic
//             if (touch.phase == TouchPhase.Moved)
//             {
//                 // Rotate horizontally based on touch delta
//                 float rotationY = touch.deltaPosition.x * lookSpeed * 0.1f;
//                 transform.rotation *= Quaternion.Euler(0, rotationY, 0);

//                 // Vertical camera rotation
//                 rotationX += -touch.deltaPosition.y * lookSpeed * 0.1f;
//                 rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
//                 playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
//             }
//         }
//     }
//     else
//     {
//         // Reset the UI and joystick interaction flags
//         isInteractingWithUI = false;
//         isJoystickDragging = false;
//     }

//     // Only allow camera rotation if the user is not interacting with UI or joystick
//     bool canRotateCamera = !isInteractingWithUI && !isJoystickDragging;
//     if (canRotateCamera)
//     {
//         // Apply camera rotation
//         // Rotate horizontally based on touch delta
//         float rotationY = touch.deltaPosition.x * lookSpeed * 0.1f;
//         transform.rotation *= Quaternion.Euler(0, rotationY, 0);

//         // Vertical camera rotation
//         rotationX += -touch.deltaPosition.y * lookSpeed * 0.1f;
//         rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
//         playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
//     }
// }
// void HandleMobileCameraRotation()
// {
//     // Check if there are any touches
//     if (Input.touchCount > 0)
//     {
//         Touch touch = Input.GetTouch(0);

//         // Check if the touch is on the right half of the screen
//         bool isTouchOnRightHalf = touch.position.x > Screen.width / 2f;

//         // Check if the touch is on a UI element
//         if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
//         {
//             // Check if the touch is on the joystick
//             if (moveJoystick != null && EventSystem.current.currentSelectedGameObject == moveJoystick.gameObject)
//             {
//                 // Set the joystick dragging flag
//                 isJoystickDragging = true;
//             }
//             else
//             {
//                 // Set the UI interaction flag
//                 isInteractingWithUI = true;
//             }
//         }
//         else
//         {
//             // Reset the UI and joystick interaction flags
//             isInteractingWithUI = false;
//             isJoystickDragging = false;

//             // Camera rotation logic - only work on right half of screen
//             if (touch.phase == TouchPhase.Moved && isTouchOnRightHalf)
//             {
//                 // Rotate horizontally based on touch delta
//                 float rotationY = touch.deltaPosition.x * lookSpeed * 0.1f;
//                 transform.rotation *= Quaternion.Euler(0, rotationY, 0);

//                 // Vertical camera rotation
//                 rotationX += -touch.deltaPosition.y * lookSpeed * 0.1f;
//                 rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
//                 playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
//             }
//         }
//     }
//     else
//     {
//         // Reset the UI and joystick interaction flags
//         isInteractingWithUI = false;
//         isJoystickDragging = false;
//     }
// }


void HandleMobileCameraRotation()
{
    // Check if there are any touches
    if (Input.touchCount > 0)
    {
        Touch touch = Input.GetTouch(0);

        // Explicitly check if the touch is on the right half of the screen
        bool isTouchOnRightHalf = touch.position.x > Screen.width / 2f;

        // Check if the touch is on a UI element
        if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
        {
            // Check if the touch is on the joystick
            if (moveJoystick != null && EventSystem.current.currentSelectedGameObject == moveJoystick.gameObject)
            {
                // Set the joystick dragging flag
                isJoystickDragging = true;
            }
            else
            {
                // Set the UI interaction flag
                isInteractingWithUI = true;
            }
        }
        else
        {
            // Reset the UI and joystick interaction flags
            isInteractingWithUI = false;
            isJoystickDragging = false;

            // Camera rotation logic - ONLY allow vertical rotation on right half of screen
            if (touch.phase == TouchPhase.Moved && isTouchOnRightHalf)
            {
                // Horizontal rotation (full screen)
                float rotationY = touch.deltaPosition.x * lookSpeed * 0.1f;
                transform.rotation *= Quaternion.Euler(0, rotationY, 0);

                // Vertical camera rotation - ONLY on right half
                rotationX += -touch.deltaPosition.y * lookSpeed * 0.1f;
                rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
                playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            }
        }
    }
    else
    {
        // Reset the UI and joystick interaction flags
        isInteractingWithUI = false;
        isJoystickDragging = false;
    }
}
    void HandleKeyboardInput()
    {
        inputx = Input.GetAxis("Horizontal");
        inputz = Input.GetAxis("Vertical");

        // Create a movement vector that includes horizontal movement
        moveDirection = new Vector3(inputx, 0, inputz);
        moveDirection = transform.TransformDirection(moveDirection).normalized;

        currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        // Handle jump logic
        if (Input.GetButtonDown("Jump"))
        {
            jumpRequested = true;
        }

        HandleJump();

        // Handle crouching
        if (Input.GetKey(KeyCode.R) && canMove)
        {
            characterController.height = crouchHeight;
            currentSpeed = crouchSpeed;
        }
        else
        {
            characterController.height = defaultHeight;
        }

        // Apply final movement
        moveDirection.y = verticalVelocity;
        characterController.Move(moveDirection * Time.deltaTime * currentSpeed);

        // Camera rotation handling
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    private void HandleJump()
    {
        // Update coyote time
        if (characterController.isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            isJumping = false;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Update jump buffer
        if (jumpRequested)
        {
            jumpBufferCounter = jumpBufferTime;
            jumpRequested = false;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Handle jump release
        bool jumpReleased = false;
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            // For mobile, check if the pointer is up
            jumpReleased = Input.GetMouseButtonUp(0) && EventSystem.current.currentSelectedGameObject == jumpButton.gameObject;
        }
        else
        {
            jumpReleased = Input.GetButtonUp("Jump");
        }

        if (jumpReleased)
        {
            jumpInputReleased = true;
            if (verticalVelocity > 0)
            {
                verticalVelocity *= 0.5f; // Cut jump height if released early
            }
        }

        // Trigger jump if conditions are met
        if (jumpBufferCounter > 0f && coyoteTimeCounter > 0f && !isJumping && jumpInputReleased && canMove)
        {
            verticalVelocity = jumpPower;
            isJumping = true;
            jumpInputReleased = false;
            jumpBufferCounter = 0f;

            // Trigger jump animation
            if (animator != null)
            {
                animator.SetBool("isJump", true);
            }
        }
        else
        {
            // Apply gravity
            if (characterController.isGrounded && verticalVelocity < 0)
            {
                verticalVelocity = -0.5f; // Small downward force when grounded
            }
            else
            {
                verticalVelocity -= gravity * Time.deltaTime;
            }

            // Update animation state
            if (animator != null)
            {
                animator.SetBool("isJump", !characterController.isGrounded);
            }
        }
    }

    void HandleAnimations()
    {
        if (animator != null)
        {
            // Set vertical movement
            animator.SetFloat("vertical", inputz);

            // Set horizontal movement
            animator.SetFloat("horizontal", inputx);

            // Trigger side walk animation when horizontal input is significant
            bool isSideWalking = Mathf.Abs(inputx) > 0.1f;
            // animator.SetBool("isSideWalk", isSideWalking);

            // New side walk direction booleans
            Debug.Log("inputx: " + inputx);
            animator.SetBool("isSideWalk", inputx < 0); // Left arrow key triggers side walk
            animator.SetBool("isSideRight", inputx > 0); // Right arrow key triggers side right

            // Jump animation
            animator.SetBool("isJump", isJumping);
        }
    }

    void UpdateRemotePlayer()
    {
        float distance = Vector3.Distance(transform.position, networkPosition);
        if (distance > lagDistance)
        {
            transform.position = networkPosition;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10f);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 10f);
    }

    void FixedUpdate()
    {
        // Removed rotation based on horizontal input
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }

    public void TriggerJump()
    {
        if (photonView.IsMine)
        {
            jumpRequested = true;
            jumpInputReleased = false;
        }
    }
}