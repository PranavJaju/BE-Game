using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.Collections;

public class PickupClass : MonoBehaviourPunCallbacks, IPunObservable

{
    [Header("Mobile Input Settings")]
    [SerializeField] public GameObject pickupButtonObj;
    [SerializeField] public GameObject throwButtonObj;
    [SerializeField] public GameObject mobileInputCanvas;

    private MobilePickupButton mobilePickupButton;
    private MobileThrowButton mobileThrowButton;
    private bool isMobilePlatform;

    [Header("Pickup Settings")]
    [SerializeField] private LayerMask PickupLayer;
    [SerializeField] private GameObject PlayerCamera;
    [SerializeField] private float PickupRange = 3f;
    [SerializeField] private Transform Hand;
    [SerializeField] private float ThrowingForce = 10f;

    [Header("VFX Settings")]
    [SerializeField] private GameObject HitVFXPrefab;
    private string vfxPath = "VFX/"; // Path within Resources folder

    [Header("Knockback Settings")]
    [SerializeField] private float KnockbackForce = 10f;
    [SerializeField] private float KnockbackUpwardForce = 2f;
    [SerializeField] private float KnockbackBackwardForce = 5f;
    [SerializeField] private float knockbackCooldown = 1f;

    [Header("Animation Settings")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private float pickupAnimationDuration = 1f;
    [SerializeField] private float throwAnimationDuration = 0.5f;
    [SerializeField] private float fallAnimationDuration = 1f;
    [SerializeField] private float transitionDuration = 0.1f;

    [Header("Debug Settings")]
    [SerializeField] private bool debugMode = false;

    // Animation Hash IDs
    private int pickupTriggerHash;
    private int throwTriggerHash;
    private int fallTriggerHash;
    private int idleTriggerHash;

    // Private Variables
    private Rigidbody CurrentObjectRigidBody;
    private Collider CurrentObjectCollider;
    private PhotonView photonView;
    private bool isThrown = false;
    private bool isAnimating = false;
    private float lastKnockbackTime;
    private bool isProcessingKnockback = false;

    [Header("Pickup Hint UI")]
     public GameObject pickupHintPanel;
     public Text pickupHintText;
    public Text pickupKeyText;

    // Existing private variables...
    private Collider lastDetectedPickupObject;

    #region Unity Methods

    void Start()
    {
        InitializeComponents();
        SetupAnimationHashes();
        ValidateSetup();
        SetupMobileControls();

    }


    private void SetupMobileControls()
    {
        isMobilePlatform = Application.platform == RuntimePlatform.Android ||
                          Application.platform == RuntimePlatform.IPhonePlayer;

        if (isMobilePlatform && photonView.IsMine)
        {
            if (mobileInputCanvas != null)
            {
                mobileInputCanvas.SetActive(true);
            }

            // Setup pickup button
            if (pickupButtonObj != null)
            {
                if (pickupButtonObj.GetComponent<MobilePickupButton>() == null)
                {
                    pickupButtonObj.AddComponent<MobilePickupButton>();
                }
                mobilePickupButton = pickupButtonObj.GetComponent<MobilePickupButton>();
            }
            else
            {
                Debug.LogError("Pickup Button is not assigned in " + gameObject.name);
            }

            // Setup throw button
            if (throwButtonObj != null)
            {
                if (throwButtonObj.GetComponent<MobileThrowButton>() == null)
                {
                    throwButtonObj.AddComponent<MobileThrowButton>();
                }
                mobileThrowButton = throwButtonObj.GetComponent<MobileThrowButton>();
            }
            else
            {
                Debug.LogError("Throw Button is not assigned in " + gameObject.name);
            }
        }
        else if (mobileInputCanvas != null)
        {
            mobileInputCanvas.SetActive(false);
        }
    }

    private void HandlePickupAndDrop()
    {
        if (isMobilePlatform)
        {
            // Mobile input handling
            if (mobilePickupButton != null && mobilePickupButton.IsPickupPressed())
            {
                if (!isAnimating && !isProcessingKnockback)
                {
                    if (!CurrentObjectRigidBody)
                    {
                        TryPickupObject();
                    }
                    else
                    {
                        photonView.RPC("DropObject", RpcTarget.All);
                    }
                }
                // Reset the button state after processing
                mobilePickupButton.OnPointerUp(null);
            }
        }
        else
        {
            // Existing keyboard input
            if (Input.GetKeyDown(KeyCode.E) && !isAnimating && !isProcessingKnockback)
            {
                if (!CurrentObjectRigidBody)
                {
                    TryPickupObject();
                }
                else
                {
                    photonView.RPC("DropObject", RpcTarget.All);
                }
            }
        }
    }

    private void HandleThrow()
    {
        if (isMobilePlatform)
        {
            // Mobile input handling
            if (mobileThrowButton != null && mobileThrowButton.IsThrowPressed())
            {
                if (CurrentObjectRigidBody && !isAnimating && !isProcessingKnockback)
                {
                    StartCoroutine(ThrowSequence());
                }
                // Reset the button state after processing
                mobileThrowButton.OnPointerUp(null);
            }
        }
        else
        {
            // Existing keyboard input
            if (Input.GetKeyDown(KeyCode.Q) && CurrentObjectRigidBody && !isAnimating && !isProcessingKnockback)
            {
                StartCoroutine(ThrowSequence());
            }
        }
    }

    
    private void Update()
    {
        if (!photonView.IsMine) return;

        HandlePickupAndDrop();
        HandleThrow();
        UpdateHeldObject();
        UpdatePickupHint();
    }

    // private void UpdatePickupHint()
    // {
    //     // Reuse the existing pickup logic to find nearby objects
    //     Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, PickupRange, PickupLayer);
    //     Collider closestObject = GetClosestObject(nearbyObjects);

    //     if (nearbyObjects.Length > 0)
    //     {
            
            
    //         if (closestObject != null)
    //         {
    //             // Show pickup hint
    //             if (pickupHintPanel != null)
    //             {
    //                 pickupHintPanel.SetActive(true);

    //                 // Set hint text with object name
    //                 if (pickupHintText != null)
    //                 {
    //                     pickupHintText.text = $"Pick up {closestObject.gameObject.name}";
    //                 }

    //                 // Set pickup key text
    //                 if (pickupKeyText != null)
    //                 {
    //                     pickupKeyText.text = isMobilePlatform ? "TAP" : "Press E";
    //                 }
    //             }
    //         }
    //     }
    //     else
    //     {
    //         // Hide pickup hint when no objects are nearby
    //         if (pickupHintPanel != null)
    //         {
    //             pickupHintPanel.SetActive(false);
    //         }
    //     }
    //     if (pickupHintPanel != null)
    //     {
    //         pickupHintPanel.SetActive(true);

    //         if (pickupHintText != null)
    //         {
    //             pickupHintText.text = $"Pick up {closestObject.gameObject.name}";
    //         }

    //         if (pickupKeyText != null)
    //         {
    //             pickupKeyText.text = isMobilePlatform ? "TAP" : "Press E";
    //         }
    //     }
    // }

    // // The existing TryPickupObject method remains unchanged
    // private void TryPickupObject()
    // {
    //     Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, PickupRange, PickupLayer);

    //     if (nearbyObjects.Length > 0)
    //     {
    //         Collider closestObject = GetClosestObject(nearbyObjects);
    //         if (closestObject != null)
    //         {
    //             PhotonView objectView = closestObject.gameObject.GetComponent<PhotonView>();
    //             if (objectView != null)
    //             {
    //                 StartCoroutine(PickupSequence(objectView.ViewID));
    //             }
    //         }
    //     }
    // }

private void UpdatePickupHint()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, PickupRange, PickupLayer);

