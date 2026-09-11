using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

// Poné este script en la escena de juego. Maneja los dos botones que solo
// ve el MasterClient: "Empezar Partida" y "Volver a Jugar" (después de un GameOver).
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

    // Conectado al OnClick() de "Empezar Partida".
    public void OnStartButtonPressed()
    {
        GameManager_v2.Instance.StartGame();
    }

    // Conectado al OnClick() de "Volver a Jugar".
    public void OnReturnToLobbyButtonPressed()
    {
        GameManager_v2.Instance.ReturnToLobby();
    }
}
