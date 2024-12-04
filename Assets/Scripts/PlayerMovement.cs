// // // // using System.Collections;
// // // // using System.Collections.Generic;
// // // // using UnityEngine;

// // // // [RequireComponent(typeof(CharacterController))]
// // // // public class PlayerMovement : MonoBehaviour
// // // // {
// // // //     public Camera playerCamera;
// // // //     public float walkSpeed = 6f;
// // // //     public float runSpeed = 12f;
// // // //     public float jumpPower = 10f;
// // // //     public float gravity = 10f;
// // // //     public float lookSpeed = 2f;
// // // //     public float lookXLimit = 45f;
// // // //     public float defaultHeight = 2f;
// // // //     public float crouchHeight = 1f;
// // // //     public float crouchSpeed = 3f;
// // // //     public float rotationSpeed = 1f;

// // // //     private Vector3 moveDirection = Vector3.zero;
// // // //     private float rotationX = 0;
// // // //     private CharacterController characterController;
// // // //     private Animator animator;

// // // //     private bool canMove = true;

// // // //     private float inputx, inputz;

// // // //     void Start()
// // // //     {
// // // //         characterController = GetComponent<CharacterController>();
// // // //         animator = GetComponent<Animator>();
// // // //         Cursor.lockState = CursorLockMode.Locked;
// // // //         Cursor.visible = false;
// // // //     }

// // // //     void Update()
// // // //     {
// // // //         inputx = Input.GetAxis("Horizontal");
// // // //         inputz = Input.GetAxis("Vertical");
// // // //         Vector3 forward = transform.TransformDirection(Vector3.forward);
// // // //         Vector3 right = transform.TransformDirection(Vector3.right);
// // // //         animator.SetFloat("vertical", Input.GetAxis("Vertical"));
// // // //         animator.SetFloat("horizontal", 0);
// // // //         bool isRunning = Input.GetKey(KeyCode.LeftShift);
// // // //         float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
// // // //         float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
// // // //         float movementDirectionY = moveDirection.y;
// // // //         moveDirection = (forward * curSpeedX) + (right * curSpeedY);

// // // //         if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
// // // //         {
// // // //             moveDirection.y = jumpPower;
// // // //             // animator.SetBool("isJump", true);
// // // //         }
// // // //         else
// // // //         {
// // // //             moveDirection.y = movementDirectionY;
// // // //         }

// // // //         if (!characterController.isGrounded)
// // // //         {
// // // //             moveDirection.y -= gravity * Time.deltaTime;
// // // //             // animator.SetBool("isJump", true);
// // // //         }
// // // //         if (characterController.isGrounded)
// // // //         {
// // // //             animator.SetBool("isJump", false);
// // // //         }

// // // //         if (Input.GetKey(KeyCode.R) && canMove)
// // // //         {
// // // //             characterController.height = crouchHeight;
// // // //             walkSpeed = crouchSpeed;
// // // //             runSpeed = crouchSpeed;

// // // //         }
// // // //         else
// // // //         {
// // // //             characterController.height = defaultHeight;
// // // //             walkSpeed = 6f;
// // // //             runSpeed = 12f;
// // // //         }

// // // //         characterController.Move(moveDirection * Time.deltaTime);

// // // //         if (canMove)
// // // //         {
// // // //             rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
// // // //             rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
// // // //             playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
// // // //             //  if (Input.GetKey(KeyCode.LeftArrow))
// // // //             // {
// // // //             //     transform.Rotate(0, -rotationSpeed, 0);
// // // //             // }
// // // //             // else if (Input.GetKey(KeyCode.RightArrow))
// // // //             // {
// // // //             //     transform.Rotate(0, rotationSpeed, 0);
// // // //             // }
// // // //             transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
// // // //         }
// // // //     }

// // // //     void FixedUpdate()
// // // //     {
// // // //         characterController.transform.Rotate(Vector3.up * inputx * (100f * Time.deltaTime));
// // // //     }
// // // // }

