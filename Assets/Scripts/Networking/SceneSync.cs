using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SceneSync : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public Transform spawnPoint;
    
    void Start()
    {
        if (PhotonNetwork.CurrentRoom == null)
            return;

        // We're in a room. spawn a player for connected user.
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity, 0);
    }

    public override void OnLeftRoom()
    {
        Debug.Log("User left a room.");
        UnityEngine.SceneManagement.SceneManager.LoadScene("WaitScene");
    }
}
