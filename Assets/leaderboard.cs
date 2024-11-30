// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.Linq;
// using UnityEngine.UI;
// using Photon.Pun;
// using TMPro;

// public class leaderboard : MonoBehaviour
// {
//     public GameObject PlayersHolder;
//     [Header("Options")]
//     public float refreshRate = 1f;
//     [Header("UI")]
//     public GameObject slots;
//     public TextMeshProUGUI[] names;
//     public TextMeshProUGUI[] scores;

//     private void Start()
//     {
//         InvokeRepeating(nameof(Refresh), 1f, refreshRate);
//     }

//     public void Refresh()
//     {
//         foreach (var slot in slots)
//         {
//             slot.setActive(false);
//         }
//         // var playerList = (from player in PhotonNetwork.PlayerList orderby player.GetScore() descending select player).ToList();
//         var playerList = (from player in PhotonNetwork.PlayerList select player).ToList();
//         int i = 0;
//         foreach (var player in playerList)
//         {
//             slots[i].setActive(true);
//             if (player.nickname == "")
//             {
//                 names[i].text = "Player " + player.ActorNumber;
//             }
//             names[i].text = player.nickname;
//             i++;
//         }

//     }
// }


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    public GameObject PlayersHolder;
    [Header("Options")]
    public float refreshRate = 1f;
    [Header("UI")]
    public GameObject[] slots;
    public TextMeshProUGUI[] names;
    public TextMeshProUGUI[] scores;

    private void Start()
    {
        InvokeRepeating(nameof(Refresh), 1f, refreshRate);
    }

    public void Refresh()
    {
        // Deactivate all slots
        foreach (var slot in slots)
        {
            slot.SetActive(false);
        }

        // Get list of players
        var playerList = PhotonNetwork.PlayerList.ToList();
        // Debug.Log(playerList);
        // Limit to number of available slots to prevent out of bounds exception
        int maxSlots = Mathf.Min(playerList.Count, slots.Length);

        for (int i = 0; i < maxSlots; i++)
        {
            var player = playerList[i];

            // Activate the slot
            slots[i].SetActive(true);

            // Set name
            if (string.IsNullOrEmpty(player.NickName))
            {
                names[i].text = "Player " + player.ActorNumber;
            }
            else
            {
                names[i].text = player.NickName;
            }
        }
    }

    // public void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Tab))
    //     {
    //         PlayersHolder.SetActive(!PlayersHolder.activeSelf);
    //     }
    // }
}