// // // using System.Collections;
// // // using System.Collections.Generic;
// // // using UnityEngine;
// // // using Photon.Pun;

// // // [RequireComponent(typeof(CharacterController))]
// // // [RequireComponent(typeof(PhotonAnimatorView))]
// // // public class PlayerMovement : MonoBehaviourPunCallbacks, IPunObservable
// // // {
// // //     public Camera playerCamera;
// // //     public float walkSpeed = 6f;
// // //     public float runSpeed = 12f;
// // //     public float jumpPower = 10f;
// // //     public float gravity = 10f;
// // //     public float lookSpeed = 2f;
// // //     public float lookXLimit = 45f;
// // //     public float defaultHeight = 2f;
// // //     public float crouchHeight = 1f;
// // //     public float crouchSpeed = 3f;
// // //     public float rotationSpeed = 1f;

// // //     private Vector3 moveDirection = Vector3.zero;
// // //     private float rotationX = 0;
// // //     private CharacterController characterController;
// // //     private Animator animator;
// // //     private PhotonAnimatorView photonAnimatorView;

// // //     private bool canMove = true;
// // //     private float inputx, inputz;
// // //     private bool isJumping = false;
// // //     private float verticalVelocity = 0f;

// // //     // Networking variables
// // //     private Vector3 networkPosition;
// // //     private Quaternion networkRotation;
// // //     private float lagDistance = 10f;

// // //     void Start()
// // //     {
// // //         characterController = GetComponent<CharacterController>();
// // //         animator = GetComponent<Animator>();
// // //         photonAnimatorView = GetComponent<PhotonAnimatorView>();

// // //         // Configure the animator's root motion
// // //         if (animator != null)
// // //         {
// // //             animator.applyRootMotion = false;  // This is crucial - disable root motion
// // //         }

// // //         // Only enable camera and cursor lock for the local player
// // //         if (photonView.IsMine)
// // //         {
// // //             playerCamera.gameObject.SetActive(true);
// // //             Cursor.lockState = CursorLockMode.Locked;
// // //             Cursor.visible = false;
// // //         }
// // //         else
// // //         {
// // //             if (playerCamera != null)
// // //                 playerCamera.gameObject.SetActive(false);
// // //         }

// // //         // Initialize network variables
// // //         networkPosition = transform.position;
// // //         networkRotation = transform.rotation;
// // //     }

// // //     void Update()
// // //     {
// // //         if (photonView.IsMine)
// // //         {
// // //             HandleMovementInput();
// // //             HandleAnimations();
// // //         }
// // //         else
// // //         {
// // //             UpdateRemotePlayer();
// // //         }
// // //     }

// // //     void HandleMovementInput()
// // //     {
// // //         inputx = Input.GetAxis("Horizontal");
// // //         inputz = Input.GetAxis("Vertical");

// // //         Vector3 forward = transform.TransformDirection(Vector3.forward);
// // //         Vector3 right = transform.TransformDirection(Vector3.right);

// // //         bool isRunning = Input.GetKey(KeyCode.LeftShift);
// // //         float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * inputz : 0;
// // //         float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * inputx : 0;

// // //         // Calculate horizontal movement
// // //         moveDirection = (forward * curSpeedX) + (right * curSpeedY);

// // //         // Handle jumping and gravity
// // //         if (characterController.isGrounded)
// // //         {
// // //             // Reset vertical velocity and jumping state when grounded
// // //             verticalVelocity = -0.5f; // Small downward force to maintain grounding
// // //             isJumping = false;

// // //             // Only allow jumping when grounded and space is pressed
// // //             if (Input.GetButtonDown("Jump") && canMove)
// // //             {
// // //                 verticalVelocity = jumpPower;
// // //                 isJumping = true;
// // //             }
// // //         }
// // //         else
// // //         {
// // //             // Apply gravity when in air
// // //             verticalVelocity -= gravity * Time.deltaTime;
// // //         }

// // //         // Apply vertical movement
// // //         moveDirection.y = verticalVelocity;

