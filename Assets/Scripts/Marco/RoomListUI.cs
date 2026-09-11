using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Dibuja la lista de salas como botones, y deja crear una sala nueva.
// No habla con Photon directamente: solo llama a los métodos de NetworkManager.
public class RoomListUI : MonoBehaviour
{
    public NetworkManager networkManager;
    public Transform roomListContainer;
    public GameObject roomButtonPrefab;
    public TMP_InputField newRoomNameInput;

    // Lo llama NetworkManager cada vez que cambia la lista de salas.
    public void RefreshRoomList(Dictionary<string, RoomInfo> rooms)
    {
        foreach (Transform child in roomListContainer)
            Destroy(child.gameObject);

        foreach (RoomInfo info in rooms.Values)
        {
            if (!info.IsOpen || !info.IsVisible) continue;

            GameObject buttonObj = Instantiate(roomButtonPrefab, roomListContainer);
            string roomName = info.Name;

            buttonObj.GetComponentInChildren<TMP_Text>().text =
                roomName + " (" + info.PlayerCount + "/" + info.MaxPlayers + ")";

            buttonObj.GetComponent<Button>().onClick.AddListener(() =>
            {
                networkManager.JoinRoom(roomName);
            });
        }
    }

    public void OnCreateRoomButtonPressed()
    {
        string roomName = newRoomNameInput.text;
        if (string.IsNullOrEmpty(roomName))
            roomName = "Sala" + Random.Range(100, 999);

        networkManager.CreateRoom(roomName);
    }
}