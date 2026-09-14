using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomListUI : MonoBehaviour
{
    public NetworkManager networkManager;
    public Transform roomListContainer;
    public GameObject roomButtonPrefab;
    public TMP_InputField newRoomNameInput;
    [SerializeField] private TMP_FontAsset roomButtonFont;

    public void RefreshRoomList(Dictionary<string, RoomInfo> rooms)
    {
        foreach (Transform child in roomListContainer)
            Destroy(child.gameObject);

        foreach (RoomInfo info in rooms.Values)
        {
            if (!info.IsOpen || !info.IsVisible) continue;

            GameObject buttonObj = Instantiate(roomButtonPrefab, roomListContainer);
            string roomName = info.Name;

            TMP_Text buttonText =
                buttonObj.GetComponentInChildren<TMP_Text>();

            buttonText.font = roomButtonFont;

            buttonText.text = roomName + " (" +info.PlayerCount + "/" + info.MaxPlayers + ")";

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