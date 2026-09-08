using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(PhotonView))]
public class PhotonManager : MonoBehaviourPunCallbacks
{
    [Header("Paneles de UI")]
    public GameObject lobbyPanel;      // Panel con InputField, Botón Crear y ScrollView
    public GameObject roomPanel;       // Panel de espera de la sala
    public GameObject startGameButton;  // Botón "Iniciar Juego"
    public GameObject menuCanvas;       // El Canvas entero que contiene todo el menú

    [Header("UI References")]
    public TMP_InputField roomNameInput;
    public TMP_Text currentRoomTitleText;

    [Header("Room List UI")]
    public Transform roomListContent;
    public GameObject roomItemPrefab;

    [Header("Game")]
    public GameObject playerPrefab;    // El Prefab de tu jugador (debe estar dentro de 'Assets/Resources/')
    public Transform[] spawnPoints;    // Puntos de spawn opcionales en el mapa

    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado al Master Server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("En el Lobby.");
        cachedRoomList.Clear();
        ClearRoomListUI();

        if (menuCanvas != null) menuCanvas.SetActive(true);
        if (lobbyPanel != null) lobbyPanel.SetActive(true);
        if (roomPanel != null) roomPanel.SetActive(false);
    }

    #region ACCIONES DE JUGADOR

    public void CreateRoom()
    {
        if (PhotonNetwork.InRoom) return;

        string name = "Sala_" + Random.Range(100, 999);
        if (roomNameInput != null && !string.IsNullOrEmpty(roomNameInput.text))
        {
            name = roomNameInput.text;
        }

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 4;
        options.IsVisible = true;
        options.IsOpen = true;

        PhotonNetwork.CreateRoom(name, options);
    }

    public void JoinRoomByName(string nameToJoin)
    {
        if (PhotonNetwork.InRoom) return;

        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.JoinRoom(nameToJoin);
        }
    }

    public void LeaveCurrentRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    // Método que llama el botón de Iniciar Juego
    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Cerramos la sala para que no entre nadie más a mitad de partida
            PhotonNetwork.CurrentRoom.IsOpen = false;

            // Enviamos el mensaje RPC a todos los jugadores de la sala
            GetComponent<PhotonView>().RPC("RPC_StartGame", RpcTarget.AllViaServer);
        }
    }

    [PunRPC]
    private void RPC_StartGame()
    {
        // 1. Desactivamos el menú por completo
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(false);
        }

        // 2. Instanciamos al jugador si hay un prefab asignado
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        if (playerPrefab != null)
        {
            Vector3 spawnPos = Vector3.zero;

            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                int randomIndex = Random.Range(0, spawnPoints.Length);
                spawnPos = spawnPoints[randomIndex].position;
            }

            // Nota: El prefab debe estar guardado en Assets/Resources/
            PhotonNetwork.Instantiate(playerPrefab.name, spawnPos, Quaternion.identity);
        }
    }

    #endregion

    #region CALLBACKS DE PHOTON

    public override void OnJoinedRoom()
    {
        Debug.Log($"Unido a la sala: {PhotonNetwork.CurrentRoom.Name}");

        if (lobbyPanel != null) lobbyPanel.SetActive(false);
        if (roomPanel != null) roomPanel.SetActive(true);

        if (currentRoomTitleText != null)
        {
            currentRoomTitleText.text = $"Sala: {PhotonNetwork.CurrentRoom.Name} ({PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers})";
        }

        if (startGameButton != null)
        {
            startGameButton.SetActive(PhotonNetwork.IsMasterClient);
        }
    }

    public override void OnLeftRoom()
    {
        if (menuCanvas != null) menuCanvas.SetActive(true);
        if (lobbyPanel != null) lobbyPanel.SetActive(true);
        if (roomPanel != null) roomPanel.SetActive(false);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            if (info.RemovedFromList)
            {
                cachedRoomList.Remove(info.Name);
            }
            else
            {
                cachedRoomList[info.Name] = info;
            }
        }
        RenderRoomList();
    }

    private void RenderRoomList()
    {
        ClearRoomListUI();

        foreach (KeyValuePair<string, RoomInfo> pair in cachedRoomList)
        {
            GameObject item = Instantiate(roomItemPrefab, roomListContent);
            item.GetComponent<RoomItemUI>().Setup(pair.Value, this);
        }
    }

    private void ClearRoomListUI()
    {
        foreach (Transform child in roomListContent)
        {
            Destroy(child.gameObject);
        }
    }

    #endregion
}