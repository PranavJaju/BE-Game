// // // // // // // // using UnityEngine;
// // // // // // // // using Photon.Pun;
// // // // // // // // using System.Collections;

// // // // // // // // public class PickupClass : MonoBehaviourPunCallbacks
// // // // // // // // {
// // // // // // // //     [Header("Pickup Settings")]
// // // // // // // //     [SerializeField] private LayerMask PickupLayer;
// // // // // // // //     [SerializeField] private GameObject PlayerCamera;
// // // // // // // //     [SerializeField] private float PickupRange = 3f;
// // // // // // // //     [SerializeField] private Transform Hand;
// // // // // // // //     [SerializeField] private float ThrowingForce = 10f;
// // // // // // // //     [SerializeField] private float KnockbackForce = 10f;
// // // // // // // //     [SerializeField] private float KnockbackUpwardForce = 2f;

// // // // // // // //     [Header("Animation")]
// // // // // // // //     [SerializeField] private PickupAnimationController animationController;

// // // // // // // //     private Rigidbody CurrentObjectRigidBody;
// // // // // // // //     private Collider CurrentObjectCollider;
// // // // // // // //     private PhotonView photonView;
// // // // // // // //     private bool isThrown = false;

// // // // // // // //     public event System.Action OnObjectPickup;
// // // // // // // //     public event System.Action OnObjectDrop;
// // // // // // // //     public event System.Action OnObjectThrow;

// // // // // // // //     void Start()
// // // // // // // //     {
// // // // // // // //         photonView = GetComponent<PhotonView>();

// // // // // // // //         if (animationController == null)
// // // // // // // //         {
// // // // // // // //             animationController = GetComponent<PickupAnimationController>();
// // // // // // // //         }

// // // // // // // //         // Ensure we have all required components
// // // // // // // //         if (PlayerCamera == null)
// // // // // // // //         {
// // // // // // // //             PlayerCamera = Camera.main.gameObject;
// // // // // // // //         }
// // // // // // // //     }

// // // // // // // //     void Update()
// // // // // // // //     {
// // // // // // // //         if (!photonView.IsMine) return;

// // // // // // // //         // Pickup/Drop Logic
// // // // // // // //         if (Input.GetKeyDown(KeyCode.E))
// // // // // // // //         {
// // // // // // // //             Ray Pickupray = new Ray(PlayerCamera.transform.position, PlayerCamera.transform.forward);
// // // // // // // //             if (Physics.Raycast(Pickupray, out RaycastHit hitInfo, PickupRange, PickupLayer))
// // // // // // // //             {
// // // // // // // //                 PhotonView objectView = hitInfo.collider.gameObject.GetComponent<PhotonView>();
// // // // // // // //                 if (objectView != null)
// // // // // // // //                 {
// // // // // // // //                     if (CurrentObjectRigidBody)
// // // // // // // //                     {
// // // // // // // //                         photonView.RPC("DropAndPickup", RpcTarget.All, objectView.ViewID);
// // // // // // // //                     }
// // // // // // // //                     else
// // // // // // // //                     {
// // // // // // // //                         photonView.RPC("Pickup", RpcTarget.All, objectView.ViewID);
// // // // // // // //                     }
// // // // // // // //                 }
// // // // // // // //                 return;
// // // // // // // //             }
// // // // // // // //             if (CurrentObjectRigidBody)
// // // // // // // //             {
// // // // // // // //                 photonView.RPC("Drop", RpcTarget.All);
// // // // // // // //             }
// // // // // // // //         }

// // // // // // // //         // Throw Logic
// // // // // // // //         if (Input.GetKeyDown(KeyCode.Q) && CurrentObjectRigidBody)
// // // // // // // //         {
// // // // // // // //             photonView.RPC("Throw", RpcTarget.All, PlayerCamera.transform.forward);
// // // // // // // //         }

// // // // // // // //         // Update held object position
// // // // // // // //         if (CurrentObjectRigidBody && !isThrown)
// // // // // // // //         {
// // // // // // // //             UpdateObjectPosition();
// // // // // // // //         }
// // // // // // // //     }

// // // // // // // //     private void UpdateObjectPosition()
// // // // // // // //     {
// // // // // // // //         if (photonView.IsMine)
// // // // // // // //         {
// // // // // // // //             CurrentObjectRigidBody.MovePosition(Hand.position);
// // // // // // // //             CurrentObjectRigidBody.MoveRotation(Hand.rotation);
// // // // // // // //         }
// // // // // // // //     }

// // // // // // // //     [PunRPC]
// // // // // // // //     private void Pickup(int objectViewID)
// // // // // // // //     {
// // // // // // // //         GameObject pickupObject = PhotonView.Find(objectViewID).gameObject;
// // // // // // // //         CurrentObjectRigidBody = pickupObject.GetComponent<Rigidbody>();
// // // // // // // //         CurrentObjectCollider = pickupObject.GetComponent<Collider>();

// // // // // // // //         CurrentObjectRigidBody.isKinematic = true;
// // // // // // // //         CurrentObjectCollider.enabled = false;
// // // // // // // //         isThrown = false;

// // // // // // // //         if (animationController != null)
// // // // // // // //         {
// // // // // // // //             animationController.HandlePickupAnimation();
// // // // // // // //         }

// // // // // // // //         OnObjectPickup?.Invoke();
// // // // // // // //     }

// // // // // // // //     [PunRPC]
// // // // // // // //     private void Drop()
// // // // // // // //     {
// // // // // // // //         if (CurrentObjectRigidBody != null)
// // // // // // // //         {
// // // // // // // //             CurrentObjectRigidBody.isKinematic = false;
// // // // // // // //             CurrentObjectCollider.enabled = true;
// // // // // // // //             CurrentObjectRigidBody = null;
// // // // // // // //             CurrentObjectCollider = null;
// // // // // // // //             isThrown = false;

// // // // // // // //             if (animationController != null)
// // // // // // // //             {
// // // // // // // //                 animationController.HandleDropAnimation();
// // // // // // // //             }

// // // // // // // //             OnObjectDrop?.Invoke();
// // // // // // // //         }
// // // // // // // //     }

// // // // // // // //     [PunRPC]
// // // // // // // //     private void Throw(Vector3 direction)
// // // // // // // //     {
// // // // // // // //         if (CurrentObjectRigidBody != null)
// // // // // // // //         {
// // // // // // // //             CurrentObjectRigidBody.isKinematic = false;
// // // // // // // //             CurrentObjectCollider.enabled = true;
// // // // // // // //             CurrentObjectRigidBody.AddForce(direction * ThrowingForce, ForceMode.Impulse);
// // // // // // // //             isThrown = true;

// // // // // // // //             if (animationController != null)
// // // // // // // //             {
// // // // // // // //                 animationController.HandleThrowAnimation();
// // // // // // // //             }

// // // // // // // //             OnObjectThrow?.Invoke();
// // // // // // // //             StartCoroutine(ClearObjectAfterDelay());
// // // // // // // //         }
// // // // // // // //     }

// // // // // // // //     private IEnumerator ClearObjectAfterDelay()
// // // // // // // //     {
// // // // // // // //         yield return new WaitForSeconds(0.1f);
// // // // // // // //         CurrentObjectRigidBody = null;
// // // // // // // //         CurrentObjectCollider = null;
// // // // // // // //         isThrown = false;
// // // // // // // //     }
// // // // // // // // }


// // // // // // // using System.Collections;
// // // // // // // using System.Collections.Generic;
// // // // // // // using UnityEngine;
// // // // // // // using Photon.Pun;

// // // // // // // public class PickupClass : MonoBehaviourPunCallbacks
// // // // // // // {
// // // // // // //     [Header("Pickup Settings")]
// // // // // // //     [SerializeField] private LayerMask PickupLayer;
// // // // // // //     [SerializeField] private GameObject PlayerCamera;
// // // // // // //     [SerializeField] private float PickupRange;
// // // // // // //     [SerializeField] private Transform Hand;
// // // // // // //     [SerializeField] private float ThrowingForce;

// // // // // // //     [Header("Knockback Settings")]
// // // // // // //     [SerializeField] private float KnockbackForce = 10f;
// // // // // // //     [SerializeField] private float KnockbackUpwardForce = 2f;

// // // // // // //     [Header("Animation Settings")]
// // // // // // //     [SerializeField] private Animator playerAnimator;
// // // // // // //     [SerializeField] private float pickupAnimationDuration = 1f;
// // // // // // //     [SerializeField] private float throwAnimationDuration = 0.5f;
// // // // // // //     [SerializeField] private float transitionDuration = 0.1f;

// // // // // // //     [Header("Animation State Names")]
// // // // // // //     [SerializeField] private string pickupStateName = "pickup";
// // // // // // //     [SerializeField] private string throwStateName = "throw";
// // // // // // //     [SerializeField] private string fallStateName = "fall";
// // // // // // //     [SerializeField] private string idleStateName = "idle";

// // // // // // //     private int pickupStateHash;
// // // // // // //     private int throwStateHash;
// // // // // // //     private int fallStateHash;
// // // // // // //     private int idleStateHash;

// // // // // // //     private Rigidbody CurrentObjectRigidBody;
// // // // // // //     private Collider CurrentObjectCollider;
// // // // // // //     private PhotonView photonView;
// // // // // // //     private bool isThrown = false;
// // // // // // //     private bool isAnimating = false;

// // // // // // //     void Start()
// // // // // // //     {
// // // // // // //         photonView = GetComponent<PhotonView>();
// // // // // // //         if (!playerAnimator)
// // // // // // //         {
// // // // // // //             playerAnimator = GetComponent<Animator>();
// // // // // // //         }

// // // // // // //         // Cache animation state hashes
// // // // // // //         pickupStateHash = Animator.StringToHash(pickupStateName);
// // // // // // //         throwStateHash = Animator.StringToHash(throwStateName);
// // // // // // //         fallStateHash = Animator.StringToHash(fallStateName);
// // // // // // //         idleStateHash = Animator.StringToHash(idleStateName);
// // // // // // //     }

// // // // // // //     void Update()
// // // // // // //     {
// // // // // // //         if (!photonView.IsMine) return;

// // // // // // //         if (Input.GetKeyDown(KeyCode.E) && !isAnimating)
// // // // // // //         {
// // // // // // //             Ray Pickupray = new Ray(PlayerCamera.transform.position, PlayerCamera.transform.forward);
// // // // // // //             if (Physics.Raycast(Pickupray, out RaycastHit hitInfo, PickupRange, PickupLayer))
// // // // // // //             {
// // // // // // //                 if (CurrentObjectRigidBody)
// // // // // // //                 {
// // // // // // //                     StartCoroutine(PickupAnimationSequence(hitInfo.collider.gameObject.GetComponent<PhotonView>().ViewID, true));
// // // // // // //                 }
// // // // // // //                 else
// // // // // // //                 {
// // // // // // //                     StartCoroutine(PickupAnimationSequence(hitInfo.collider.gameObject.GetComponent<PhotonView>().ViewID, false));
// // // // // // //                 }
// // // // // // //                 return;
// // // // // // //             }
// // // // // // //             if (CurrentObjectRigidBody)
// // // // // // //             {
// // // // // // //                 photonView.RPC("Drop", RpcTarget.All);
// // // // // // //             }
// // // // // // //         }

// // // // // // //         if (Input.GetKeyDown(KeyCode.Q) && !isAnimating)
// // // // // // //         {
// // // // // // //             if (CurrentObjectRigidBody)
// // // // // // //             {
// // // // // // //                 StartCoroutine(ThrowAnimationSequence());
// // // // // // //             }
// // // // // // //         }

// // // // // // //         if (CurrentObjectRigidBody && !isThrown)
// // // // // // //         {
// // // // // // //             photonView.RPC("UpdatePosition", RpcTarget.All);
// // // // // // //         }
// // // // // // //     }

// // // // // // //     private IEnumerator PickupAnimationSequence(int objectViewID, bool isDropAndPickup)
// // // // // // //     {
// // // // // // //         isAnimating = true;

// // // // // // //         // Play pickup animation
// // // // // // //         photonView.RPC("PlayPickupAnimation", RpcTarget.All);

// // // // // // //         // Wait for animation
// // // // // // //         yield return new WaitForSeconds(pickupAnimationDuration);

// // // // // // //         // Return to idle
// // // // // // //         photonView.RPC("PlayIdleAnimation", RpcTarget.All);

// // // // // // //         // Perform actual pickup
// // // // // // //         if (isDropAndPickup)
// // // // // // //         {
// // // // // // //             photonView.RPC("DropAndPickup", RpcTarget.All, objectViewID);
// // // // // // //         }
// // // // // // //         else
// // // // // // //         {
// // // // // // //             photonView.RPC("Pickup", RpcTarget.All, objectViewID);
// // // // // // //         }

// // // // // // //         isAnimating = false;
// // // // // // //     }

// // // // // // //     private IEnumerator ThrowAnimationSequence()
// // // // // // //     {
// // // // // // //         isAnimating = true;

// // // // // // //         // Play throw animation
// // // // // // //         photonView.RPC("PlayThrowAnimation", RpcTarget.All);

// // // // // // //         // Wait for animation
// // // // // // //         yield return new WaitForSeconds(throwAnimationDuration);

// // // // // // //         // Return to idle
// // // // // // //         photonView.RPC("PlayIdleAnimation", RpcTarget.All);

// // // // // // //         // Perform actual throw
// // // // // // //         photonView.RPC("Throw", RpcTarget.All, PlayerCamera.transform.forward);

// // // // // // //         isAnimating = false;
// // // // // // //     }

