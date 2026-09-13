using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GameFlowUI : MonoBehaviour
{
    public Button startButton;
    public Button returnToLobbyButton;

    void Update()
    {
        bool isMaster = PhotonNetwork.IsMasterClient;
        GameManager_v2.GameState state = GameManager_v2.Instance.CurrentState;

        startButton.gameObject.SetActive(
            isMaster && state == GameManager_v2.GameState.Waiting && PhotonNetwork.CurrentRoom.PlayerCount == 4
        );

        returnToLobbyButton.gameObject.SetActive(
            isMaster && state == GameManager_v2.GameState.GameOver
        );
    }

    public void OnStartButtonPressed()
    {
        GameManager_v2.Instance.StartGame();
    }

    public void OnReturnToLobbyButtonPressed()
    {
        GameManager_v2.Instance.ReturnToLobby();
    }
}