// // //         // Handle crouching
// // //         if (Input.GetKey(KeyCode.R) && canMove)
// // //         {
// // //             characterController.height = crouchHeight;
// // //             walkSpeed = crouchSpeed;
// // //             runSpeed = crouchSpeed;
// // //         }
// // //         else
// // //         {
// // //             characterController.height = defaultHeight;
// // //             walkSpeed = 6f;
// // //             runSpeed = 12f;
// // //         }

// // //         // Apply movement
// // //         characterController.Move(moveDirection * Time.deltaTime);

// // //         // Handle camera rotation
// // //         if (canMove)
// // //         {
// // //             rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
// // //             rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
// // //             playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
// // //             transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
// // //         }
// // //     }

// // //     void HandleAnimations()
// // //     {
// // //         // Set animation parameters
// // //         animator.SetFloat("vertical", inputz);
// // //         animator.SetFloat("horizontal", inputx);
// // //         animator.SetBool("isJump", isJumping);
// // //     }

// // //     void UpdateRemotePlayer()
// // //     {
// // //         float distance = Vector3.Distance(transform.position, networkPosition);
// // //         if (distance > lagDistance)
// // //         {
// // //             transform.position = networkPosition;
// // //         }
// // //         else
// // //         {
// // //             transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10f);
// // //         }

// // //         transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 10f);
// // //     }

// // //     void FixedUpdate()
// // //     {
// // //         if (photonView.IsMine)
// // //         {
// // //             characterController.transform.Rotate(Vector3.up * inputx * (100f * Time.deltaTime));
// // //         }
// // //     }

// // //     public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
// // //     {
// // //         if (stream.IsWriting)
// // //         {
// // //             stream.SendNext(transform.position);
// // //             stream.SendNext(transform.rotation);
// // //         }
// // //         else
// // //         {
// // //             networkPosition = (Vector3)stream.ReceiveNext();
// // //             networkRotation = (Quaternion)stream.ReceiveNext();
// // //         }
// // //     }
// // // }
// // using System.Collections;
// // using System.Collections.Generic;
// // using UnityEngine;
// // using UnityEngine.UI;
// // using Photon.Pun;
// // using UnityEngine.EventSystems;

// // [RequireComponent(typeof(CharacterController))]
// // [RequireComponent(typeof(PhotonAnimatorView))]
// // public class PlayerMovement : MonoBehaviourPunCallbacks, IPunObservable
// // {
// //     public Camera playerCamera;
// //     public float walkSpeed = 6f;
// //     public float runSpeed = 12f;
// //     public float jumpPower = 10f;
// //     public float gravity = 10f;
// //     public float lookSpeed = 2f;
// //     public float lookXLimit = 45f;
// //     public float defaultHeight = 2f;
// //     public float crouchHeight = 1f;
// //     public float crouchSpeed = 3f;
// //     public float rotationSpeed = 1f;

// //     private Vector3 moveDirection = Vector3.zero;
// //     private float rotationX = 0;
// //     private CharacterController characterController;
// //     private Animator animator;
// //     private PhotonAnimatorView photonAnimatorView;
// //     private float currentSpeed;
// //     private bool canMove = true;
// //     private float inputx, inputz;
// //     private bool isJumping = false;
// //     private float verticalVelocity = 0f;

// //     // Networking variables
// //     private Vector3 networkPosition;
// //     private Quaternion networkRotation;
// //     private float lagDistance = 10f;

// //     public GameObject mobileControlsCanvas; 
// //     public Button jumpButton;              
// //     public Joystick moveJoystick;     

// //     void Start()
// // {
// //     if (jumpButton == null)
// //     {
// //         Debug.LogError("JUMP is not assigned.");
// //     }
// //     else
// //     {
// //         Debug.Log("JUMP is assigned correctly.");
// //     }


// //     characterController = GetComponent<CharacterController>();
// //     animator = GetComponent<Animator>();
// //     photonAnimatorView = GetComponent<PhotonAnimatorView>();