// // // // // // //     [PunRPC]
// // // // // // //     private void PlayPickupAnimation()
// // // // // // //     {
// // // // // // //         if (playerAnimator)
// // // // // // //         {

// // // // // // //             playerAnimator.CrossFade(pickupStateHash, transitionDuration);
// // // // // // //             playerAnimator.SetTrigger("PickupTrigger");
// // // // // // //         }
// // // // // // //     }

// // // // // // //     [PunRPC]
// // // // // // //     private void PlayThrowAnimation()
// // // // // // //     {
// // // // // // //         if (playerAnimator)
// // // // // // //         {

// // // // // // //             playerAnimator.CrossFade(throwStateHash, transitionDuration);
// // // // // // //             playerAnimator.SetTrigger("ThrowTrigger");
// // // // // // //         }
// // // // // // //     }

// // // // // // //     [PunRPC]
// // // // // // //     private void PlayFallAnimation()
// // // // // // //     {
// // // // // // //         if (playerAnimator)
// // // // // // //         {
// // // // // // //             playerAnimator.CrossFade(fallStateHash, transitionDuration);
// // // // // // //             StartCoroutine(ReturnToIdleAfterDelay(1f));
// // // // // // //             playerAnimator.SetTrigger("fall"); // Adjust delay as needed
// // // // // // //         }
// // // // // // //     }

// // // // // // //     [PunRPC]
// // // // // // //     private void PlayIdleAnimation()
// // // // // // //     {
// // // // // // //         if (playerAnimator)
// // // // // // //         {
// // // // // // //             playerAnimator.CrossFade(idleStateHash, transitionDuration);
// // // // // // //         }
// // // // // // //     }

// // // // // // //     private IEnumerator ReturnToIdleAfterDelay(float delay)
// // // // // // //     {
// // // // // // //         yield return new WaitForSeconds(delay);
// // // // // // //         PlayIdleAnimation();
// // // // // // //     }

// // // // // // //     [PunRPC]
// // // // // // //     private void Pickup(int objectViewID)
// // // // // // //     {
// // // // // // //         GameObject pickupObject = PhotonView.Find(objectViewID).gameObject;
// // // // // // //         CurrentObjectRigidBody = pickupObject.GetComponent<Rigidbody>();
// // // // // // //         CurrentObjectCollider = pickupObject.GetComponent<Collider>();

// // // // // // //         if (!CurrentObjectRigidBody.gameObject.GetComponent<ThrowableCollisionHandler>())
// // // // // // //         {
// // // // // // //             CurrentObjectRigidBody.gameObject.AddComponent<ThrowableCollisionHandler>().Initialize(this);
// // // // // // //         }

// // // // // // //         CurrentObjectRigidBody.isKinematic = true;
// // // // // // //         CurrentObjectCollider.enabled = false;
// // // // // // //         isThrown = false;
// // // // // // //     }

// // // // // // //     [PunRPC]
// // // // // // //     private void Drop()
// // // // // // //     {
// // // // // // //         CurrentObjectRigidBody.isKinematic = false;
// // // // // // //         CurrentObjectCollider.enabled = true;
// // // // // // //         CurrentObjectRigidBody = null;
// // // // // // //         CurrentObjectCollider = null;
// // // // // // //         isThrown = false;
// // // // // // //     }

// // // // // // //     [PunRPC]
// // // // // // //     private void DropAndPickup(int newObjectViewID)
// // // // // // //     {
// // // // // // //         CurrentObjectRigidBody.isKinematic = false;
// // // // // // //         CurrentObjectCollider.enabled = true;

// // // // // // //         GameObject pickupObject = PhotonView.Find(newObjectViewID).gameObject;
// // // // // // //         CurrentObjectRigidBody = pickupObject.GetComponent<Rigidbody>();
// // // // // // //         CurrentObjectCollider = pickupObject.GetComponent<Collider>();

// // // // // // //         if (!CurrentObjectRigidBody.gameObject.GetComponent<ThrowableCollisionHandler>())
// // // // // // //         {
// // // // // // //             CurrentObjectRigidBody.gameObject.AddComponent<ThrowableCollisionHandler>().Initialize(this);
// // // // // // //         }

// // // // // // //         CurrentObjectRigidBody.isKinematic = true;
// // // // // // //         CurrentObjectCollider.enabled = false;
// // // // // // //         isThrown = false;
// // // // // // //     }

// // // // // // //     [PunRPC]
// // // // // // //     private void Throw(Vector3 direction)
// // // // // // //     {
// // // // // // //         CurrentObjectRigidBody.isKinematic = false;
// // // // // // //         CurrentObjectCollider.enabled = true;
// // // // // // //         CurrentObjectRigidBody.AddForce(direction * ThrowingForce, ForceMode.Impulse);
// // // // // // //         isThrown = true;

// // // // // // //         StartCoroutine(ClearObjectAfterDelay());
// // // // // // //     }

// // // // // // //     private IEnumerator ClearObjectAfterDelay()
// // // // // // //     {
// // // // // // //         yield return new WaitForSeconds(0.1f);
// // // // // // //         CurrentObjectRigidBody = null;
// // // // // // //         CurrentObjectCollider = null;
// // // // // // //         isThrown = false;
// // // // // // //     }

// // // // // // //     [PunRPC]
// // // // // // //     private void UpdatePosition()
// // // // // // //     {
// // // // // // //         CurrentObjectRigidBody.position = Hand.position;
// // // // // // //         CurrentObjectRigidBody.rotation = Hand.rotation;
// // // // // // //     }

// // // // // // //     [PunRPC]
// // // // // // //     public void ApplyKnockback(int hitPlayerViewID, Vector3 hitDirection)
// // // // // // //     {
// // // // // // //         PhotonView hitPlayerView = PhotonView.Find(hitPlayerViewID);
// // // // // // //         if (hitPlayerView != null)
// // // // // // //         {
// // // // // // //             // Play fall animation on the hit player
// // // // // // //             Animator hitPlayerAnimator = hitPlayerView.gameObject.GetComponent<Animator>();
// // // // // // //             if (hitPlayerAnimator)
// // // // // // //             {
// // // // // // //                 hitPlayerView.RPC("PlayFallAnimation", RpcTarget.All);
// // // // // // //             }

// // // // // // //             Rigidbody playerRb = hitPlayerView.gameObject.GetComponent<Rigidbody>();
// // // // // // //             if (playerRb != null)
// // // // // // //             {
// // // // // // //                 Vector3 knockbackDirection = (hitDirection + Vector3.up * KnockbackUpwardForce).normalized;
// // // // // // //                 playerRb.AddForce(knockbackDirection * KnockbackForce, ForceMode.Impulse);
// // // // // // //             }
// // // // // // //         }
// // // // // // //     }
// // // // // // // }

// // // // // // // public class ThrowableCollisionHandler : MonoBehaviour
// // // // // // // {
// // // // // // //     private PickupClass pickupClass;
// // // // // // //     private bool hasCollided = false;

// // // // // // //     public void Initialize(PickupClass pickup)
// // // // // // //     {
// // // // // // //         pickupClass = pickup;
// // // // // // //     }

// // // // // // //     private void OnCollisionEnter(Collision collision)
// // // // // // //     {
// // // // // // //         if (hasCollided) return;

// // // // // // //         PhotonView hitPlayerView = collision.gameObject.GetComponent<PhotonView>();
// // // // // // //         if (hitPlayerView != null && pickupClass != null)
// // // // // // //         {
// // // // // // //             Vector3 hitDirection = (collision.contacts[0].point - transform.position).normalized;
// // // // // // //             pickupClass.photonView.RPC("ApplyKnockback", RpcTarget.All, hitPlayerView.ViewID, hitDirection);
// // // // // // //             hasCollided = true;
// // // // // // //         }
// // // // // // //     }
// // // // // // // }


// // // // // // using UnityEngine;
// // // // // // using Photon.Pun;
// // // // // // using System.Collections;

// // // // // // public class PickupClass : MonoBehaviourPunCallbacks
// // // // // // {
// // // // // //     [Header("Pickup Settings")]
// // // // // //     [SerializeField] private LayerMask PickupLayer;
// // // // // //     [SerializeField] private GameObject PlayerCamera;
// // // // // //     [SerializeField] private float PickupRange = 3f;
// // // // // //     [SerializeField] private Transform Hand;
// // // // // //     [SerializeField] private float ThrowingForce = 10f;

// // // // // //     [Header("Knockback Settings")]
// // // // // //     [SerializeField] private float KnockbackForce = 10f;
// // // // // //     [SerializeField] private float KnockbackUpwardForce = 2f;

// // // // // //     [Header("Animation Settings")]
// // // // // //     [SerializeField] private Animator playerAnimator;
// // // // // //     [SerializeField] private float pickupAnimationDuration = 1f;
// // // // // //     [SerializeField] private float throwAnimationDuration = 0.5f;
// // // // // //     [SerializeField] private float fallAnimationDuration = 1f;
// // // // // //     [SerializeField] private float transitionDuration = 0.1f;

// // // // // //     [Header("Animation State Names")]
// // // // // //     [SerializeField] private string pickupStateName = "pickup";
// // // // // //     [SerializeField] private string throwStateName = "throw";
// // // // // //     [SerializeField] private string fallStateName = "fall";
// // // // // //     [SerializeField] private string idleStateName = "idle";

// // // // // //     private int pickupStateHash;
// // // // // //     private int throwStateHash;
// // // // // //     private int fallStateHash;
// // // // // //     private int idleStateHash;

// // // // // //     private Rigidbody CurrentObjectRigidBody;
// // // // // //     private Collider CurrentObjectCollider;
// // // // // //     private PhotonView photonView;
// // // // // //     private bool isThrown = false;
// // // // // //     private bool isAnimating = false;
// // // // // //     private bool isFalling = false;

// // // // // //     void Start()
// // // // // //     {
// // // // // //         photonView = GetComponent<PhotonView>();
// // // // // //         if (!playerAnimator)
// // // // // //         {
// // // // // //             playerAnimator = GetComponent<Animator>();
// // // // // //         }

// // // // // //         if (PlayerCamera == null)
// // // // // //         {
// // // // // //             PlayerCamera = Camera.main.gameObject;
// // // // // //         }

// // // // // //         // Cache animation state hashes
// // // // // //         pickupStateHash = Animator.StringToHash(pickupStateName);
// // // // // //         throwStateHash = Animator.StringToHash(throwStateName);
// // // // // //         fallStateHash = Animator.StringToHash(fallStateName);
// // // // // //         idleStateHash = Animator.StringToHash(idleStateName);
// // // // // //     }

// // // // // //     void Update()
// // // // // //     {
// // // // // //         if (!photonView.IsMine) return;

// // // // // //         // Pickup/Drop Logic
// // // // // //         if (Input.GetKeyDown(KeyCode.E) && !isAnimating)
// // // // // //         {
// // // // // //             Ray Pickupray = new Ray(PlayerCamera.transform.position, PlayerCamera.transform.forward);
// // // // // //             if (Physics.Raycast(Pickupray, out RaycastHit hitInfo, PickupRange, PickupLayer))
// // // // // //             {
// // // // // //                 PhotonView objectView = hitInfo.collider.gameObject.GetComponent<PhotonView>();
// // // // // //                 if (objectView != null)
// // // // // //                 {
// // // // // //                     if (CurrentObjectRigidBody)
// // // // // //                     {
// // // // // //                         StartCoroutine(PickupAnimationSequence(objectView.ViewID, true));
// // // // // //                     }
// // // // // //                     else
// // // // // //                     {
// // // // // //                         StartCoroutine(PickupAnimationSequence(objectView.ViewID, false));
// // // // // //                     }
// // // // // //                 }
// // // // // //                 return;
// // // // // //             }
// // // // // //             if (CurrentObjectRigidBody)
// // // // // //             {
// // // // // //                 photonView.RPC("Drop", RpcTarget.All);
// // // // // //             }
// // // // // //         }

// // // // // //         // Throw Logic
// // // // // //         if (Input.GetKeyDown(KeyCode.Q) && CurrentObjectRigidBody && !isAnimating)
// // // // // //         {
// // // // // //             StartCoroutine(ThrowAnimationSequence());
// // // // // //         }

// // // // // //         // Update held object position
// // // // // //         if (CurrentObjectRigidBody && !isThrown)
// // // // // //         {
// // // // // //             photonView.RPC("UpdatePosition", RpcTarget.All);
// // // // // //         }
// // // // // //     }

// // // // // //     private IEnumerator PickupAnimationSequence(int objectViewID, bool isDropAndPickup)
// // // // // //     {
// // // // // //         isAnimating = true;

// // // // // //         // Play pickup animation
// // // // // //         photonView.RPC("PlayPickupAnimation", RpcTarget.All);

// // // // // //         // Wait for animation
// // // // // //         yield return new WaitForSeconds(pickupAnimationDuration);

// // // // // //         // Perform actual pickup
// // // // // //         if (isDropAndPickup)
// // // // // //         {
// // // // // //             photonView.RPC("DropAndPickup", RpcTarget.All, objectViewID);
// // // // // //         }
// // // // // //         else
// // // // // //         {
// // // // // //             photonView.RPC("Pickup", RpcTarget.All, objectViewID);
// // // // // //         }

// // // // // //         // Return to idle
// // // // // //         photonView.RPC("PlayIdleAnimation", RpcTarget.All);

// // // // // //         isAnimating = false;
// // // // // //     }

// // // // // //     private IEnumerator ThrowAnimationSequence()
// // // // // //     {
// // // // // //         isAnimating = true;

