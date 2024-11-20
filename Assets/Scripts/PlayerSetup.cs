// using UnityEngine;
// using Photon.Pun;
// using TextMeshPro = TMPro.TextMeshPro;

// public class PlayerSetup : MonoBehaviourPunCallbacks
// {
//     public PlayerMovement movement;
//     public GameObject playerCamera;
//     private PhotonView photonView;

//     public string nickname;
//     public TextMeshPro nicknameText;

//     void Awake()
//     {
//         photonView = GetComponent<PhotonView>();

//         // Disable movement and camera by default
//         movement.enabled = false;
//         if (playerCamera != null)
//             playerCamera.SetActive(false);

//         // If this is our local player, enable controls
//         if (photonView.IsMine)
//         {
//             IsLocalPlayer();
//         }
//     }

//     public void IsLocalPlayer()
//     {
//         movement.enabled = true;
//         if (playerCamera != null)
//             playerCamera.SetActive(true);
//     }
//     [PunRPC]
//     public void SetNickname(string _name)
//     {
//         // Debug.Log("Setting nickname to: " + _name); 
//         nickname = _name;
//         // PhotonNetwork.NickName = nickName;
//         nicknameText.text = nickname;
//     }
//     public void Update()
//     {
//         // Debug.Log("Nickname: " + nickname);
//     }


// }


using UnityEngine;
using Photon.Pun;
using TextMeshPro = TMPro.TextMeshPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public PlayerMovement movement;
    public GameObject playerCamera;
    private PhotonView photonView;

    public string nickname;
    public TextMeshPro nicknameText;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();

        // Disable movement and camera by default
        movement.enabled = false;
        if (playerCamera != null)
            playerCamera.SetActive(false);

        // If this is our local player, enable controls
        if (photonView.IsMine)
        {
            IsLocalPlayer();
            // Hide your own nickname
            if (nicknameText != null)
                nicknameText.enabled = false;
        }
        else
        {
            // For other players, show their nickname
            if (nicknameText != null)
                nicknameText.enabled = true;
        }
    }

    public void IsLocalPlayer()
    {
        movement.enabled = true;
        if (playerCamera != null)
            playerCamera.SetActive(true);
    }

    [PunRPC]
    public void SetNickname(string _name)
    {
        nickname = _name;

        // Only set text if it's another player's nickname
        if (nicknameText != null && !photonView.IsMine)
        {
            nicknameText.text = nickname;
        }
    }
}