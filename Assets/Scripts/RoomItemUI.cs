using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Realtime;

public class RoomItemUI : MonoBehaviour
{
    [SerializeField] private TMP_Text roomNameText;
    [SerializeField] private TMP_Text playerCountText;
    [SerializeField] private Button joinButton;

    private string roomName;
    private PhotonManager photonManager;

    public void Setup(RoomInfo info, PhotonManager manager)
    {
        roomName = info.Name;
        photonManager = manager;

        roomNameText.text = info.Name;
        playerCountText.text = $"{info.PlayerCount}/{info.MaxPlayers}";

        // Deshabilita el bot�n si la sala ya tiene 4 jugadores o est� cerrada
        bool isFull = info.PlayerCount >= info.MaxPlayers;
        joinButton.interactable = info.IsOpen && !isFull;

        joinButton.onClick.RemoveAllListeners();
        joinButton.onClick.AddListener(() =>
        {
            photonManager.JoinRoomByName(roomName);
        });
    }
}