// // // // // //         // Play throw animation
// // // // // //         photonView.RPC("PlayThrowAnimation", RpcTarget.All);

// // // // // //         // Wait for animation
// // // // // //         yield return new WaitForSeconds(throwAnimationDuration * 0.5f); // Throw object midway through animation

// // // // // //         // Perform actual throw
// // // // // //         photonView.RPC("Throw", RpcTarget.All, PlayerCamera.transform.forward);

// // // // // //         yield return new WaitForSeconds(throwAnimationDuration * 0.5f); // Complete animation

// // // // // //         // Return to idle
// // // // // //         photonView.RPC("PlayIdleAnimation", RpcTarget.All);

// // // // // //         isAnimating = false;
// // // // // //     }

// // // // // //     [PunRPC]
// // // // // //     private void PlayPickupAnimation()
// // // // // //     {
// // // // // //         if (playerAnimator)
// // // // // //         {
// // // // // //             playerAnimator.ResetTrigger("ThrowTrigger");
// // // // // //             playerAnimator.ResetTrigger("fall");

// // // // // //             playerAnimator.SetTrigger("PickupTrigger");
// // // // // //             playerAnimator.CrossFade(pickupStateHash, transitionDuration);
// // // // // //         }
// // // // // //     }

// // // // // //     [PunRPC]
// // // // // //     private void PlayThrowAnimation()
// // // // // //     {
// // // // // //         if (playerAnimator)
// // // // // //         {
// // // // // //             playerAnimator.ResetTrigger("PickupTrigger");
// // // // // //             playerAnimator.ResetTrigger("fall");

// // // // // //             playerAnimator.SetTrigger("ThrowTrigger");
// // // // // //             playerAnimator.CrossFade(throwStateHash, transitionDuration);
// // // // // //         }
// // // // // //     }

// // // // // //     // [PunRPC]
// // // // // //     // private void PlayFallAnimation()
// // // // // //     // {
// // // // // //     //     if (!playerAnimator || isFalling) return;

// // // // // //     //     isFalling = true;
// // // // // //     //     isAnimating = true;

// // // // // //     //     // Reset any existing triggers
// // // // // //     //     playerAnimator.ResetTrigger("PickupTrigger");
// // // // // //     //     playerAnimator.ResetTrigger("ThrowTrigger");

// // // // // //     //     playerAnimator.SetTrigger("Fall");
// // // // // //     //     playerAnimator.CrossFade(fallStateHash, transitionDuration);

// // // // // //     //     StartCoroutine(FallAnimationSequence());
// // // // // //     // }
// // // // // //     [PunRPC]
// // // // // //     private void PlayFallAnimation()
// // // // // //     {
// // // // // //         if (playerAnimator && !isFalling)
// // // // // //         {
// // // // // //             isFalling = true;
// // // // // //             isAnimating = true;

// // // // // //             // Reset any existing triggers
// // // // // //             playerAnimator.ResetTrigger("PickupTrigger");
// // // // // //             playerAnimator.ResetTrigger("ThrowTrigger");
// // // // // //             // playerAnimator.ResetTrigger("FallEnd"); // Reset any end triggers to allow re-entry

// // // // // //             playerAnimator.SetTrigger("fall");
// // // // // //             playerAnimator.CrossFade(fallStateHash, transitionDuration);

// // // // // //             // StartCoroutine(FallAnimationSequence());
// // // // // //         }
// // // // // //     }


// // // // // //     // private IEnumerator FallAnimationSequence()
// // // // // //     // {
// // // // // //     //     // Wait for the fall animation to complete
// // // // // //     //     yield return new WaitForSeconds(fallAnimationDuration);

// // // // // //     //     if (playerAnimator)
// // // // // //     //     {
// // // // // //     //         // Manually set the state to Idle once the fall animation is done
// // // // // //     //         playerAnimator.SetTrigger("FallEnd");
// // // // // //     //         playerAnimator.CrossFade(idleStateHash, transitionDuration);
// // // // // //     //     }

// // // // // //     //     isFalling = false;
// // // // // //     //     isAnimating = false;
// // // // // //     // }

// // // // // //     [PunRPC]
// // // // // //     private void PlayIdleAnimation()
// // // // // //     {
// // // // // //         if (playerAnimator)
// // // // // //         {
// // // // // //             playerAnimator.CrossFade(idleStateHash, transitionDuration);
// // // // // //         }
// // // // // //     }

// // // // // //     [PunRPC]
// // // // // //     private void Pickup(int objectViewID)
// // // // // //     {
// // // // // //         GameObject pickupObject = PhotonView.Find(objectViewID).gameObject;
// // // // // //         CurrentObjectRigidBody = pickupObject.GetComponent<Rigidbody>();
// // // // // //         CurrentObjectCollider = pickupObject.GetComponent<Collider>();

// // // // // //         if (!CurrentObjectRigidBody.gameObject.GetComponent<ThrowableCollisionHandler>())
// // // // // //         {
// // // // // //             CurrentObjectRigidBody.gameObject.AddComponent<ThrowableCollisionHandler>().Initialize(this);
// // // // // //         }

// // // // // //         CurrentObjectRigidBody.isKinematic = true;
// // // // // //         CurrentObjectCollider.enabled = false;
// // // // // //         isThrown = false;
// // // // // //     }

// // // // // //     [PunRPC]
// // // // // //     private void Drop()
// // // // // //     {
// // // // // //         if (CurrentObjectRigidBody != null)
// // // // // //         {
// // // // // //             CurrentObjectRigidBody.isKinematic = false;
// // // // // //             CurrentObjectCollider.enabled = true;
// // // // // //             CurrentObjectRigidBody = null;
// // // // // //             CurrentObjectCollider = null;
// // // // // //             isThrown = false;
// // // // // //         }
// // // // // //     }

// // // // // //     [PunRPC]
// // // // // //     private void DropAndPickup(int newObjectViewID)
// // // // // //     {
// // // // // //         if (CurrentObjectRigidBody != null)
// // // // // //         {
// // // // // //             CurrentObjectRigidBody.isKinematic = false;
// // // // // //             CurrentObjectCollider.enabled = true;
// // // // // //         }

// // // // // //         GameObject pickupObject = PhotonView.Find(newObjectViewID).gameObject;
// // // // // //         CurrentObjectRigidBody = pickupObject.GetComponent<Rigidbody>();
// // // // // //         CurrentObjectCollider = pickupObject.GetComponent<Collider>();

// // // // // //         if (!CurrentObjectRigidBody.gameObject.GetComponent<ThrowableCollisionHandler>())
// // // // // //         {
// // // // // //             CurrentObjectRigidBody.gameObject.AddComponent<ThrowableCollisionHandler>().Initialize(this);
// // // // // //         }

// // // // // //         CurrentObjectRigidBody.isKinematic = true;
// // // // // //         CurrentObjectCollider.enabled = false;
// // // // // //         isThrown = false;
// // // // // //     }

// // // // // //     [PunRPC]
// // // // // //     private void Throw(Vector3 direction)
// // // // // //     {
// // // // // //         if (CurrentObjectRigidBody != null)
// // // // // //         {
// // // // // //             CurrentObjectRigidBody.isKinematic = false;
// // // // // //             CurrentObjectCollider.enabled = true;
// // // // // //             CurrentObjectRigidBody.AddForce(direction * ThrowingForce, ForceMode.Impulse);
// // // // // //             isThrown = true;

// // // // // //             StartCoroutine(ClearObjectAfterDelay());
// // // // // //         }
// // // // // //     }

// // // // // //     private IEnumerator ClearObjectAfterDelay()
// // // // // //     {
// // // // // //         yield return new WaitForSeconds(0.1f);
// // // // // //         CurrentObjectRigidBody = null;
// // // // // //         CurrentObjectCollider = null;
// // // // // //         isThrown = false;
// // // // // //     }

// // // // // //     [PunRPC]
// // // // // //     private void UpdatePosition()
// // // // // //     {
// // // // // //         if (CurrentObjectRigidBody != null)
// // // // // //         {
// // // // // //             CurrentObjectRigidBody.MovePosition(Hand.position);
// // // // // //             CurrentObjectRigidBody.MoveRotation(Hand.rotation);
// // // // // //         }
// // // // // //     }

// // // // // //     [PunRPC]
// // // // // //     public void ApplyKnockback(int hitPlayerViewID, Vector3 hitDirection)
// // // // // //     {
// // // // // //         PhotonView hitPlayerView = PhotonView.Find(hitPlayerViewID);
// // // // // //         if (hitPlayerView != null)
// // // // // //         {
// // // // // //             PickupClass hitPlayerPickup = hitPlayerView.gameObject.GetComponent<PickupClass>();
// // // // // //             if (hitPlayerPickup != null)
// // // // // //             {
// // // // // //                 hitPlayerPickup.photonView.RPC("PlayFallAnimation", RpcTarget.All);
// // // // // //             }

// // // // // //             Rigidbody playerRb = hitPlayerView.gameObject.GetComponent<Rigidbody>();
// // // // // //             if (playerRb != null)
// // // // // //             {
// // // // // //                 Vector3 knockbackDirection = (hitDirection + Vector3.up * KnockbackUpwardForce).normalized;
// // // // // //                 playerRb.AddForce(knockbackDirection * KnockbackForce, ForceMode.Impulse);
// // // // // //             }
// // // // // //         }
// // // // // //     }
// // // // // // }

// // // // // // public class ThrowableCollisionHandler : MonoBehaviour
// // // // // // {
// // // // // //     private PickupClass pickupClass;
// // // // // //     private bool hasCollided = false;
// // // // // //     private float collisionCooldown = 0.5f; // Prevent multiple collisions
// // // // // //     private float lastCollisionTime;

// // // // // //     public void Initialize(PickupClass pickup)
// // // // // //     {
// // // // // //         pickupClass = pickup;
// // // // // //         lastCollisionTime = -collisionCooldown; // Allow immediate first collision
// // // // // //     }

// // // // // //     // private void OnCollisionEnter(Collision collision)
// // // // // //     // {
// // // // // //     //     if (hasCollided || Time.time - lastCollisionTime < collisionCooldown) return;

// // // // // //     //     PhotonView hitPlayerView = collision.gameObject.GetComponent<PhotonView>();
// // // // // //     //     if (hitPlayerView != null && pickupClass != null)
// // // // // //     //     {
// // // // // //     //         Vector3 hitDirection = (collision.contacts[0].point - transform.position).normalized;
// // // // // //     //         pickupClass.photonView.RPC("ApplyKnockback", RpcTarget.All, hitPlayerView.ViewID, hitDirection);

// // // // // //     //         lastCollisionTime = Time.time;
// // // // // //     //         hasCollided = true;
// // // // // //     //     }
// // // // // //     // }
// // // // // //     private void OnCollisionEnter(Collision collision)
// // // // // //     {
// // // // // //         if (hasCollided || Time.time - lastCollisionTime < collisionCooldown) return;

// // // // // //         Debug.Log("Collision detected with: " + collision.gameObject.name); // Log collision

// // // // // //         PhotonView hitPlayerView = collision.gameObject.GetComponent<PhotonView>();
// // // // // //         if (hitPlayerView != null && pickupClass != null)
// // // // // //         {
// // // // // //             Vector3 hitDirection = (collision.contacts[0].point - transform.position).normalized;
// // // // // //             pickupClass.photonView.RPC("ApplyKnockback", RpcTarget.All, hitPlayerView.ViewID, hitDirection);

// // // // // //             lastCollisionTime = Time.time;
// // // // // //             hasCollided = true;
// // // // // //         }
// // // // // //     }
// // // // // // }


// // // // // using UnityEngine;
// // // // // using Photon.Pun;
// // // // // using System.Collections;

// // // // // public class PickupClass : MonoBehaviourPunCallbacks
// // // // // {
// // // // //     [Header("Pickup Settings")]
// // // // //     [SerializeField] private LayerMask PickupLayer;
// // // // //     [SerializeField] private GameObject PlayerCamera;
// // // // //     [SerializeField] private float PickupRange = 3f;
// // // // //     [SerializeField] private Transform Hand;
// // // // //     [SerializeField] private float ThrowingForce = 10f;

// // // // //     [Header("Knockback Settings")]
// // // // //     [SerializeField] private float KnockbackForce = 10f;
// // // // //     [SerializeField] private float KnockbackUpwardForce = 2f;
// // // // //     [SerializeField] private float KnockbackBackwardForce = 5f;

// // // // //     [Header("Animation Settings")]
// // // // //     [SerializeField] private Animator playerAnimator;
// // // // //     [SerializeField] private float pickupAnimationDuration = 1f;
// // // // //     [SerializeField] private float throwAnimationDuration = 0.5f;
// // // // //     [SerializeField] private float fallAnimationDuration = 1f;
// // // // //     [SerializeField] private float transitionDuration = 0.1f;

// // // // //     [Header("Animation State Names")]
// // // // //     [SerializeField] private string pickupStateName = "pickup";
// // // // //     [SerializeField] private string throwStateName = "throw";
// // // // //     [SerializeField] private string fallStateName = "fall";
// // // // //     [SerializeField] private string idleStateName = "idle";

// // // // //     private int pickupStateHash;
// // // // //     private int throwStateHash;
// // // // //     private int fallStateHash;
// // // // //     private int idleStateHash;

// // // // //     private Rigidbody CurrentObjectRigidBody;
// // // // //     private Collider CurrentObjectCollider;
// // // // //     private PhotonView photonView;
// // // // //     private bool isThrown = false;
// // // // //     private bool isAnimating = false;
// // // // //     private bool isFalling = false;

