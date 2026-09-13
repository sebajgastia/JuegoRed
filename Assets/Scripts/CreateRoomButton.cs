using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomButton : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button createRoomButton;

    private void Start()
    {
        createRoomButton.interactable = PhotonNetwork.InLobby;
    }

    public override void OnConnectedToMaster()
    {
        createRoomButton.interactable = false;
    }

    public override void OnJoinedLobby()
    {
        createRoomButton.interactable = true;
    }

    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        createRoomButton.interactable = false;
    }
}
