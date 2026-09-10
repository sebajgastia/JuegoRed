using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(PhotonView))]
public class PhotonManager : MonoBehaviourPunCallbacks
{
    [Header("Paneles de UI")]
    public GameObject lobbyPanel;      // Panel con InputField, Bot�n Crear y ScrollView
    public GameObject roomPanel;       // Panel de espera de la sala
    public GameObject startGameButton;  // Bot�n "Iniciar Juego"
    public GameObject menuCanvas;       // El Canvas entero que contiene todo el men�

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

        // chequeo para que no tire null
        if (roomNameInput == null || string.IsNullOrWhiteSpace(roomNameInput.text))
        {
            Debug.LogWarning("Nombre de sala inválido. Debes ingresar un nombre.");
            return;
        }

        string name = roomNameInput.text.Trim();

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

    // M�todo que llama el bot�n de Iniciar Juego
    public void StartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Cerramos la sala para que no entre nadie m�s a mitad de partida
            PhotonNetwork.CurrentRoom.IsOpen = false;

            // Enviamos el mensaje RPC a todos los jugadores de la sala
            GetComponent<PhotonView>().RPC("RPC_StartGame", RpcTarget.AllViaServer);
        }
    }

    [PunRPC]
    private void RPC_StartGame()
    {
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(false);
        }

        SpawnPlayer();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGame();
        }
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