// // // // //     void Start()
// // // // //     {
// // // // //         photonView = GetComponent<PhotonView>();
// // // // //         if (!playerAnimator)
// // // // //         {
// // // // //             playerAnimator = GetComponent<Animator>();
// // // // //         }

// // // // //         if (PlayerCamera == null)
// // // // //         {
// // // // //             PlayerCamera = Camera.main.gameObject;
// // // // //         }

// // // // //         pickupStateHash = Animator.StringToHash(pickupStateName);
// // // // //         throwStateHash = Animator.StringToHash(throwStateName);
// // // // //         fallStateHash = Animator.StringToHash(fallStateName);
// // // // //         idleStateHash = Animator.StringToHash(idleStateName);
// // // // //     }

// // // // //     void Update()
// // // // //     {
// // // // //         if (!photonView.IsMine) return;

// // // // //         HandlePickupAndDrop();
// // // // //         HandleThrow();
// // // // //         UpdateHeldObject();
// // // // //     }

// // // // //     private void HandlePickupAndDrop()
// // // // //     {
// // // // //         if (Input.GetKeyDown(KeyCode.E) && !isAnimating)
// // // // //         {
// // // // //             if (!CurrentObjectRigidBody)
// // // // //             {
// // // // //                 TryPickupObject();
// // // // //             }
// // // // //             else
// // // // //             {
// // // // //                 photonView.RPC("Drop", RpcTarget.All);
// // // // //             }
// // // // //         }
// // // // //     }

// // // // //     private void TryPickupObject()
// // // // //     {
// // // // //         Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, PickupRange, PickupLayer);

// // // // //         if (nearbyObjects.Length > 0)
// // // // //         {
// // // // //             Collider closestObject = GetClosestObject(nearbyObjects);
// // // // //             if (closestObject != null)
// // // // //             {
// // // // //                 PhotonView objectView = closestObject.gameObject.GetComponent<PhotonView>();
// // // // //                 if (objectView != null)
// // // // //                 {
// // // // //                     StartCoroutine(PickupAnimationSequence(objectView.ViewID, false));
// // // // //                 }
// // // // //             }
// // // // //         }
// // // // //     }

// // // // //     private Collider GetClosestObject(Collider[] objects)
// // // // //     {
// // // // //         Collider closestObject = null;
// // // // //         float closestDistance = float.MaxValue;

// // // // //         foreach (Collider obj in objects)
// // // // //         {
// // // // //             float distance = Vector3.Distance(transform.position, obj.transform.position);
// // // // //             if (distance < closestDistance)
// // // // //             {
// // // // //                 closestDistance = distance;
// // // // //                 closestObject = obj;
// // // // //             }
// // // // //         }

// // // // //         return closestObject;
// // // // //     }

// // // // //     private void HandleThrow()
// // // // //     {
// // // // //         if (Input.GetKeyDown(KeyCode.Q) && CurrentObjectRigidBody && !isAnimating)
// // // // //         {
// // // // //             StartCoroutine(ThrowAnimationSequence());
// // // // //         }
// // // // //     }

// // // // //     private void UpdateHeldObject()
// // // // //     {
// // // // //         if (CurrentObjectRigidBody && !isThrown)
// // // // //         {
// // // // //             photonView.RPC("UpdatePosition", RpcTarget.All);
// // // // //         }
// // // // //     }

// // // // //     private IEnumerator PickupAnimationSequence(int objectViewID, bool isDropAndPickup)
// // // // //     {
// // // // //         isAnimating = true;

// // // // //         photonView.RPC("PlayPickupAnimation", RpcTarget.All);
// // // // //         yield return new WaitForSeconds(pickupAnimationDuration);

// // // // //         if (isDropAndPickup)
// // // // //         {
// // // // //             photonView.RPC("DropAndPickup", RpcTarget.All, objectViewID);
// // // // //         }
// // // // //         else
// // // // //         {
// // // // //             photonView.RPC("Pickup", RpcTarget.All, objectViewID);
// // // // //         }

// // // // //         photonView.RPC("PlayIdleAnimation", RpcTarget.All);
// // // // //         isAnimating = false;
// // // // //     }

// // // // //     private IEnumerator ThrowAnimationSequence()
// // // // //     {
// // // // //         isAnimating = true;

// // // // //         photonView.RPC("PlayThrowAnimation", RpcTarget.All);
// // // // //         yield return new WaitForSeconds(throwAnimationDuration * 0.5f);

// // // // //         photonView.RPC("Throw", RpcTarget.All, PlayerCamera.transform.forward);
// // // // //         yield return new WaitForSeconds(throwAnimationDuration * 0.5f);

// // // // //         photonView.RPC("PlayIdleAnimation", RpcTarget.All);
// // // // //         isAnimating = false;
// // // // //     }

// // // // //     [PunRPC]
// // // // //     private void PlayPickupAnimation()
// // // // //     {
// // // // //         if (playerAnimator)
// // // // //         {
// // // // //             playerAnimator.ResetTrigger("ThrowTrigger");
// // // // //             playerAnimator.ResetTrigger("fall");
// // // // //             playerAnimator.SetTrigger("PickupTrigger");
// // // // //             playerAnimator.CrossFade(pickupStateHash, transitionDuration);
// // // // //         }
// // // // //     }

// // // // //     [PunRPC]
// // // // //     private void PlayThrowAnimation()
// // // // //     {
// // // // //         if (playerAnimator)
// // // // //         {
// // // // //             playerAnimator.ResetTrigger("PickupTrigger");
// // // // //             playerAnimator.ResetTrigger("fall");
// // // // //             playerAnimator.SetTrigger("ThrowTrigger");
// // // // //             playerAnimator.CrossFade(throwStateHash, transitionDuration);
// // // // //         }
// // // // //     }

// // // // //     [PunRPC]
// // // // //     private void PlayFallAnimation()
// // // // //     {
// // // // //         if (playerAnimator && !isFalling)
// // // // //         {
// // // // //             isFalling = true;
// // // // //             isAnimating = true;

// // // // //             playerAnimator.ResetTrigger("PickupTrigger");
// // // // //             playerAnimator.ResetTrigger("ThrowTrigger");
// // // // //             playerAnimator.SetTrigger("fall");
// // // // //             playerAnimator.CrossFade(fallStateHash, transitionDuration);

// // // // //             StartCoroutine(AutoRecoverFromFall());
// // // // //         }
// // // // //     }

// // // // //     private IEnumerator AutoRecoverFromFall()
// // // // //     {
// // // // //         yield return new WaitForSeconds(fallAnimationDuration);

// // // // //         if (playerAnimator)
// // // // //         {
// // // // //             playerAnimator.CrossFade(idleStateHash, transitionDuration);
// // // // //         }

// // // // //         isFalling = false;
// // // // //         isAnimating = false;
// // // // //     }

// // // // //     [PunRPC]
// // // // //     private void PlayIdleAnimation()
// // // // //     {
// // // // //         if (playerAnimator)
// // // // //         {
// // // // //             playerAnimator.CrossFade(idleStateHash, transitionDuration);
// // // // //         }
// // // // //     }

// // // // //     [PunRPC]
// // // // //     private void Pickup(int objectViewID)
// // // // //     {
// // // // //         GameObject pickupObject = PhotonView.Find(objectViewID).gameObject;
// // // // //         SetupPickedObject(pickupObject);
// // // // //     }

// // // // //     [PunRPC]
// // // // //     private void Drop()
// // // // //     {
// // // // //         if (CurrentObjectRigidBody != null)
// // // // //         {
// // // // //             ReleaseObject();
// // // // //         }
// // // // //     }

// // // // //     [PunRPC]
// // // // //     private void DropAndPickup(int newObjectViewID)
// // // // //     {
// // // // //         if (CurrentObjectRigidBody != null)
// // // // //         {
// // // // //             ReleaseObject();
// // // // //         }

// // // // //         GameObject pickupObject = PhotonView.Find(newObjectViewID).gameObject;
// // // // //         SetupPickedObject(pickupObject);
// // // // //     }

// // // // //     private void SetupPickedObject(GameObject pickupObject)
// // // // //     {
// // // // //         CurrentObjectRigidBody = pickupObject.GetComponent<Rigidbody>();
// // // // //         CurrentObjectCollider = pickupObject.GetComponent<Collider>();

// // // // //         if (!CurrentObjectRigidBody.gameObject.GetComponent<ThrowableCollisionHandler>())
// // // // //         {
// // // // //             CurrentObjectRigidBody.gameObject.AddComponent<ThrowableCollisionHandler>().Initialize(this);
// // // // //         }

// // // // //         CurrentObjectRigidBody.isKinematic = true;
// // // // //         CurrentObjectCollider.enabled = false;
// // // // //         isThrown = false;
// // // // //     }

// // // // //     private void ReleaseObject()
// // // // //     {
// // // // //         CurrentObjectRigidBody.isKinematic = false;
// // // // //         CurrentObjectCollider.enabled = true;
// // // // //         CurrentObjectRigidBody = null;
// // // // //         CurrentObjectCollider = null;
// // // // //         isThrown = false;
// // // // //     }

// // // // //     [PunRPC]
// // // // //     private void Throw(Vector3 direction)
// // // // //     {
// // // // //         if (CurrentObjectRigidBody != null)
// // // // //         {
// // // // //             CurrentObjectRigidBody.isKinematic = false;
// // // // //             CurrentObjectCollider.enabled = true;
// // // // //             CurrentObjectRigidBody.AddForce(direction * ThrowingForce, ForceMode.Impulse);
// // // // //             isThrown = true;

// // // // //             StartCoroutine(ClearObjectAfterDelay());
// // // // //         }
// // // // //     }

// // // // //     private IEnumerator ClearObjectAfterDelay()
// // // // //     {
// // // // //         yield return new WaitForSeconds(0.1f);
// // // // //         CurrentObjectRigidBody = null;
// // // // //         CurrentObjectCollider = null;
// // // // //         isThrown = false;
// // // // //     }

// // // // //     [PunRPC]
// // // // //     private void UpdatePosition()
// // // // //     {
// // // // //         if (CurrentObjectRigidBody != null)
// // // // //         {
// // // // //             CurrentObjectRigidBody.MovePosition(Hand.position);
// // // // //             CurrentObjectRigidBody.MoveRotation(Hand.rotation);
// // // // //         }
// // // // //     }

// // // // //     [PunRPC]
// // // // //     public void ApplyKnockback(int hitPlayerViewID, Vector3 hitDirection)
// // // // //     {
// // // // //         PhotonView hitPlayerView = PhotonView.Find(hitPlayerViewID);
// // // // //         if (hitPlayerView != null)
// // // // //         {
// // // // //             PickupClass hitPlayerPickup = hitPlayerView.gameObject.GetComponent<PickupClass>();
// // // // //             if (hitPlayerPickup != null)
// // // // //             {
// // // // //                 hitPlayerPickup.photonView.RPC("PlayFallAnimation", RpcTarget.All);
// // // // //             }

// // // // //             Rigidbody playerRb = hitPlayerView.gameObject.GetComponent<Rigidbody>();
// // // // //             if (playerRb != null)
// // // // //             {
// // // // //                 Vector3 knockbackDirection = hitDirection + Vector3.up * KnockbackUpwardForce;
// // // // //                 knockbackDirection -= hitPlayerView.transform.forward * KnockbackBackwardForce;
// // // // //                 knockbackDirection.Normalize();

// // // // //                 playerRb.AddForce(knockbackDirection * KnockbackForce, ForceMode.Impulse);
// // // // //             }
// // // // //         }
// // // // //     }
// // // // // }

// // // // // public class ThrowableCollisionHandler : MonoBehaviour
// // // // // {
// // // // //     private PickupClass pickupClass;
// // // // //     private bool hasCollided = false;
// // // // //     private float collisionCooldown = 0.5f;
// // // // //     private float lastCollisionTime;

// // // // //     public void Initialize(PickupClass pickup)
// // // // //     {
// // // // //         pickupClass = pickup;
// // // // //         lastCollisionTime = -collisionCooldown;
// // // // //     }

// // // // //     private void OnCollisionEnter(Collision collision)
// // // // //     {
// // // // //         if (hasCollided || Time.time - lastCollisionTime < collisionCooldown) return;

// // // // //         Debug.Log($"Collision detected with: {collision.gameObject.name}");

// // // // //         PhotonView hitPlayerView = collision.gameObject.GetComponent<PhotonView>();
// // // // //         if (hitPlayerView != null && pickupClass != null)
// // // // //         {
// // // // //             Vector3 hitDirection = (collision.contacts[0].point - transform.position).normalized;
// // // // //             pickupClass.photonView.RPC("ApplyKnockback", RpcTarget.All, hitPlayerView.ViewID, hitDirection);

// // // // //             lastCollisionTime = Time.time;
// // // // //             hasCollided = true;
// // // // //         }
// // // // //     }
// // // // // }

// // // // using UnityEngine;
// // // // using Photon.Pun;
// // // // using System.Collections;

// // // // public class PickupClass : MonoBehaviourPunCallbacks
// // // // {
// // // //     [Header("Pickup Settings")]
// // // //     [SerializeField] private LayerMask PickupLayer;
// // // //     [SerializeField] private GameObject PlayerCamera;
// // // //     [SerializeField] private float PickupRange = 3f;
// // // //     [SerializeField] private Transform Hand;
// // // //     [SerializeField] private float ThrowingForce = 10f;

// // // //     [Header("Knockback Settings")]
// // // //     [SerializeField] private float KnockbackForce = 10f;
// // // //     [SerializeField] private float KnockbackUpwardForce = 2f;
// // // //     [SerializeField] private float KnockbackBackwardForce = 5f;

