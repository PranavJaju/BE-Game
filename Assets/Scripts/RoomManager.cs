// using UnityEngine;
// using Photon.Pun;
// using Photon.Realtime;

// public class RoomManager : MonoBehaviourPunCallbacks
// {
//     public static RoomManager instance;
//     public GameObject _player;
//     public GameObject playerPrefab;
//     public Transform spawnPoint;

//     [Space]
//     public GameObject roomCam;

//     void Awake()
//     {
//         instance = this;
//     }
//     void Start()
//     {
//         Debug.Log("Connecting...");
//         PhotonNetwork.ConnectUsingSettings();
//     }

//     public override void OnConnectedToMaster()
//     {
//         base.OnConnectedToMaster();
//         Debug.Log("Connected to Server");
//         PhotonNetwork.JoinLobby();
//     }

//     public override void OnJoinedLobby()
//     {
//         base.OnJoinedLobby();
//         RoomOptions roomOptions = new RoomOptions();
//         roomOptions.MaxPlayers = 4;
//         PhotonNetwork.JoinOrCreateRoom("test", roomOptions, TypedLobby.Default);
//         Debug.Log("Attempting to join or create room...");
//     }

//     public override void OnJoinedRoom()
//     {
//         base.OnJoinedRoom();
//         Debug.Log("Joined Room Successfully");
//         roomCam.SetActive(false);
//         // Instantiate player and let PlayerSetup handle the rest
//         PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity);
//         _player.GetComponent<PlayerSetup>().IsLocalPlayer();
//         _player.GetComponent<PlayerMovement>().isLocalPlayer = true;
//     }

//     public override void OnCreateRoomFailed(short returnCode, string message)
//     {
//         Debug.LogError($"Room creation failed: {message}");
//     }

//     public override void OnJoinRoomFailed(short returnCode, string message)
//     {
//         Debug.LogError($"Room joining failed: {message}");
//     }

//     public void RespawnPlayer()
//     {
//         PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity);
//         _player.GetComponent<PlayerSetup>().IsLocalPlayer();
//         _player.GetComponent<PlayerMovement>().isLocalPlayer = true;
//     }
// }


using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager instance;
    public GameObject _player;
    public GameObject playerPrefab;
    public Transform spawnPoint;

    [Space]
    public GameObject roomCam;

    void Awake()
    {
        // Ensure only one instance of RoomManager exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Debug.Log("Connecting...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected to Server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        PhotonNetwork.JoinOrCreateRoom("test", roomOptions, TypedLobby.Default);
        Debug.Log("Attempting to join or create room...");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined Room Successfully");
        roomCam.SetActive(false);
        
        // Instantiate player and let PlayerSetup handle the rest
        _player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity);
        
        PlayerSetup playerSetup = _player.GetComponent<PlayerSetup>();
        PlayerMovement playerMovement = _player.GetComponent<PlayerMovement>();

        if (playerSetup != null)
        {
            playerSetup.IsLocalPlayer();
        }

        if (playerMovement != null)
        {
            playerMovement.isLocalPlayer = true;
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Room creation failed: {message}");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Room joining failed: {message}");
    }

    public void RespawnPlayer()
    {
        // Destroy the current player if it exists
        if (_player != null)
        {
            PhotonNetwork.Destroy(_player);
        }

        // Instantiate a new player
        _player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity);
        
        // Ensure local player setup
        PlayerSetup playerSetup = _player.GetComponent<PlayerSetup>();
        PlayerMovement playerMovement = _player.GetComponent<PlayerMovement>();

        if (playerSetup != null)
        {
            playerSetup.IsLocalPlayer();
        }

        if (playerMovement != null)
        {
            playerMovement.isLocalPlayer = true;
        }
    }
}