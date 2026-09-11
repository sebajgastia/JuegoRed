using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public TMP_Text statusText;
    public RoomListUI roomListUI;
    public string gameSceneName = "EscondidasGame";

    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    void Awake()
    {
        statusText.text = "Conectando...";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        statusText.text = "Conectado. Entrando al lobby...";
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        statusText.text = "Elegí una sala o creá una nueva.";
    }

 
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList)
                cachedRoomList.Remove(info.Name);
            else
                cachedRoomList[info.Name] = info;
        }

        roomListUI.RefreshRoomList(cachedRoomList);
    }

    public void JoinRoom(string roomName)
    {
        statusText.text = "Uniéndose a " + roomName + "...";
        PhotonNetwork.JoinRoom(roomName);
    }

    public void CreateRoom(string roomName)
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 4;
        options.IsVisible = true;
        options.IsOpen = true;

        statusText.text = "Creando sala " + roomName + "...";
        PhotonNetwork.CreateRoom(roomName, options);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        statusText.text = "No se pudo unir: " + message;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        statusText.text = "No se pudo crear la sala: " + message;
    }

    public override void OnJoinedRoom()
    {
        statusText.text = "Sala '" + PhotonNetwork.CurrentRoom.Name + "' (" +
                           PhotonNetwork.CurrentRoom.PlayerCount + "/4)";

        PhotonNetwork.AutomaticallySyncScene = true;

        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(gameSceneName);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        statusText.text = newPlayer.NickName + " se unió a la partida.";
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        statusText.text = otherPlayer.NickName + " abandonó la partida.";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        statusText.text = "Desconectado: " + cause;
        PhotonNetwork.ReconnectAndRejoin();
    }
}