// // // //     [Header("Animation Settings")]
// // // //     [SerializeField] private Animator playerAnimator;
// // // //     [SerializeField] private float pickupAnimationDuration = 1f;
// // // //     [SerializeField] private float throwAnimationDuration = 0.5f;
// // // //     [SerializeField] private float fallAnimationDuration = 1f;
// // // //     [SerializeField] private float transitionDuration = 0.1f;
// // // //     [SerializeField] private float knockbackCooldown = 1f;

// // // //     [Header("Animation State Names")]
// // // //     [SerializeField] private string pickupStateName = "pickup";
// // // //     [SerializeField] private string throwStateName = "throw";
// // // //     [SerializeField] private string fallStateName = "fall";
// // // //     [SerializeField] private string idleStateName = "idle";

// // // //     private int pickupStateHash;
// // // //     private int throwStateHash;
// // // //     private int fallStateHash;
// // // //     private int idleStateHash;

// // // //     private Rigidbody CurrentObjectRigidBody;
// // // //     private Collider CurrentObjectCollider;
// // // //     private PhotonView photonView;
// // // //     private bool isThrown = false;
// // // //     private bool isAnimating = false;
// // // //     private bool isFalling = false;
// // // //     private float lastKnockbackTime;

// // // //     void Start()
// // // //     {
// // // //         photonView = GetComponent<PhotonView>();
// // // //         if (!playerAnimator)
// // // //         {
// // // //             playerAnimator = GetComponent<Animator>();
// // // //         }

// // // //         if (PlayerCamera == null)
// // // //         {
// // // //             PlayerCamera = Camera.main.gameObject;
// // // //         }

// // // //         pickupStateHash = Animator.StringToHash(pickupStateName);
// // // //         throwStateHash = Animator.StringToHash(throwStateName);
// // // //         fallStateHash = Animator.StringToHash(fallStateName);
// // // //         idleStateHash = Animator.StringToHash(idleStateName);
// // // //         lastKnockbackTime = -knockbackCooldown;
// // // //     }

// // // //     void Update()
// // // //     {
// // // //         if (!photonView.IsMine) return;

// // // //         HandlePickupAndDrop();
// // // //         HandleThrow();
// // // //         UpdateHeldObject();
// // // //     }

// // // //     private void HandlePickupAndDrop()
// // // //     {
// // // //         if (Input.GetKeyDown(KeyCode.E) && !isAnimating)
// // // //         {
// // // //             if (!CurrentObjectRigidBody)
// // // //             {
// // // //                 TryPickupObject();
// // // //             }
// // // //             else
// // // //             {
// // // //                 photonView.RPC("Drop", RpcTarget.All);
// // // //             }
// // // //         }
// // // //     }

// // // //     private void TryPickupObject()
// // // //     {
// // // //         Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, PickupRange, PickupLayer);

// // // //         if (nearbyObjects.Length > 0)
// // // //         {
// // // //             Collider closestObject = GetClosestObject(nearbyObjects);
// // // //             if (closestObject != null)
// // // //             {
// // // //                 PhotonView objectView = closestObject.gameObject.GetComponent<PhotonView>();
// // // //                 if (objectView != null)
// // // //                 {
// // // //                     StartCoroutine(PickupAnimationSequence(objectView.ViewID, false));
// // // //                 }
// // // //             }
// // // //         }
// // // //     }

// // // //     private Collider GetClosestObject(Collider[] objects)
// // // //     {
// // // //         Collider closestObject = null;
// // // //         float closestDistance = float.MaxValue;

// // // //         foreach (Collider obj in objects)
// // // //         {
// // // //             float distance = Vector3.Distance(transform.position, obj.transform.position);
// // // //             if (distance < closestDistance)
// // // //             {
// // // //                 closestDistance = distance;
// // // //                 closestObject = obj;
// // // //             }
// // // //         }

// // // //         return closestObject;
// // // //     }

// // // //     private void HandleThrow()
// // // //     {
// // // //         if (Input.GetKeyDown(KeyCode.Q) && CurrentObjectRigidBody && !isAnimating)
// // // //         {
// // // //             StartCoroutine(ThrowAnimationSequence());
// // // //         }
// // // //     }

// // // //     private void UpdateHeldObject()
// // // //     {
// // // //         if (CurrentObjectRigidBody && !isThrown)
// // // //         {
// // // //             photonView.RPC("UpdatePosition", RpcTarget.All);
// // // //         }
// // // //     }

// // // //     private IEnumerator PickupAnimationSequence(int objectViewID, bool isDropAndPickup)
// // // //     {
// // // //         isAnimating = true;

// // // //         photonView.RPC("PlayPickupAnimation", RpcTarget.All);
// // // //         yield return new WaitForSeconds(pickupAnimationDuration);

// // // //         if (isDropAndPickup)
// // // //         {
// // // //             photonView.RPC("DropAndPickup", RpcTarget.All, objectViewID);
// // // //         }
// // // //         else
// // // //         {
// // // //             photonView.RPC("Pickup", RpcTarget.All, objectViewID);
// // // //         }

// // // //         photonView.RPC("PlayIdleAnimation", RpcTarget.All);
// // // //         isAnimating = false;
// // // //     }

// // // //     private IEnumerator ThrowAnimationSequence()
// // // //     {
// // // //         isAnimating = true;

// // // //         photonView.RPC("PlayThrowAnimation", RpcTarget.All);
// // // //         yield return new WaitForSeconds(throwAnimationDuration * 0.5f);

// // // //         photonView.RPC("Throw", RpcTarget.All, PlayerCamera.transform.forward);
// // // //         yield return new WaitForSeconds(throwAnimationDuration * 0.5f);

// // // //         photonView.RPC("PlayIdleAnimation", RpcTarget.All);
// // // //         isAnimating = false;
// // // //     }

// // // //     [PunRPC]
// // // //     private void PlayPickupAnimation()
// // // //     {
// // // //         if (playerAnimator)
// // // //         {
// // // //             playerAnimator.ResetTrigger("ThrowTrigger");
// // // //             playerAnimator.ResetTrigger("fall");
// // // //             playerAnimator.SetTrigger("PickupTrigger");
// // // //             playerAnimator.CrossFade(pickupStateHash, transitionDuration);
// // // //         }
// // // //     }

// // // //     [PunRPC]
// // // //     private void PlayThrowAnimation()
// // // //     {
// // // //         if (playerAnimator)
// // // //         {
// // // //             playerAnimator.ResetTrigger("PickupTrigger");
// // // //             playerAnimator.ResetTrigger("fall");
// // // //             playerAnimator.SetTrigger("ThrowTrigger");
// // // //             playerAnimator.CrossFade(throwStateHash, transitionDuration);
// // // //         }
// // // //     }

// // // //     [PunRPC]
// // // //     private void PlayFallAnimation()
// // // //     {
// // // //         if (playerAnimator && !isFalling && Time.time >= lastKnockbackTime + knockbackCooldown)
// // // //         {
// // // //             isFalling = true;
// // // //             isAnimating = true;
// // // //             lastKnockbackTime = Time.time;

// // // //             playerAnimator.ResetTrigger("PickupTrigger");
// // // //             playerAnimator.ResetTrigger("ThrowTrigger");
// // // //             playerAnimator.SetTrigger("fall");
// // // //             playerAnimator.CrossFade(fallStateHash, transitionDuration);

// // // //             StartCoroutine(AutoRecoverFromFall());
// // // //         }
// // // //     }

// // // //     private IEnumerator AutoRecoverFromFall()
// // // //     {
// // // //         yield return new WaitForSeconds(fallAnimationDuration);

// // // //         if (playerAnimator)
// // // //         {
// // // //             playerAnimator.CrossFade(idleStateHash, transitionDuration);
// // // //             playerAnimator.ResetTrigger("fall");
// // // //         }

// // // //         isFalling = false;
// // // //         isAnimating = false;
// // // //     }

// // // //     [PunRPC]
// // // //     private void PlayIdleAnimation()
// // // //     {
// // // //         if (playerAnimator)
// // // //         {
// // // //             playerAnimator.CrossFade(idleStateHash, transitionDuration);
// // // //         }
// // // //     }

// // // //     [PunRPC]
// // // //     private void Pickup(int objectViewID)
// // // //     {
// // // //         GameObject pickupObject = PhotonView.Find(objectViewID).gameObject;
// // // //         SetupPickedObject(pickupObject);
// // // //     }

// // // //     [PunRPC]
// // // //     private void Drop()
// // // //     {
// // // //         if (CurrentObjectRigidBody != null)
// // // //         {
// // // //             ReleaseObject();
// // // //         }
// // // //     }

// // // //     [PunRPC]
// // // //     private void DropAndPickup(int newObjectViewID)
// // // //     {
// // // //         if (CurrentObjectRigidBody != null)
// // // //         {
// // // //             ReleaseObject();
// // // //         }

// // // //         GameObject pickupObject = PhotonView.Find(newObjectViewID).gameObject;
// // // //         SetupPickedObject(pickupObject);
// // // //     }

// // // //     private void SetupPickedObject(GameObject pickupObject)
// // // //     {
// // // //         CurrentObjectRigidBody = pickupObject.GetComponent<Rigidbody>();
// // // //         CurrentObjectCollider = pickupObject.GetComponent<Collider>();

// // // //         if (!CurrentObjectRigidBody.gameObject.GetComponent<ThrowableCollisionHandler>())
// // // //         {
// // // //             CurrentObjectRigidBody.gameObject.AddComponent<ThrowableCollisionHandler>().Initialize(this);
// // // //         }

// // // //         CurrentObjectRigidBody.isKinematic = true;
// // // //         CurrentObjectCollider.enabled = false;
// // // //         isThrown = false;
// // // //     }

// // // //     private void ReleaseObject()
// // // //     {
// // // //         CurrentObjectRigidBody.isKinematic = false;
// // // //         CurrentObjectCollider.enabled = true;
// // // //         CurrentObjectRigidBody = null;
// // // //         CurrentObjectCollider = null;
// // // //         isThrown = false;
// // // //     }

// // // //     [PunRPC]
// // // //     private void Throw(Vector3 direction)
// // // //     {
// // // //         if (CurrentObjectRigidBody != null)
// // // //         {
// // // //             CurrentObjectRigidBody.isKinematic = false;
// // // //             CurrentObjectCollider.enabled = true;
// // // //             CurrentObjectRigidBody.AddForce(direction * ThrowingForce, ForceMode.Impulse);
// // // //             isThrown = true;

// // // //             StartCoroutine(ClearObjectAfterDelay());
// // // //         }
// // // //     }

// // // //     private IEnumerator ClearObjectAfterDelay()
// // // //     {
// // // //         yield return new WaitForSeconds(0.1f);
// // // //         CurrentObjectRigidBody = null;
// // // //         CurrentObjectCollider = null;
// // // //         isThrown = false;
// // // //     }

// // // //     [PunRPC]
// // // //     private void UpdatePosition()
// // // //     {
// // // //         if (CurrentObjectRigidBody != null)
// // // //         {
// // // //             CurrentObjectRigidBody.MovePosition(Hand.position);
// // // //             CurrentObjectRigidBody.MoveRotation(Hand.rotation);
// // // //         }
// // // //     }

// // // //     [PunRPC]
// // // //     public void ApplyKnockback(int hitPlayerViewID, Vector3 hitDirection)
// // // //     {
// // // //         PhotonView hitPlayerView = PhotonView.Find(hitPlayerViewID);
// // // //         if (hitPlayerView != null)
// // // //         {
// // // //             PickupClass hitPlayerPickup = hitPlayerView.gameObject.GetComponent<PickupClass>();
// // // //             if (hitPlayerPickup != null)
// // // //             {
// // // //                 hitPlayerPickup.photonView.RPC("PlayFallAnimation", RpcTarget.All);
// // // //             }

// // // //             Rigidbody playerRb = hitPlayerView.gameObject.GetComponent<Rigidbody>();
// // // //             if (playerRb != null)
// // // //             {
// // // //                 Vector3 knockbackDirection = hitDirection + Vector3.up * KnockbackUpwardForce;
// // // //                 knockbackDirection -= hitPlayerView.transform.forward * KnockbackBackwardForce;
// // // //                 knockbackDirection.Normalize();

// // // //                 playerRb.AddForce(knockbackDirection * KnockbackForce, ForceMode.Impulse);
// // // //             }
// // // //         }
// // // //     }
// // // // }

// // // // public class ThrowableCollisionHandler : MonoBehaviour
// // // // {
// // // //     private PickupClass pickupClass;
// // // //     private bool hasCollided = false;
// // // //     private float collisionCooldown = 0.5f;
// // // //     private float lastCollisionTime;

// // // //     public void Initialize(PickupClass pickup)
// // // //     {
// // // //         pickupClass = pickup;
// // // //         lastCollisionTime = -collisionCooldown;
// // // //         hasCollided = false;
// // // //     }

// // // //     private void OnCollisionEnter(Collision collision)
// // // //     {
// // // //         if (Time.time - lastCollisionTime < collisionCooldown) return;

// // // //         PhotonView hitPlayerView = collision.gameObject.GetComponent<PhotonView>();
// // // //         if (hitPlayerView != null && pickupClass != null)
// // // //         {
// // // //             Vector3 hitDirection = (collision.contacts[0].point - transform.position).normalized;
// // // //             pickupClass.photonView.RPC("ApplyKnockback", RpcTarget.All, hitPlayerView.ViewID, hitDirection);
// // // //             lastCollisionTime = Time.time;
// // // //         }
// // // //     }
// // // // }


