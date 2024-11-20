using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager instance;
    public GameObject _player;
    public GameObject playerPrefab;
    public Transform[] spawnPoints;

    [Space]
    public GameObject roomCam;

    public GameObject nameUI;
    public GameObject connectingUI;

    private string nickname = "player";

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
        // Debug.Log("Connecting...");
        // PhotonNetwork.ConnectUsingSettings();

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
        RespawnPlayer();
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
        Transform spawnPoint = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
        // Instantiate a new player
        _player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity);

        // Ensure local player setup
        PlayerSetup playerSetup = _player.GetComponent<PlayerSetup>();
        PlayerMovement playerMovement = _player.GetComponent<PlayerMovement>();
        PhotonView photonView = playerSetup.GetComponent<PhotonView>();

        if (playerSetup != null)
        {
            playerSetup.IsLocalPlayer();
        }

        if (playerMovement != null)
        {
            playerMovement.isLocalPlayer = true;
        }
        if (photonView != null)

        {
            // Debug.Log("Setting nickname to: " + nickname);
            // photonView.RPC("SetNickname", RpcTarget.AllBuffered, nickname);  
            photonView.RPC("SetNickname", RpcTarget.AllBuffered, nickname);
        }

    }
    public void ChangeNickName(string _name)
    {
        // Set local nickname
        nickname = _name;
    }

    public void JoinRoomButtonPressed()
    {
        Debug.Log("Connecting...");
        PhotonNetwork.ConnectUsingSettings();
        nameUI.SetActive(false);
        connectingUI.SetActive(true);
    }
}

