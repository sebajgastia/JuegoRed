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
    [SerializeField] private TMP_Text hidingTimerText;

    private GameManager_v2.GameState previousState;
    private float searchMessageTimer;

    private void Start()
    {
        searchStartedText.gameObject.SetActive(false);

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

        startButton.interactable =
            PhotonNetwork.CurrentRoom.PlayerCount == 3;//cambiar a 4

        returnToLobbyButton.gameObject.SetActive(
            isMaster &&
            state == GameManager_v2.GameState.GameOver
        );

        UpdateRoleText(state);
        UpdateCapturedText();
        UpdateHidingTimer(state);

        if (state != previousState)
        {
            if (state == GameManager_v2.GameState.Seeking)
            {
                ShowSearchStartedMessage();
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

    private void UpdateHidingTimer(GameManager_v2.GameState state)
    {
        if (state != GameManager_v2.GameState.Hiding)
        {
            hidingTimerText.gameObject.SetActive(false);
            return;
        }

        hidingTimerText.gameObject.SetActive(true);

        float timeRemaining =
            GameManager_v2.Instance.HidingTimeRemaining;

        int seconds =
            Mathf.CeilToInt(timeRemaining);

        hidingTimerText.text =
            seconds.ToString();
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
                "¡COMENZÁ A BUSCAR!";
        }
        else
        {
            searchStartedText.text =
                "¡EL SEEKER YA ESTÁ BUSCANDO!";
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

        if (state != GameManager_v2.GameState.Hiding)
        {
            roleText.gameObject.SetActive(false);
            return;
        }

        roleText.gameObject.SetActive(true);

        if (localPlayer.CurrentRole == PlayerRole.Role.Seeker)
        {
            roleText.text = "SOS EL SEEKER\nEsperá a que los jugadores se escondan";
        }
        else
        {
            roleText.text = "SOS HIDER\nTenés 45 segundos para esconderte";
        }
    }

    private void UpdateCapturedText()
    {
        PlayerRole localPlayer = GetLocalPlayerRole();

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