// // // using UnityEngine;
// // // using Photon.Pun;
// // // using System.Collections;

// // // public class PickupClass : MonoBehaviourPunCallbacks
// // // {
// // //     [Header("Pickup Settings")]
// // //     [SerializeField] private LayerMask PickupLayer;
// // //     [SerializeField] private GameObject PlayerCamera;
// // //     [SerializeField] private float PickupRange = 3f;
// // //     [SerializeField] private Transform Hand;
// // //     [SerializeField] private float ThrowingForce = 10f;

// // //     [Header("Knockback Settings")]
// // //     [SerializeField] private float KnockbackForce = 10f;
// // //     [SerializeField] private float KnockbackUpwardForce = 2f;
// // //     [SerializeField] private float KnockbackBackwardForce = 5f;
// // //     [SerializeField] private float knockbackCooldown = 1f;

// // //     [Header("Animation Settings")]
// // //     [SerializeField] private Animator playerAnimator;
// // //     [SerializeField] private float pickupAnimationDuration = 1f;
// // //     [SerializeField] private float throwAnimationDuration = 0.5f;
// // //     [SerializeField] private float fallAnimationDuration = 1f;
// // //     [SerializeField] private float transitionDuration = 0.1f;

// // //     [Header("Animation State Names")]
// // //     [SerializeField] private string pickupStateName = "pickup";
// // //     [SerializeField] private string throwStateName = "throw";
// // //     [SerializeField] private string fallStateName = "fall";
// // //     [SerializeField] private string idleStateName = "idle";

// // //     [Header("Debug")]
// // //     [SerializeField] private bool debugMode = false;

// // //     private int pickupStateHash;
// // //     private int throwStateHash;
// // //     private int fallStateHash;
// // //     private int idleStateHash;

// // //     private Rigidbody CurrentObjectRigidBody;
// // //     private Collider CurrentObjectCollider;
// // //     private PhotonView photonView;
// // //     private bool isThrown = false;
// // //     private bool isAnimating = false;
// // //     private float lastKnockbackTime;
// // //     private bool isProcessingKnockback = false;

// // //     void Start()
// // //     {
// // //         photonView = GetComponent<PhotonView>();
// // //         if (!playerAnimator)
// // //         {
// // //             playerAnimator = GetComponent<Animator>();
// // //         }

// // //         if (PlayerCamera == null)
// // //         {
// // //             PlayerCamera = Camera.main.gameObject;
// // //         }

// // //         pickupStateHash = Animator.StringToHash(pickupStateName);
// // //         throwStateHash = Animator.StringToHash(throwStateName);
// // //         fallStateHash = Animator.StringToHash(fallStateName);
// // //         idleStateHash = Animator.StringToHash(idleStateName);
// // //         lastKnockbackTime = -knockbackCooldown;
// // //     }

// // //     void Update()
// // //     {
// // //         if (!photonView.IsMine) return;

// // //         HandlePickupAndDrop();
// // //         HandleThrow();
// // //         UpdateHeldObject();
// // //     }

// // //     private void HandlePickupAndDrop()
// // //     {
// // //         if (Input.GetKeyDown(KeyCode.E) && !isAnimating && !isProcessingKnockback)
// // //         {
// // //             if (!CurrentObjectRigidBody)
// // //             {
// // //                 TryPickupObject();
// // //             }
// // //             else
// // //             {
// // //                 photonView.RPC("Drop", RpcTarget.All);
// // //             }
// // //         }
// // //     }

// // //     private void TryPickupObject()
// // //     {
// // //         Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, PickupRange, PickupLayer);

// // //         if (nearbyObjects.Length > 0)
// // //         {
// // //             Collider closestObject = GetClosestObject(nearbyObjects);
// // //             if (closestObject != null)
// // //             {
// // //                 PhotonView objectView = closestObject.gameObject.GetComponent<PhotonView>();
// // //                 if (objectView != null)
// // //                 {
// // //                     StartCoroutine(PickupAnimationSequence(objectView.ViewID, false));
// // //                 }
// // //             }
// // //         }
// // //     }

// // //     private Collider GetClosestObject(Collider[] objects)
// // //     {
// // //         Collider closestObject = null;
// // //         float closestDistance = float.MaxValue;

// // //         foreach (Collider obj in objects)
// // //         {
// // //             float distance = Vector3.Distance(transform.position, obj.transform.position);
// // //             if (distance < closestDistance)
// // //             {
// // //                 closestDistance = distance;
// // //                 closestObject = obj;
// // //             }
// // //         }

// // //         return closestObject;
// // //     }

// // //     private void HandleThrow()
// // //     {
// // //         if (Input.GetKeyDown(KeyCode.Q) && CurrentObjectRigidBody && !isAnimating && !isProcessingKnockback)
// // //         {
// // //             StartCoroutine(ThrowAnimationSequence());
// // //         }
// // //     }

// // //     private void UpdateHeldObject()
// // //     {
// // //         if (CurrentObjectRigidBody && !isThrown)
// // //         {
// // //             photonView.RPC("UpdatePosition", RpcTarget.All);
// // //         }
// // //     }

// // //     private IEnumerator PickupAnimationSequence(int objectViewID, bool isDropAndPickup)
// // //     {
// // //         isAnimating = true;

// // //         photonView.RPC("PlayPickupAnimation", RpcTarget.All);
// // //         yield return new WaitForSeconds(pickupAnimationDuration);

// // //         if (isDropAndPickup)
// // //         {
// // //             photonView.RPC("DropAndPickup", RpcTarget.All, objectViewID);
// // //         }
// // //         else
// // //         {
// // //             photonView.RPC("Pickup", RpcTarget.All, objectViewID);
// // //         }

// // //         photonView.RPC("PlayIdleAnimation", RpcTarget.All);
// // //         isAnimating = false;
// // //     }

// // //     private IEnumerator ThrowAnimationSequence()
// // //     {
// // //         isAnimating = true;

// // //         photonView.RPC("PlayThrowAnimation", RpcTarget.All);
// // //         yield return new WaitForSeconds(throwAnimationDuration * 0.5f);

// // //         photonView.RPC("Throw", RpcTarget.All, PlayerCamera.transform.forward);
// // //         yield return new WaitForSeconds(throwAnimationDuration * 0.5f);

// // //         photonView.RPC("PlayIdleAnimation", RpcTarget.All);
// // //         isAnimating = false;
// // //     }

// // //     [PunRPC]
// // //     private void PlayPickupAnimation()
// // //     {
// // //         if (playerAnimator)
// // //         {
// // //             playerAnimator.ResetTrigger("ThrowTrigger");
// // //             playerAnimator.ResetTrigger("fall");
// // //             playerAnimator.SetTrigger("PickupTrigger");
// // //             playerAnimator.CrossFade(pickupStateHash, transitionDuration);
// // //         }
// // //     }

// // //     [PunRPC]
// // //     private void PlayThrowAnimation()
// // //     {
// // //         if (playerAnimator)
// // //         {
// // //             playerAnimator.ResetTrigger("PickupTrigger");
// // //             playerAnimator.ResetTrigger("fall");
// // //             playerAnimator.SetTrigger("ThrowTrigger");
// // //             playerAnimator.CrossFade(throwStateHash, transitionDuration);
// // //         }
// // //     }

// // //     [PunRPC]
// // //     private void PlayFallAnimation()
// // //     {
// // //         if (debugMode) Debug.Log($"PlayFallAnimation called. isProcessingKnockback: {isProcessingKnockback}");

// // //         if (playerAnimator && !isProcessingKnockback && Time.time >= lastKnockbackTime + knockbackCooldown)
// // //         {
// // //             if (debugMode) Debug.Log("Starting fall animation sequence");

// // //             isProcessingKnockback = true;
// // //             lastKnockbackTime = Time.time;

// // //             // Reset all animation triggers first
// // //             playerAnimator.ResetTrigger("PickupTrigger");
// // //             playerAnimator.ResetTrigger("ThrowTrigger");
// // //             playerAnimator.ResetTrigger("fall");

// // //             // Set the new trigger and play animation
// // //             playerAnimator.SetTrigger("fall");
// // //             playerAnimator.CrossFade(fallStateHash, transitionDuration);

// // //             StartCoroutine(AutoRecoverFromFall());
// // //         }
// // //         else if (debugMode)
// // //         {
// // //             Debug.Log($"Skipped fall animation. Time since last: {Time.time - lastKnockbackTime}");
// // //         }
// // //     }

// // //     private IEnumerator AutoRecoverFromFall()
// // //     {
// // //         if (debugMode) Debug.Log("Starting fall recovery sequence");

// // //         yield return new WaitForSeconds(fallAnimationDuration);

// // //         if (playerAnimator)
// // //         {
// // //             playerAnimator.ResetTrigger("fall");
// // //             playerAnimator.CrossFade(idleStateHash, transitionDuration);
// // //         }

// // //         isProcessingKnockback = false;
// // //         if (debugMode) Debug.Log("Fall recovery complete");
// // //     }

// // //     [PunRPC]
// // //     private void PlayIdleAnimation()
// // //     {
// // //         if (playerAnimator)
// // //         {
// // //             playerAnimator.CrossFade(idleStateHash, transitionDuration);
// // //         }
// // //     }

// // //     [PunRPC]
// // //     private void Pickup(int objectViewID)
// // //     {
// // //         GameObject pickupObject = PhotonView.Find(objectViewID).gameObject;
// // //         SetupPickedObject(pickupObject);
// // //     }

// // //     [PunRPC]
// // //     private void Drop()
// // //     {
// // //         if (CurrentObjectRigidBody != null)
// // //         {
// // //             ReleaseObject();
// // //         }
// // //     }

// // //     [PunRPC]
// // //     private void DropAndPickup(int newObjectViewID)
// // //     {
// // //         if (CurrentObjectRigidBody != null)
// // //         {
// // //             ReleaseObject();
// // //         }

// // //         GameObject pickupObject = PhotonView.Find(newObjectViewID).gameObject;
// // //         SetupPickedObject(pickupObject);
// // //     }

// // //     private void SetupPickedObject(GameObject pickupObject)
// // //     {
// // //         CurrentObjectRigidBody = pickupObject.GetComponent<Rigidbody>();
// // //         CurrentObjectCollider = pickupObject.GetComponent<Collider>();

// // //         if (!CurrentObjectRigidBody.gameObject.GetComponent<ThrowableCollisionHandler>())
// // //         {
// // //             CurrentObjectRigidBody.gameObject.AddComponent<ThrowableCollisionHandler>().Initialize(this);
// // //         }

// // //         CurrentObjectRigidBody.isKinematic = true;
// // //         CurrentObjectCollider.enabled = false;
// // //         isThrown = false;
// // //     }

// // //     private void ReleaseObject()
// // //     {
// // //         CurrentObjectRigidBody.isKinematic = false;
// // //         CurrentObjectCollider.enabled = true;
// // //         CurrentObjectRigidBody = null;
// // //         CurrentObjectCollider = null;
// // //         isThrown = false;
// // //     }

// // //     [PunRPC]
// // //     private void Throw(Vector3 direction)
// // //     {
// // //         if (CurrentObjectRigidBody != null)
// // //         {
// // //             CurrentObjectRigidBody.isKinematic = false;
// // //             CurrentObjectCollider.enabled = true;
// // //             CurrentObjectRigidBody.AddForce(direction * ThrowingForce, ForceMode.Impulse);
// // //             isThrown = true;

// // //             StartCoroutine(ClearObjectAfterDelay());
// // //         }
// // //     }

// // //     private IEnumerator ClearObjectAfterDelay()
// // //     {
// // //         yield return new WaitForSeconds(0.1f);
// // //         CurrentObjectRigidBody = null;
// // //         CurrentObjectCollider = null;
// // //         isThrown = false;
// // //     }

// // //     [PunRPC]
// // //     private void UpdatePosition()
// // //     {
// // //         if (CurrentObjectRigidBody != null)
// // //         {
// // //             CurrentObjectRigidBody.MovePosition(Hand.position);
// // //             CurrentObjectRigidBody.MoveRotation(Hand.rotation);
// // //         }
// // //     }

// // //     [PunRPC]
// // //     public void ApplyKnockback(int hitPlayerViewID, Vector3 hitDirection)
// // //     {
// // //         if (debugMode) Debug.Log($"ApplyKnockback called for player: {hitPlayerViewID}");

// // //         PhotonView hitPlayerView = PhotonView.Find(hitPlayerViewID);
// // //         if (hitPlayerView != null)
// // //         {
// // //             PickupClass hitPlayerPickup = hitPlayerView.gameObject.GetComponent<PickupClass>();
// // //             if (hitPlayerPickup != null && !hitPlayerPickup.isProcessingKnockback)
// // //             {
// // //                 if (debugMode) Debug.Log("Applying knockback and playing fall animation");

// // //                 hitPlayerPickup.photonView.RPC("PlayFallAnimation", RpcTarget.All);

// // //                 Rigidbody playerRb = hitPlayerView.gameObject.GetComponent<Rigidbody>();
// // //                 if (playerRb != null)
// // //                 {
// // //                     Vector3 knockbackDirection = hitDirection + Vector3.up * KnockbackUpwardForce;
// // //                     knockbackDirection -= hitPlayerView.transform.forward * KnockbackBackwardForce;
// // //                     knockbackDirection.Normalize();

// // //                     playerRb.AddForce(knockbackDirection * KnockbackForce, ForceMode.Impulse);
// // //                 }
// // //             }
// // //             else if (debugMode)
// // //             {
// // //                 Debug.Log($"Knockback skipped - already processing: {hitPlayerPickup?.isProcessingKnockback}");
// // //             }
// // //         }
// // //     }
// // // }