// //     // Configure the animator's root motion
// //     if (animator != null)
// //     {
// //         animator.applyRootMotion = false;
// //     }

// //     // Only enable camera and cursor lock for the local player
// //     if (photonView.IsMine)
// //     {
// //         playerCamera.gameObject.SetActive(true);

// //         // Only lock cursor on non-mobile platforms
// //         if (!(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer))
// //         {
// //             Cursor.lockState = CursorLockMode.Locked;
// //             Cursor.visible = false;
// //         }

// //     }
// //     else
// //     {
// //         if (playerCamera != null)
// //             playerCamera.gameObject.SetActive(false);
// //     }

// //     // Initialize network variables
// //     networkPosition = transform.position;
// //     networkRotation = transform.rotation;

// //     // Show/hide mobile controls based on platform
// //     if (mobileControlsCanvas != null)
// //     {
// //         if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
// //         {
// //             mobileControlsCanvas.SetActive(true);
// //         }
// //         else
// //         {
// //             mobileControlsCanvas.SetActive(false);
// //         }
// //     }
// // }
// //     void Update()
// //     {

// //         if (photonView.IsMine)
// //         {
// //             if (Application.platform == RuntimePlatform.Android)
// //             {
// //                 HandleMobileInput();
// //             }
// //             else
// //             {
// //                 HandleKeyboardInput();
// //             }
// //             HandleAnimations();
// //         }
// //         else
// //         {
// //             UpdateRemotePlayer();
// //         }

// //     }

// //    void HandleMobileInput()
// // {
// //     if (moveJoystick != null)
// //     {
// //         inputx = moveJoystick.Horizontal;
// //         inputz = moveJoystick.Vertical;

// //         // Print joystick data for debugging
// //         Debug.Log("Joystick Horizontal: " + inputx);
// //         Debug.Log("Joystick Vertical: " + inputz);

// //         // Only move if there's significant input to avoid accidental movements
// //         if (Mathf.Abs(inputx) > 0.1f || Mathf.Abs(inputz) > 0.1f)
// //         {
// //             moveDirection = new Vector3(inputx, 0, inputz); // Vertical velocity should not affect X and Z movement
// //             moveDirection = transform.TransformDirection(moveDirection);

// //             // Print moveDirection for debugging
// //             Debug.Log("Move Direction: " + moveDirection);
// //             currentSpeed = (inputz > 0 && moveJoystick.Vertical > 0.5f) ? runSpeed : walkSpeed; // Adjust speed for running
// //             characterController.Move(moveDirection * Time.deltaTime * currentSpeed);
// //         }
// //         else
// //         {
// //             // Reset moveDirection if input is minimal
// //             moveDirection = Vector3.zero;
// //         }
// //     }

// //     // Ensure button interactions do not conflict with swipes or other gestures
// //     if (EventSystem.current.IsPointerOverGameObject())
// //     {
// //         Debug.Log("Pointer over UI, skipping movement.");
// //         return; // Skip input if over a UI element
// //     }
// // }

// //     void HandleKeyboardInput()
// // {
// //     inputx = Input.GetAxis("Horizontal");
// //     inputz = Input.GetAxis("Vertical");

// //     // Handle jumping and gravity
// //     if (characterController.isGrounded)
// //     {
// //         // Reset vertical velocity and jumping state when grounded
// //         verticalVelocity = -0.5f; // Small downward force to maintain grounding
// //         isJumping = false;

// //         // Only allow jumping when grounded and space is pressed
// //         if (Input.GetButtonDown("Jump") && canMove)
// //         {
// //             verticalVelocity = jumpPower;
// //             isJumping = true;
// //         }
// //     }
// //     else
// //     {
// //         // Apply gravity when in air
// //         verticalVelocity -= gravity * Time.deltaTime;
// //     }

// //     // Construct the movement direction vector
// //     moveDirection = new Vector3(inputx, verticalVelocity, inputz);

// //     // Transform the movement direction from local to world space
// //     moveDirection = transform.TransformDirection(moveDirection);

