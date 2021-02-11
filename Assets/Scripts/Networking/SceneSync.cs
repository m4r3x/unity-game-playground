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
        // In case we started this demo with the wrong scene being active, simply load the menu scene
        if (PhotonNetwork.CurrentRoom == null)
        {
            Debug.Log("Is not in the room. Exiting...");
            // UnityEngine.SceneManagement.SceneManager.LoadScene("WaitScene");
            return;
        }
        Debug.Log("Room is not empty. Instantinating player instance.");

        // We're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
        PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint.position, Quaternion.identity, 0);
    }

    public override void OnLeftRoom()
    {
        Debug.Log("User left a room.");
        UnityEngine.SceneManagement.SceneManager.LoadScene("WaitScene");
    }
}
