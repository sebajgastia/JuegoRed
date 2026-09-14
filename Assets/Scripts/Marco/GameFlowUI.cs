using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameFlowUI : MonoBehaviour
{
    public Button startButton;
    public Button returnToLobbyButton;

    [SerializeField] private TMP_Text roleText;
    [SerializeField] private TMP_Text capturedText;

    [SerializeField] private TMP_Text searchStartedText;
    [SerializeField] private float searchMessageDuration = 3f;

    [Header("End Game")]
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private TMP_Text resultText;

    private GameManager_v2.GameState previousState;
    private float searchMessageTimer;

    private void Start()
    {

        endGamePanel.SetActive(false);

        returnToLobbyButton.gameObject.SetActive(false);

        if (GameManager_v2.Instance != null)
        {
            previousState =
                GameManager_v2.Instance.CurrentState;
        }
    }

    void Update()
    {
        if (!PhotonNetwork.InRoom || GameManager_v2.Instance == null)
            return;

        bool isMaster = PhotonNetwork.IsMasterClient;
        
        GameManager_v2.GameState state =
            GameManager_v2.Instance.CurrentState;

        startButton.gameObject.SetActive(
            isMaster &&
            state == GameManager_v2.GameState.Waiting
        );

        int count = PhotonNetwork.CurrentRoom.PlayerCount;

        
        startButton.interactable = PhotonNetwork.IsMasterClient && (count >= 2 && count <= 4);

        returnToLobbyButton.gameObject.SetActive(
            isMaster &&
            state == GameManager_v2.GameState.GameOver
        );

        UpdateRoleText(state);
        UpdateCapturedText();
        //UpdateHidingTimer(state);

        if (state != previousState)
        {
            if (state == GameManager_v2.GameState.Seeking)
            {
                ShowSearchStartedMessage();
            }

            if (state == GameManager_v2.GameState.GameOver)
            {
                ShowGameOverResult();
            }

            if (state == GameManager_v2.GameState.Waiting)
            {
                endGamePanel.SetActive(false);
            }

            previousState = state;
        }

        if (searchMessageTimer > 0f)
        {
            searchMessageTimer -= Time.deltaTime;

            if (searchMessageTimer <= 0f)
            {
                searchStartedText.gameObject.SetActive(false);
            }
        }
    }

    private void ShowGameOverResult()
    {
        PlayerRole localPlayer =
            GetLocalPlayerRole();

        if (localPlayer == null)
            return;

        PlayerRole.Role winner =
            GameManager_v2.Instance.WinningRole;

        bool localPlayerWon =
            localPlayer.CurrentRole == winner;

        if (localPlayerWon)
        {
            resultText.text = "VICTORIA";
            resultText.color = Color.green;
        }
        else
        {
            resultText.text = "DERROTA";
            resultText.color = Color.red;
        }

        capturedText.gameObject.SetActive(false);

        roleText.gameObject.SetActive(false);
        //hidingTimerText.gameObject.SetActive(false);
        searchStartedText.gameObject.SetActive(false);

        endGamePanel.SetActive(true);

        returnToLobbyButton.gameObject.SetActive(
            PhotonNetwork.IsMasterClient
        );

        returnToLobbyButton.interactable =
            PhotonNetwork.IsMasterClient;
    }

    private void ShowSearchStartedMessage()
    {
        PlayerRole localPlayer =
            GetLocalPlayerRole();

        if (localPlayer == null)
            return;

        if (localPlayer.CurrentRole ==
            PlayerRole.Role.Seeker)
        {
            searchStartedText.text =
                "COMIENZA A BUSCAR!";
        }
        else
        {
            searchStartedText.text =
                "EL SEEKER YA ESTA BUSCANDO!";
        }

        searchStartedText.gameObject.SetActive(true);

        searchMessageTimer =
            searchMessageDuration;
    }

    private void UpdateRoleText(GameManager_v2.GameState state)
    {
        PlayerRole localPlayer = GetLocalPlayerRole();

        if (localPlayer == null)
        {
            roleText.gameObject.SetActive(false);
            return;
        }

        if (roleText != null && state != GameManager_v2.GameState.Hiding)
        {
            roleText.gameObject.SetActive(false);
            return;
        }

        if (state != GameManager_v2.GameState.GameOver) roleText.gameObject.SetActive(true);

        if (roleText != null && localPlayer.CurrentRole == PlayerRole.Role.Seeker)
        {
            roleText.text = "SOS EL SEEKER\nEspera a que los jugadores se escondan";
        }
        else
        {
            roleText.text = "SOS HIDER\nTenes 45 segundos para esconderte";
        }
    }

    private void UpdateCapturedText()
    {
        if (GameManager_v2.Instance == null)
            return;

        //durante la fase de b�squeda
        if (GameManager_v2.Instance.CurrentState !=
            GameManager_v2.GameState.Seeking)
        {
            capturedText.gameObject.SetActive(false);
            return;
        }

        PlayerRole localPlayer =
            GetLocalPlayerRole();

        if (localPlayer == null)
        {
            capturedText.gameObject.SetActive(false);
            return;
        }

        PlayerCaptured captured =
            localPlayer.GetComponent<PlayerCaptured>();

        if (captured == null)
        {
            capturedText.gameObject.SetActive(false);
            return;
        }

        capturedText.gameObject.SetActive(
            captured.IsCaptured
        );
    }

    private PlayerRole GetLocalPlayerRole()
    {
        PlayerRole[] players =
            FindObjectsOfType<PlayerRole>();

        foreach (PlayerRole player in players)
        {
            PhotonView view =
                player.GetComponent<PhotonView>();

            if (view != null && view.IsMine)
                return player;
        }

        return null;
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