// //     // Apply movement
// //     characterController.Move(moveDirection * Time.deltaTime * (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed));

// //     Handle camera rotation
// //     if (canMove)
// //     {
// //         rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
// //         rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
// //         playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
// //         transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
// //     }
// // }

// //     void HandleAnimations()
// //     {
// //         // Set animation parameters
// //         animator.SetFloat("vertical", inputz);
// //         animator.SetFloat("horizontal", inputx);
// //         animator.SetBool("isJump", isJumping);
// //     }

// //     void UpdateRemotePlayer()
// //     {
// //         float distance = Vector3.Distance(transform.position, networkPosition);
// //         if (distance > lagDistance)
// //         {
// //             transform.position = networkPosition;
// //         }
// //         else
// //         {
// //             transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * 10f);
// //         }

// //         transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, Time.deltaTime * 10f);
// //     }

// //     void FixedUpdate()
// //     {
// //         if (photonView.IsMine)
// //         {
// //             characterController.transform.Rotate(Vector3.up * inputx * (100f * Time.deltaTime));
// //         }
// //     }

// //     public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
// //     {
// //         if (stream.IsWriting)
// //         {
// //             stream.SendNext(transform.position);
// //             stream.SendNext(transform.rotation);
// //         }
// //         else
// //         {
// //             networkPosition = (Vector3)stream.ReceiveNext();
// //             networkRotation = (Quaternion)stream.ReceiveNext();
// //         }
// //     }


// // public void TriggerJump()
// // {
// //     //
// // }
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

    void HandleMobileInput()
    {
        if (moveJoystick != null)
        {
            // Capture joystick input
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

    void Update()
    {
        if (photonView.IsMine)
        {
            if (Application.platform == RuntimePlatform.Android ||
                Application.platform == RuntimePlatform.IPhonePlayer)
            {
                HandleMobileInput();
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
            // Debug.Log("Respawn key pressed");
            // Debug.Log("Is local player: " + isLocalPlayer);

            // Always use RoomManager's instance to respawn
            if (RoomManager.instance != null)
            {
                // Debug.Log("Calling RoomManager respawn");
                RoomManager.instance.RespawnPlayer();
            }
            else
            {
                Debug.LogError("RoomManager instance is null");
            }
        }
    }


    // void HandleMobileInput()
    // {
    //     if (moveJoystick != null)
    //     {
    //         inputx = moveJoystick.Horizontal;
    //         inputz = moveJoystick.Vertical;

    //         // Only move if there's significant input to avoid accidental movements
    //         if (Mathf.Abs(inputx) > 0.1f || Mathf.Abs(inputz) > 0.1f)
    //         {
    //             moveDirection = new Vector3(inputx, 0, inputz);
    //             moveDirection = transform.TransformDirection(moveDirection);
    //             currentSpeed = (inputz > 0 && moveJoystick.Vertical > 0.5f) ? runSpeed : walkSpeed;
    //         }
    //         else
    //         {
    //             moveDirection.x = 0;
    //             moveDirection.z = 0;
    //         }
    //     }

    //     // Handle jump logic
        // HandleJump();


    //     // Apply final movement
    //     moveDirection.y = verticalVelocity;
    //     characterController.Move(moveDirection * Time.deltaTime * currentSpeed);

    //     // Ensure button interactions do not conflict with swipes or other gestures
    //     if (EventSystem.current.IsPointerOverGameObject())
    //     {
    //         return;
    //     }
    // }

    void HandleKeyboardInput()
    {
        inputx = Input.GetAxis("Horizontal");
        inputz = Input.GetAxis("Vertical");

        moveDirection = new Vector3(inputx, 0, inputz);
        moveDirection = transform.TransformDirection(moveDirection);
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

        // Handle camera rotation
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
            animator.SetFloat("vertical", inputz);
            animator.SetFloat("horizontal", inputx);
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
        if (photonView.IsMine)
        {
            characterController.transform.Rotate(Vector3.up * inputx * (100f * Time.deltaTime));
        }
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
