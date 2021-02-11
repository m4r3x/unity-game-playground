using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSync : MonoBehaviourPun, IPunObservable
{
    //List of the scripts that should only be active for the local player (ex. PlayerController)
    public MonoBehaviour[] localScripts;
    //List of the GameObjects that should only be active for the local player (ex. Camera, WeaponSocket)
    public GameObject[] localObjects;
    //Values that will be synced over network
    Vector3 latestPos;
    
    void Start()
    {
        if (photonView.IsMine)
        {
            // Player is local
        }
        else
        {
            // Player is Remote, deactivate the scripts and object that should only be enabled for the local player
            for (int i = 0; i < localScripts.Length; i++)
            {
                localScripts[i].enabled = false;
            }
            for (int i = 0; i < localObjects.Length; i++)
            {
                localObjects[i].SetActive(false);
            }
        }
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            // Update remote player state
            transform.position = Vector3.Lerp(transform.position, latestPos, Time.deltaTime * 5);
        }
    }
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send others our data.
            stream.SendNext(transform.position);
        }
        else
        {
            // Receive data from others.
            latestPos = (Vector3) stream.ReceiveNext();
        }
    }
}