// // // public class ThrowableCollisionHandler : MonoBehaviour
// // // {
// // //     private PickupClass pickupClass;
// // //     private float collisionCooldown = 0.5f;
// // //     private float lastCollisionTime;

// // //     public void Initialize(PickupClass pickup)
// // //     {
// // //         pickupClass = pickup;
// // //         lastCollisionTime = -collisionCooldown;
// // //     }

// // //     private void OnCollisionEnter(Collision collision)
// // //     {
// // //         if (Time.time - lastCollisionTime < collisionCooldown)
// // //         {
// // //             return;
// // //         }

// // //         PhotonView hitPlayerView = collision.gameObject.GetComponent<PhotonView>();
// // //         if (hitPlayerView != null && pickupClass != null)
// // //         {
// // //             Vector3 hitDirection = (collision.contacts[0].point - transform.position).normalized;
// // //             lastCollisionTime = Time.time;
// // //             pickupClass.photonView.RPC("ApplyKnockback", RpcTarget.All, hitPlayerView.ViewID, hitDirection);
// // //         }
// // //     }
// // // }

// // using UnityEngine;
// // using Photon.Pun;
// // using System.Collections;

// // public class PickupClass : MonoBehaviourPunCallbacks
// // {
// //     [Header("Pickup Settings")]
// //     [SerializeField] private LayerMask PickupLayer;
// //     [SerializeField] private GameObject PlayerCamera;
// //     [SerializeField] private float PickupRange = 3f;
// //     [SerializeField] private Transform Hand;
// //     [SerializeField] private float ThrowingForce = 10f;

// //     [Header("VFX Settings")]
// //     public GameObject HitVFXPrefab; // Prefab for hit visual effects

// //     [Header("Knockback Settings")]
// //     [SerializeField] private float KnockbackForce = 10f;
// //     [SerializeField] private float KnockbackUpwardForce = 2f;
// //     [SerializeField] private float KnockbackBackwardForce = 5f;
// //     [SerializeField] private float knockbackCooldown = 1f;

// //     [Header("Animation Settings")]
// //     [SerializeField] private Animator playerAnimator;
// //     [SerializeField] private float pickupAnimationDuration = 1f;
// //     [SerializeField] private float throwAnimationDuration = 0.5f;
// //     [SerializeField] private float fallAnimationDuration = 1f;
// //     [SerializeField] private float transitionDuration = 0.1f;

// //     [Header("Animation State Names")]
// //     [SerializeField] private string pickupStateName = "pickup";
// //     [SerializeField] private string throwStateName = "throw";
// //     [SerializeField] private string fallStateName = "fall";
// //     [SerializeField] private string idleStateName = "idle";

// //     [Header("Debug")]
// //     [SerializeField] private bool debugMode = false;

// //     private int pickupStateHash;
// //     private int throwStateHash;
// //     private int fallStateHash;
// //     private int idleStateHash;

// //     private Rigidbody CurrentObjectRigidBody;
// //     private Collider CurrentObjectCollider;
// //     private PhotonView photonView;
// //     private bool isThrown = false;
// //     private bool isAnimating = false;
// //     private float lastKnockbackTime;
// //     private bool isProcessingKnockback = false;

// //     void Start()
// //     {
// //         photonView = GetComponent<PhotonView>();
// //         if (!playerAnimator)
// //         {
// //             playerAnimator = GetComponent<Animator>();
// //         }

// //         if (PlayerCamera == null)
// //         {
// //             PlayerCamera = Camera.main.gameObject;
// //         }

// //         pickupStateHash = Animator.StringToHash(pickupStateName);
// //         throwStateHash = Animator.StringToHash(throwStateName);
// //         fallStateHash = Animator.StringToHash(fallStateName);
// //         idleStateHash = Animator.StringToHash(idleStateName);
// //         lastKnockbackTime = -knockbackCooldown;
// //     }

// //     void Update()
// //     {
// //         if (!photonView.IsMine) return;

// //         HandlePickupAndDrop();
// //         HandleThrow();
// //         UpdateHeldObject();
// //     }

// //     private void HandlePickupAndDrop()
// //     {
// //         if (Input.GetKeyDown(KeyCode.E) && !isAnimating && !isProcessingKnockback)
// //         {
// //             if (!CurrentObjectRigidBody)
// //             {
// //                 TryPickupObject();
// //             }
// //             else
// //             {
// //                 photonView.RPC("Drop", RpcTarget.All);
// //             }
// //         }
// //     }

// //     private void TryPickupObject()
// //     {
// //         Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, PickupRange, PickupLayer);

// //         if (nearbyObjects.Length > 0)
// //         {
// //             Collider closestObject = GetClosestObject(nearbyObjects);
// //             if (closestObject != null)
// //             {
// //                 PhotonView objectView = closestObject.gameObject.GetComponent<PhotonView>();
// //                 if (objectView != null)
// //                 {
// //                     StartCoroutine(PickupAnimationSequence(objectView.ViewID, false));
// //                 }
// //             }
// //         }
// //     }

// //     private Collider GetClosestObject(Collider[] objects)
// //     {
// //         Collider closestObject = null;
// //         float closestDistance = float.MaxValue;

// //         foreach (Collider obj in objects)
// //         {
// //             float distance = Vector3.Distance(transform.position, obj.transform.position);
// //             if (distance < closestDistance)
// //             {
// //                 closestDistance = distance;
// //                 closestObject = obj;
// //             }
// //         }

// //         return closestObject;
// //     }

// //     private void HandleThrow()
// //     {
// //         if (Input.GetKeyDown(KeyCode.Q) && CurrentObjectRigidBody && !isAnimating && !isProcessingKnockback)
// //         {
// //             StartCoroutine(ThrowAnimationSequence());
// //         }
// //     }

// //     private void UpdateHeldObject()
// //     {
// //         if (CurrentObjectRigidBody && !isThrown)
// //         {
// //             photonView.RPC("UpdatePosition", RpcTarget.All);
// //         }
// //     }

// //     private IEnumerator PickupAnimationSequence(int objectViewID, bool isDropAndPickup)
// //     {
// //         isAnimating = true;

// //         photonView.RPC("PlayPickupAnimation", RpcTarget.All);
// //         yield return new WaitForSeconds(pickupAnimationDuration);

// //         if (isDropAndPickup)
// //         {
// //             photonView.RPC("DropAndPickup", RpcTarget.All, objectViewID);
// //         }
// //         else
// //         {
// //             photonView.RPC("Pickup", RpcTarget.All, objectViewID);
// //         }

// //         photonView.RPC("PlayIdleAnimation", RpcTarget.All);
// //         isAnimating = false;
// //     }

// //     private IEnumerator ThrowAnimationSequence()
// //     {
// //         isAnimating = true;

// //         photonView.RPC("PlayThrowAnimation", RpcTarget.All);
// //         yield return new WaitForSeconds(throwAnimationDuration * 0.5f);

// //         photonView.RPC("Throw", RpcTarget.All, PlayerCamera.transform.forward);
// //         yield return new WaitForSeconds(throwAnimationDuration * 0.5f);

// //         photonView.RPC("PlayIdleAnimation", RpcTarget.All);
// //         isAnimating = false;
// //     }

// //     [PunRPC]
// //     private void PlayPickupAnimation()
// //     {
// //         if (playerAnimator)
// //         {
// //             playerAnimator.ResetTrigger("ThrowTrigger");
// //             playerAnimator.ResetTrigger("fall");
// //             playerAnimator.SetTrigger("PickupTrigger");
// //             playerAnimator.CrossFade(pickupStateHash, transitionDuration);
// //         }
// //     }

// //     [PunRPC]
// //     private void PlayThrowAnimation()
// //     {
// //         if (playerAnimator)
// //         {
// //             playerAnimator.ResetTrigger("PickupTrigger");
// //             playerAnimator.ResetTrigger("fall");
// //             playerAnimator.SetTrigger("ThrowTrigger");
// //             playerAnimator.CrossFade(throwStateHash, transitionDuration);
// //         }
// //     }

// //     [PunRPC]
// //     private void PlayFallAnimation()
// //     {
// //         if (debugMode) Debug.Log($"PlayFallAnimation called. isProcessingKnockback: {isProcessingKnockback}");

// //         if (playerAnimator && !isProcessingKnockback && Time.time >= lastKnockbackTime + knockbackCooldown)
// //         {
// //             if (debugMode) Debug.Log("Starting fall animation sequence");

// //             isProcessingKnockback = true;
// //             lastKnockbackTime = Time.time;

// //             playerAnimator.ResetTrigger("PickupTrigger");
// //             playerAnimator.ResetTrigger("ThrowTrigger");
// //             playerAnimator.ResetTrigger("fall");

// //             playerAnimator.SetTrigger("fall");
// //             playerAnimator.CrossFade(fallStateHash, transitionDuration);

// //             StartCoroutine(AutoRecoverFromFall());
// //         }
// //         else if (debugMode)
// //         {
// //             Debug.Log($"Skipped fall animation. Time since last: {Time.time - lastKnockbackTime}");
// //         }
// //     }

// //     private IEnumerator AutoRecoverFromFall()
// //     {
// //         if (debugMode) Debug.Log("Starting fall recovery sequence");

// //         yield return new WaitForSeconds(fallAnimationDuration);

// //         if (playerAnimator)
// //         {
// //             playerAnimator.ResetTrigger("fall");
// //             playerAnimator.CrossFade(idleStateHash, transitionDuration);
// //         }

// //         isProcessingKnockback = false;
// //         if (debugMode) Debug.Log("Fall recovery complete");
// //     }

// //     [PunRPC]
// //     private void PlayIdleAnimation()
// //     {
// //         if (playerAnimator)
// //         {
// //             playerAnimator.CrossFade(idleStateHash, transitionDuration);
// //         }
// //     }

// //     [PunRPC]
// //     private void Pickup(int objectViewID)
// //     {
// //         GameObject pickupObject = PhotonView.Find(objectViewID).gameObject;
// //         SetupPickedObject(pickupObject);
// //     }

// //     [PunRPC]
// //     private void Drop()
// //     {
// //         if (CurrentObjectRigidBody != null)
// //         {
// //             ReleaseObject();
// //         }
// //     }

// //     [PunRPC]
// //     private void DropAndPickup(int newObjectViewID)
// //     {
// //         if (CurrentObjectRigidBody != null)
// //         {
// //             ReleaseObject();
// //         }

// //         GameObject pickupObject = PhotonView.Find(newObjectViewID).gameObject;
// //         SetupPickedObject(pickupObject);
// //     }

// //     private void SetupPickedObject(GameObject pickupObject)
// //     {
// //         CurrentObjectRigidBody = pickupObject.GetComponent<Rigidbody>();
// //         CurrentObjectCollider = pickupObject.GetComponent<Collider>();

// //         if (!CurrentObjectRigidBody.gameObject.GetComponent<ThrowableCollisionHandler>())
// //         {
// //             CurrentObjectRigidBody.gameObject.AddComponent<ThrowableCollisionHandler>().Initialize(this);
// //         }

// //         CurrentObjectRigidBody.isKinematic = true;
// //         CurrentObjectCollider.enabled = false;
// //         isThrown = false;
// //     }

// //     private void ReleaseObject()
// //     {
// //         CurrentObjectRigidBody.isKinematic = false;
// //         CurrentObjectCollider.enabled = true;
// //         CurrentObjectRigidBody = null;
// //         CurrentObjectCollider = null;
// //         isThrown = false;
// //     }

// //     [PunRPC]
// //     private void Throw(Vector3 direction)
// //     {
// //         if (CurrentObjectRigidBody != null)
// //         {
// //             CurrentObjectRigidBody.isKinematic = false;
// //             CurrentObjectCollider.enabled = true;
// //             CurrentObjectRigidBody.AddForce(direction * ThrowingForce, ForceMode.Impulse);
// //             isThrown = true;

// //             StartCoroutine(ClearObjectAfterDelay());
// //         }
// //     }

// //     private IEnumerator ClearObjectAfterDelay()
// //     {
// //         yield return new WaitForSeconds(0.1f);
// //         CurrentObjectRigidBody = null;
// //         CurrentObjectCollider = null;
// //         isThrown = false;
// //     }

// //     [PunRPC]
// //     private void UpdatePosition()
// //     {
// //         if (CurrentObjectRigidBody != null)
// //         {
// //             CurrentObjectRigidBody.MovePosition(Hand.position);
// //             CurrentObjectRigidBody.MoveRotation(Hand.rotation);
// //         }
// //     }

// //     [PunRPC]
// //     private void SpawnHitVFX(Vector3 hitPosition)
// //     {
// //         if (HitVFXPrefab != null)
// //         {
// //             PhotonNetwork.Instantiate(HitVFXPrefab.name, hitPosition, Quaternion.identity);
// //         }
// //         else if (debugMode)
// //         {
// //             Debug.LogWarning("HitVFXPrefab is not assigned!");
// //         }
// //     }

// //     [PunRPC]
// //     public void ApplyKnockback(int hitPlayerViewID, Vector3 hitDirection, Vector3 hitPosition)
// //     {
// //         if (debugMode) Debug.Log($"ApplyKnockback called for player: {hitPlayerViewID}");

// //         PhotonView hitPlayerView = PhotonView.Find(hitPlayerViewID);
// //         if (hitPlayerView != null)
// //         {
// //             PickupClass hitPlayerPickup = hitPlayerView.gameObject.GetComponent<PickupClass>();
// //             if (hitPlayerPickup != null && !hitPlayerPickup.isProcessingKnockback)
// //             {
// //                 if (debugMode) Debug.Log("Applying knockback and playing fall animation");