        if (nearbyObjects.Length > 0)
        {
            Collider closestObject = GetClosestObject(nearbyObjects);
            
            if (closestObject != null)
            {
                // Store the closest object for use in TryPickupObject
                lastDetectedPickupObject = closestObject;

                // Show pickup hint
                if (pickupHintPanel != null)
                {
                    pickupHintPanel.SetActive(true);

                    if (pickupHintText != null)
                    {
                        pickupHintText.text = $"Pick up {closestObject.gameObject.name}";
                    }

                    if (pickupKeyText != null)
                    {
                        pickupKeyText.text = isMobilePlatform ? "TAP" : "Press E";
                    }
                }
            }
        }
        else
        {
            lastDetectedPickupObject = null;

            if (pickupHintPanel != null)
            {
                pickupHintPanel.SetActive(false);

                if (pickupHintText != null)
                {
                    pickupHintText.text = string.Empty; // Set text to empty
                }

                if (pickupKeyText != null)
                {
                    pickupKeyText.text = string.Empty; // Set text to empty
                }
            }
        }
    }

    private void TryPickupObject()
    {
        if (lastDetectedPickupObject != null)
        {
            PhotonView objectView = lastDetectedPickupObject.gameObject.GetComponent<PhotonView>();
            if (objectView != null)
            {
                StartCoroutine(PickupSequence(objectView.ViewID));

                if (pickupHintPanel != null)
                {
                    pickupHintPanel.SetActive(false);
                }
            }
        }
    }
    void OnDrawGizmosSelected()
    {
        if (debugMode)
        {
            // Draw pickup range sphere
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, PickupRange);
        }
    }

    #endregion

    #region Initialization Methods

    private void InitializeComponents()
    {
        photonView = GetComponent<PhotonView>();
        if (!playerAnimator)
        {
            playerAnimator = GetComponent<Animator>();
        }

        if (PlayerCamera == null)
        {
            PlayerCamera = Camera.main.gameObject;
        }

        lastKnockbackTime = -knockbackCooldown;
    }

    private void SetupAnimationHashes()
    {
        pickupTriggerHash = Animator.StringToHash("PickupTrigger");
        throwTriggerHash = Animator.StringToHash("ThrowTrigger");
        fallTriggerHash = Animator.StringToHash("fall");
        idleTriggerHash = Animator.StringToHash("idle");
    }

    private void ValidateSetup()
    {
        if (HitVFXPrefab == null)
        {
            Debug.LogError("HitVFXPrefab is not assigned in " + gameObject.name);
        }

        if (Hand == null)
        {
            Debug.LogError("Hand transform is not assigned in " + gameObject.name);
        }

        if (PickupLayer == 0)
        {
            Debug.LogWarning("PickupLayer is not set in " + gameObject.name);
        }
    }

    #endregion

    #region Input Handling

    // private void HandlePickupAndDrop()
    // {
    //     if (Input.GetKeyDown(KeyCode.E) && !isAnimating && !isProcessingKnockback)
    //     {
    //         if (!CurrentObjectRigidBody)
    //         {
    //             TryPickupObject();
    //         }
    //         else
    //         {
    //             photonView.RPC("DropObject", RpcTarget.All);
    //         }
    //     }
    // }

    // private void HandleThrow()
    // {
    //     if (Input.GetKeyDown(KeyCode.Q) && CurrentObjectRigidBody && !isAnimating && !isProcessingKnockback)
    //     {
    //         StartCoroutine(ThrowSequence());
    //     }
    // }

    private void UpdateHeldObject()
    {
        if (CurrentObjectRigidBody && !isThrown)
        {
            photonView.RPC("UpdateObjectPosition", RpcTarget.All);
        }
    }

    #endregion

    #region Pickup System


    // private void TryPickupObject()
    // {
    //     Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, PickupRange, PickupLayer);

    //     if (nearbyObjects.Length > 0)
    //     {
    //         Collider closestObject = GetClosestObject(nearbyObjects);
    //         if (closestObject != null)
    //         {
    //             PhotonView objectView = closestObject.gameObject.GetComponent<PhotonView>();
    //             if (objectView != null)
    //             {
    //                 StartCoroutine(PickupSequence(objectView.ViewID));
    //             }
    //         }
    //     }
    // }

    private Collider GetClosestObject(Collider[] objects)
    {
        Collider closestObject = null;
        float closestDistance = float.MaxValue;

        foreach (Collider obj in objects)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestObject = obj;
            }
        }

        return closestObject;
    }

    #endregion

    #region Animation Sequences

    private IEnumerator PickupSequence(int objectViewID)
    {
        isAnimating = true;
        photonView.RPC("TriggerPickupAnimation", RpcTarget.All);
        yield return new WaitForSeconds(pickupAnimationDuration);
        photonView.RPC("PickupObject", RpcTarget.All, objectViewID);
        isAnimating = false;
    }

    private IEnumerator ThrowSequence()
    {
        isAnimating = true;
        photonView.RPC("TriggerThrowAnimation", RpcTarget.All);
        yield return new WaitForSeconds(throwAnimationDuration * 0.5f);
        photonView.RPC("ThrowObject", RpcTarget.All, PlayerCamera.transform.forward);
        yield return new WaitForSeconds(throwAnimationDuration * 0.5f);
        isAnimating = false;
    }

    private IEnumerator RecoverFromFall()
    {
        yield return new WaitForSeconds(fallAnimationDuration);
        isProcessingKnockback = false;
        if (playerAnimator)
        {
            playerAnimator.SetTrigger(idleTriggerHash);
        }
    }

    private IEnumerator ClearObjectAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);
        CurrentObjectRigidBody = null;
        CurrentObjectCollider = null;
        isThrown = false;
    }

    private IEnumerator DestroyVFXAfterDelay(GameObject vfx)
    {
        yield return new WaitForSeconds(2f);
        if (vfx != null && photonView.IsMine)
        {
            PhotonNetwork.Destroy(vfx);
        }
    }

    #endregion

    #region PunRPC Methods

    [PunRPC]
    private void TriggerPickupAnimation()
    {
        if (playerAnimator)
        {
            playerAnimator.SetTrigger(pickupTriggerHash);
        }
    }

    [PunRPC]
    private void TriggerThrowAnimation()
    {
        if (playerAnimator)
        {
            playerAnimator.SetTrigger(throwTriggerHash);
        }
    }

    [PunRPC]
    private void TriggerFallAnimation()
    {
        if (playerAnimator && !isProcessingKnockback && Time.time >= lastKnockbackTime + knockbackCooldown)
        {
            isProcessingKnockback = true;
            lastKnockbackTime = Time.time;
            playerAnimator.SetTrigger(fallTriggerHash);
            StartCoroutine(RecoverFromFall());
        }
    }

    [PunRPC]
    private void PickupObject(int objectViewID)
    {
        GameObject pickupObject = PhotonView.Find(objectViewID).gameObject;
        CurrentObjectRigidBody = pickupObject.GetComponent<Rigidbody>();
        CurrentObjectCollider = pickupObject.GetComponent<Collider>();

        if (!CurrentObjectRigidBody.gameObject.GetComponent<ThrowableCollisionHandler>())
        {
            CurrentObjectRigidBody.gameObject.AddComponent<ThrowableCollisionHandler>().Initialize(this);
        }

        CurrentObjectRigidBody.isKinematic = true;
        CurrentObjectCollider.enabled = false;
        isThrown = false;

        if (debugMode)
        {
            Debug.Log($"Picked up object: {pickupObject.name}");
        }
    }

    [PunRPC]
    private void DropObject()
    {
        if (CurrentObjectRigidBody != null)
        {
            CurrentObjectRigidBody.isKinematic = false;
            CurrentObjectCollider.enabled = true;
            CurrentObjectRigidBody = null;
            CurrentObjectCollider = null;
            isThrown = false;

            if (debugMode)
            {
                Debug.Log("Object dropped");
            }
        }
    }

    [PunRPC]
    private void ThrowObject(Vector3 direction)
    {
        if (CurrentObjectRigidBody != null)
        {
            CurrentObjectRigidBody.isKinematic = false;
            CurrentObjectCollider.enabled = true;
            CurrentObjectRigidBody.AddForce(direction * ThrowingForce, ForceMode.Impulse);
            isThrown = true;
            StartCoroutine(ClearObjectAfterDelay());

            if (debugMode)
            {
                Debug.Log($"Object thrown with force: {ThrowingForce}");
            }
        }
    }

    [PunRPC]
    private void UpdateObjectPosition()
    {
        if (CurrentObjectRigidBody != null)
        {
            CurrentObjectRigidBody.MovePosition(Hand.position);
            CurrentObjectRigidBody.MoveRotation(Hand.rotation);
        }
    }

    [PunRPC]
    private void SpawnHitVFX(Vector3 hitPosition)
    {
        if (HitVFXPrefab != null)
        {
            // Local VFX instantiation
            GameObject vfx = Instantiate(HitVFXPrefab, hitPosition, Quaternion.identity);
            Destroy(vfx, 2f);

            if (debugMode)
            {
                Debug.Log($"VFX spawned at position: {hitPosition}");
            }
        }
    }

    [PunRPC]
    public void ApplyKnockback(int hitPlayerViewID, Vector3 hitDirection, Vector3 hitPosition)
    {
        PhotonView hitPlayerView = PhotonView.Find(hitPlayerViewID);
        if (hitPlayerView != null)
        {
            PickupClass hitPlayerPickup = hitPlayerView.gameObject.GetComponent<PickupClass>();
            if (hitPlayerPickup != null && !hitPlayerPickup.isProcessingKnockback)
            {
                hitPlayerPickup.photonView.RPC("TriggerFallAnimation", RpcTarget.All);
                hitPlayerPickup.photonView.RPC("SpawnHitVFX", RpcTarget.All, hitPosition);

                Rigidbody playerRb = hitPlayerView.gameObject.GetComponent<Rigidbody>();
                if (playerRb != null)
                {
                    Vector3 knockbackDirection = hitDirection + Vector3.up * KnockbackUpwardForce;
                    knockbackDirection -= hitPlayerView.transform.forward * KnockbackBackwardForce;
                    knockbackDirection.Normalize();
                    playerRb.AddForce(knockbackDirection * KnockbackForce, ForceMode.Impulse);

                    if (debugMode)
                    {
                        Debug.Log($"Knockback applied to player {hitPlayerViewID} with force: {KnockbackForce}");
                    }
                }
            }
        }
    }

    #endregion

    #region Network Synchronization

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Synchronize position, rotation, and states across the network
        if (stream.IsWriting)
        {
            // Send data to other clients
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(isThrown);
            stream.SendNext(isAnimating);
            stream.SendNext(isProcessingKnockback);

            // Send held object data
            bool hasHeldObject = CurrentObjectRigidBody != null;
            stream.SendNext(hasHeldObject);

            if (hasHeldObject)
            {
                // Send held object information
                stream.SendNext(CurrentObjectRigidBody.gameObject.GetComponent<PhotonView>().ViewID);
                stream.SendNext(CurrentObjectRigidBody.velocity);
                stream.SendNext(CurrentObjectRigidBody.angularVelocity);
                stream.SendNext(CurrentObjectRigidBody.position);
                stream.SendNext(CurrentObjectRigidBody.rotation);
                stream.SendNext(CurrentObjectRigidBody.isKinematic);
            }

            // Send animation states
            if (playerAnimator != null)
            {
                stream.SendNext(playerAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash);
                stream.SendNext(playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            }

            // Send knockback data
            stream.SendNext(lastKnockbackTime);
        }
        else
        {
            // Receive data from sender
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
            isThrown = (bool)stream.ReceiveNext();
            isAnimating = (bool)stream.ReceiveNext();
            isProcessingKnockback = (bool)stream.ReceiveNext();

            // Receive held object data
            bool hasHeldObject = (bool)stream.ReceiveNext();

            if (hasHeldObject)
            {
                // Get or update held object
                int objectViewID = (int)stream.ReceiveNext();
                PhotonView objectView = PhotonView.Find(objectViewID);

                if (objectView != null)
                {
                    CurrentObjectRigidBody = objectView.GetComponent<Rigidbody>();
                    if (CurrentObjectRigidBody != null)
                    {
                        // Update physics properties
                        CurrentObjectRigidBody.velocity = (Vector3)stream.ReceiveNext();
                        CurrentObjectRigidBody.angularVelocity = (Vector3)stream.ReceiveNext();
                        CurrentObjectRigidBody.position = (Vector3)stream.ReceiveNext();
                        CurrentObjectRigidBody.rotation = (Quaternion)stream.ReceiveNext();
                        CurrentObjectRigidBody.isKinematic = (bool)stream.ReceiveNext();

                        // Update collider reference
                        CurrentObjectCollider = objectView.GetComponent<Collider>();
                    }
                }
            }
            else if (CurrentObjectRigidBody != null)
            {
                // Clear references if no object is held
                CurrentObjectRigidBody = null;
                CurrentObjectCollider = null;
            }

            // Receive and update animation states
            if (playerAnimator != null)
            {
                int stateHash = (int)stream.ReceiveNext();
                float normalizedTime = (float)stream.ReceiveNext();

                // Only update animation if state is different
                AnimatorStateInfo currentState = playerAnimator.GetCurrentAnimatorStateInfo(0);
                if (currentState.fullPathHash != stateHash)
                {
                    playerAnimator.Play(stateHash, 0, normalizedTime);
                }
            }

            // Receive knockback data
            lastKnockbackTime = (float)stream.ReceiveNext();

            // Calculate and apply network lag compensation
            if (info.Sender != PhotonNetwork.LocalPlayer)
            {
                double lag = PhotonNetwork.Time - info.SentServerTime;
                if (lag > 0 && CurrentObjectRigidBody != null && !CurrentObjectRigidBody.isKinematic)
                {
                    // Extrapolate position based on velocities
                    Vector3 extrapolatedPosition = CurrentObjectRigidBody.position +
                        (CurrentObjectRigidBody.velocity * (float)lag);
                    CurrentObjectRigidBody.MovePosition(extrapolatedPosition);
                }
            }
        }

        // Debug logging
        if (debugMode)
        {
            if (stream.IsWriting)
            {
                Debug.Log($"Sending network data for player {photonView.ViewID}");
            }
            else
            {
                Debug.Log($"Receiving network data for player {photonView.ViewID}");
            }
        }
    }

    #endregion
}

public class ThrowableCollisionHandler : MonoBehaviour
{
    private PickupClass pickupClass;
    private float collisionCooldown = 0.5f;
    private float lastCollisionTime;

    public void Initialize(PickupClass pickup)
    {
        pickupClass = pickup;
        lastCollisionTime = -collisionCooldown;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time - lastCollisionTime < collisionCooldown) return;

        PhotonView hitPlayerView = collision.gameObject.GetComponent<PhotonView>();
        if (hitPlayerView != null && pickupClass != null)
        {
            Vector3 hitPoint = collision.contacts[0].point;
            Vector3 hitDirection = (hitPoint - transform.position).normalized;
            lastCollisionTime = Time.time;
            pickupClass.photonView.RPC("ApplyKnockback", RpcTarget.All, hitPlayerView.ViewID, hitDirection, hitPoint);
        }
    }

    

}


















// Rest of the existing code remains the same...
