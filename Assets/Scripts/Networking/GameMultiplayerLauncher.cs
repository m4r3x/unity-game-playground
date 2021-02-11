using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class GameMultiplayerLauncher : MonoBehaviourPunCallbacks
{
    // Users are separated from each other by gameVersion (which allows you make breaking changes).
    string GameVersion = "0.0.1";
    // Name of room, currently only single room is supported!
    const string RunForestRoomName = "fizzbuzz";
    private RoomOptions roomOptions;

    // MonoBehaviour method called on GameObject by Unity during initialization phase.
    void Start()
    {
        // Create roomOptions used later in a game.
        roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte) 16;
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        
        // This makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically.
        PhotonNetwork.AutomaticallySyncScene = true;
        Connect();
    }

    // Start the connection process.
    // - If already connected, we attempt joining a random room
    // - if not yet connected, Connect this application instance to Photon Cloud Network
    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("New player is connected.");
        }
        else
        {
            Debug.Log("Starting the game server. Connection to Photon initalized! GameVersion: " + GameVersion);
            PhotonNetwork.GameVersion = GameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("User Disconnected. Cause: " + cause.ToString() + " ServerAddress: " + PhotonNetwork.ServerAddress);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("First user connected to master. Joining " + RunForestRoomName + " room.");
        PhotonNetwork.JoinOrCreateRoom(RunForestRoomName, roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("User joined lobby: " + PhotonNetwork.NickName);
    }
    
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("We have received the Room list");
        Debug.Log(roomList);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom()");
        PhotonNetwork.NickName = "Player #" + Random.Range(0.0f, 100.0f);
        Debug.Log("Nickname: " + PhotonNetwork.NickName);
        PhotonNetwork.LoadLevel("MainScene");
    }
    
    public override void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom()");
    }
}