// //                 hitPlayerPickup.photonView.RPC("PlayFallAnimation", RpcTarget.All);
// //                 hitPlayerPickup.photonView.RPC("SpawnHitVFX", RpcTarget.All, hitPosition);

// //                 Rigidbody playerRb = hitPlayerView.gameObject.GetComponent<Rigidbody>();
// //                 if (playerRb != null)
// //                 {
// //                     Vector3 knockbackDirection = hitDirection + Vector3.up * KnockbackUpwardForce;
// //                     knockbackDirection -= hitPlayerView.transform.forward * KnockbackBackwardForce;
// //                     knockbackDirection.Normalize();

// //                     playerRb.AddForce(knockbackDirection * KnockbackForce, ForceMode.Impulse);
// //                 }
// //             }
// //             else if (debugMode)
// //             {
// //                 Debug.Log($"Knockback skipped - already processing: {hitPlayerPickup?.isProcessingKnockback}");
// //             }
// //         }
// //     }
// // }

// // public class ThrowableCollisionHandler : MonoBehaviour
// // {
// //     private PickupClass pickupClass;
// //     private float collisionCooldown = 0.5f;
// //     private float lastCollisionTime;

// //     public void Initialize(PickupClass pickup)
// //     {
// //         pickupClass = pickup;
// //         lastCollisionTime = -collisionCooldown;
// //     }

// //     private void OnCollisionEnter(Collision collision)
// //     {
// //         if (Time.time - lastCollisionTime < collisionCooldown)
// //         {
// //             return;
// //         }

// //         PhotonView hitPlayerView = collision.gameObject.GetComponent<PhotonView>();
// //         if (hitPlayerView != null && pickupClass != null)
// //         {
// //             Vector3 hitPoint = collision.contacts[0].point;
// //             Vector3 hitDirection = (hitPoint - transform.position).normalized;
// //             lastCollisionTime = Time.time;
// //             pickupClass.photonView.RPC("ApplyKnockback", RpcTarget.All, hitPlayerView.ViewID, hitDirection, hitPoint);
// //         }
// //     }
// // }

// using UnityEngine;
// using Photon.Pun;
// using System.Collections;

// public class PickupClass : MonoBehaviourPunCallbacks, IPunObservable
// {
//     [Header("Pickup Settings")]
//     [SerializeField] private LayerMask PickupLayer;
//     [SerializeField] private GameObject PlayerCamera;
//     [SerializeField] private float PickupRange = 3f;
//     [SerializeField] private Transform Hand;
//     [SerializeField] private float ThrowingForce = 10f;

//     [Header("VFX Settings")]
//     public GameObject HitVFXPrefab;

//     [Header("Knockback Settings")]
//     [SerializeField] private float KnockbackForce = 10f;
//     [SerializeField] private float KnockbackUpwardForce = 2f;
//     [SerializeField] private float KnockbackBackwardForce = 5f;
//     [SerializeField] private float knockbackCooldown = 1f;

//     [Header("Animation Settings")]
//     [SerializeField] private Animator playerAnimator;
//     [SerializeField] private float pickupAnimationDuration = 1f;
//     [SerializeField] private float throwAnimationDuration = 0.5f;
//     [SerializeField] private float fallAnimationDuration = 1f;
//     [SerializeField] private float transitionDuration = 0.1f;

//     [Header("Debug")]
//     [SerializeField] private bool debugMode = false;

//     private int pickupTriggerHash;
//     private int throwTriggerHash;
//     private int fallTriggerHash;
//     private int idleTriggerHash;

//     private Rigidbody CurrentObjectRigidBody;
//     private Collider CurrentObjectCollider;
//     private PhotonView photonView;
//     private bool isThrown = false;
//     private bool isAnimating = false;
//     private float lastKnockbackTime;
//     private bool isProcessingKnockback = false;

//     void Start()
//     {
//         photonView = GetComponent<PhotonView>();
//         if (!playerAnimator)
//         {
//             playerAnimator = GetComponent<Animator>();
//         }

//         if (PlayerCamera == null)
//         {
//             PlayerCamera = Camera.main.gameObject;
//         }

//         // Set up animation trigger hashes
//         pickupTriggerHash = Animator.StringToHash("PickupTrigger");
//         throwTriggerHash = Animator.StringToHash("ThrowTrigger");
//         fallTriggerHash = Animator.StringToHash("fall");
//         idleTriggerHash = Animator.StringToHash("idle");

//         lastKnockbackTime = -knockbackCooldown;
//     }

//     void Update()
//     {
//         if (!photonView.IsMine) return;

//         HandlePickupAndDrop();
//         HandleThrow();
//         UpdateHeldObject();
//     }

//     private void HandlePickupAndDrop()
//     {
//         if (Input.GetKeyDown(KeyCode.E) && !isAnimating && !isProcessingKnockback)
//         {
//             if (!CurrentObjectRigidBody)
//             {
//                 TryPickupObject();
//             }
//             else
//             {
//                 photonView.RPC("DropObject", RpcTarget.All);
//             }
//         }
//     }

//     private void HandleThrow()
//     {
//         if (Input.GetKeyDown(KeyCode.Q) && CurrentObjectRigidBody && !isAnimating && !isProcessingKnockback)
//         {
//             StartCoroutine(ThrowSequence());
//         }
//     }

//     private void UpdateHeldObject()
//     {
//         if (CurrentObjectRigidBody && !isThrown)
//         {
//             photonView.RPC("UpdateObjectPosition", RpcTarget.All);
//         }
//     }

//     private void TryPickupObject()
//     {
//         Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, PickupRange, PickupLayer);

//         if (nearbyObjects.Length > 0)
//         {
//             Collider closestObject = GetClosestObject(nearbyObjects);
//             if (closestObject != null)
//             {
//                 PhotonView objectView = closestObject.gameObject.GetComponent<PhotonView>();
//                 if (objectView != null)
//                 {
//                     StartCoroutine(PickupSequence(objectView.ViewID));
//                 }
//             }
//         }
//     }

//     private Collider GetClosestObject(Collider[] objects)
//     {
//         Collider closestObject = null;
//         float closestDistance = float.MaxValue;

//         foreach (Collider obj in objects)
//         {
//             float distance = Vector3.Distance(transform.position, obj.transform.position);
//             if (distance < closestDistance)
//             {
//                 closestDistance = distance;
//                 closestObject = obj;
//             }
//         }

//         return closestObject;
//     }

//     private IEnumerator PickupSequence(int objectViewID)
//     {
//         isAnimating = true;
//         photonView.RPC("TriggerPickupAnimation", RpcTarget.All);
//         yield return new WaitForSeconds(pickupAnimationDuration);
//         photonView.RPC("PickupObject", RpcTarget.All, objectViewID);
//         isAnimating = false;
//     }

//     private IEnumerator ThrowSequence()
//     {
//         isAnimating = true;
//         photonView.RPC("TriggerThrowAnimation", RpcTarget.All);
//         yield return new WaitForSeconds(throwAnimationDuration * 0.5f);
//         photonView.RPC("ThrowObject", RpcTarget.All, PlayerCamera.transform.forward);
//         yield return new WaitForSeconds(throwAnimationDuration * 0.5f);
//         isAnimating = false;
//     }

//     [PunRPC]
//     private void TriggerPickupAnimation()
//     {
//         if (playerAnimator)
//         {
//             playerAnimator.SetTrigger(pickupTriggerHash);
//         }
//     }

//     [PunRPC]
//     private void TriggerThrowAnimation()
//     {
//         if (playerAnimator)
//         {
//             playerAnimator.SetTrigger(throwTriggerHash);
//         }
//     }

//     [PunRPC]
//     private void TriggerFallAnimation()
//     {
//         if (playerAnimator && !isProcessingKnockback && Time.time >= lastKnockbackTime + knockbackCooldown)
//         {
//             isProcessingKnockback = true;
//             lastKnockbackTime = Time.time;
//             playerAnimator.SetTrigger(fallTriggerHash);
//             StartCoroutine(RecoverFromFall());
//         }
//     }

//     private IEnumerator RecoverFromFall()
//     {
//         yield return new WaitForSeconds(fallAnimationDuration);
//         isProcessingKnockback = false;
//         if (playerAnimator)
//         {
//             playerAnimator.SetTrigger(idleTriggerHash);
//         }
//     }

//     [PunRPC]
//     private void PickupObject(int objectViewID)
//     {
//         GameObject pickupObject = PhotonView.Find(objectViewID).gameObject;
//         CurrentObjectRigidBody = pickupObject.GetComponent<Rigidbody>();
//         CurrentObjectCollider = pickupObject.GetComponent<Collider>();

//         if (!CurrentObjectRigidBody.gameObject.GetComponent<ThrowableCollisionHandler>())
//         {
//             CurrentObjectRigidBody.gameObject.AddComponent<ThrowableCollisionHandler>().Initialize(this);
//         }

//         CurrentObjectRigidBody.isKinematic = true;
//         CurrentObjectCollider.enabled = false;
//         isThrown = false;
//     }

//     [PunRPC]
//     private void DropObject()
//     {
//         if (CurrentObjectRigidBody != null)
//         {
//             CurrentObjectRigidBody.isKinematic = false;
//             CurrentObjectCollider.enabled = true;
//             CurrentObjectRigidBody = null;
//             CurrentObjectCollider = null;
//             isThrown = false;
//         }
//     }

//     [PunRPC]
//     private void ThrowObject(Vector3 direction)
//     {
//         if (CurrentObjectRigidBody != null)
//         {
//             CurrentObjectRigidBody.isKinematic = false;
//             CurrentObjectCollider.enabled = true;
//             CurrentObjectRigidBody.AddForce(direction * ThrowingForce, ForceMode.Impulse);
//             isThrown = true;
//             StartCoroutine(ClearObjectAfterDelay());
//         }
//     }

//     private IEnumerator ClearObjectAfterDelay()
//     {
//         yield return new WaitForSeconds(0.1f);
//         CurrentObjectRigidBody = null;
//         CurrentObjectCollider = null;
//         isThrown = false;
//     }

//     [PunRPC]
//     private void UpdateObjectPosition()
//     {
//         if (CurrentObjectRigidBody != null)
//         {
//             CurrentObjectRigidBody.MovePosition(Hand.position);
//             CurrentObjectRigidBody.MoveRotation(Hand.rotation);
//         }
//     }

//     [PunRPC]
//     private void SpawnHitVFX(Vector3 hitPosition)
//     {
//         if (HitVFXPrefab != null)
//         {
//             PhotonNetwork.Instantiate(HitVFXPrefab.name, hitPosition, Quaternion.identity);
//         }
//     }

//     [PunRPC]
//     public void ApplyKnockback(int hitPlayerViewID, Vector3 hitDirection, Vector3 hitPosition)
//     {
//         PhotonView hitPlayerView = PhotonView.Find(hitPlayerViewID);
//         if (hitPlayerView != null)
//         {
//             PickupClass hitPlayerPickup = hitPlayerView.gameObject.GetComponent<PickupClass>();
//             if (hitPlayerPickup != null && !hitPlayerPickup.isProcessingKnockback)
//             {
//                 hitPlayerPickup.photonView.RPC("TriggerFallAnimation", RpcTarget.All);
//                 hitPlayerPickup.photonView.RPC("SpawnHitVFX", RpcTarget.All, hitPosition);

//                 Rigidbody playerRb = hitPlayerView.gameObject.GetComponent<Rigidbody>();
//                 if (playerRb != null)
//                 {
//                     Vector3 knockbackDirection = hitDirection + Vector3.up * KnockbackUpwardForce;
//                     knockbackDirection -= hitPlayerView.transform.forward * KnockbackBackwardForce;
//                     knockbackDirection.Normalize();
//                     playerRb.AddForce(knockbackDirection * KnockbackForce, ForceMode.Impulse);
//                 }
//             }
//         }
//     }

//     public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
//     {
//         // Implement if needed for network synchronization
//     }
// }

// public class ThrowableCollisionHandler : MonoBehaviour
// {
//     private PickupClass pickupClass;
//     private float collisionCooldown = 0.5f;
//     private float lastCollisionTime;

//     public void Initialize(PickupClass pickup)
//     {
//         pickupClass = pickup;
//         lastCollisionTime = -collisionCooldown;
//     }

//     private void OnCollisionEnter(Collision collision)
//     {
//         if (Time.time - lastCollisionTime < collisionCooldown) return;

//         PhotonView hitPlayerView = collision.gameObject.GetComponent<PhotonView>();
//         if (hitPlayerView != null && pickupClass != null)
//         {
//             Vector3 hitPoint = collision.contacts[0].point;
//             Vector3 hitDirection = (hitPoint - transform.position).normalized;
//             lastCollisionTime = Time.time;
//             pickupClass.photonView.RPC("ApplyKnockback", RpcTarget.All, hitPlayerView.ViewID, hitDirection, hitPoint);
//         }
//     }
// }


using UnityEngine;
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

    void Update()
    {
        if (!photonView.IsMine) return;

        HandlePickupAndDrop();
        HandleThrow();
        UpdateHeldObject();
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


    private void TryPickupObject()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, PickupRange, PickupLayer);

        if (nearbyObjects.Length > 0)
        {
            Collider closestObject = GetClosestObject(nearbyObjects);
            if (closestObject != null)
            {
                PhotonView objectView = closestObject.gameObject.GetComponent<PhotonView>();
                if (objectView != null)
                {
                    StartCoroutine(PickupSequence(objectView.ViewID));
                }
            }
        }
    }

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
