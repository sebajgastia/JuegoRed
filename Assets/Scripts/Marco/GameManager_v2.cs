using Photon.Pun;
using System.Collections;
using System.Reflection;
using UnityEngine;

public class GameManager_v2 : MonoBehaviourPunCallbacks
{
    public static GameManager_v2 Instance { get; private set; }

    public enum GameState
    {
        Waiting,
        Hiding,
        Seeking,
        GameOver
    }

    public GameState CurrentState { get; private set; }
    public PlayerRole.Role WinningRole { get; private set; }

    private PhotonView pv;

    [SerializeField] private PropSpawner propSpawner;
    [SerializeField] private float hidingTime = 45f;
    [SerializeField] private float seekingTime = 120f; 

    private double hidingEndTime;
    private double seekingEndTime; 

    public float HidingTimeRemaining
    {
        get
        {
            if (CurrentState != GameState.Hiding)
                return 0f;

            return Mathf.Max(
                0f,
                (float)(hidingEndTime - PhotonNetwork.Time)
            );
        }
    }

    public float SeekingTimeRemaining
    {
        get
        {
            if (CurrentState != GameState.Seeking)
                return 0f;

            return Mathf.Max(
                0f,
                (float)(seekingEndTime - PhotonNetwork.Time)
            );
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        CurrentState = GameState.Waiting;
    }

    public void StartGame()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        
        if (playerCount < 2 || playerCount > 4) return;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        Debug.Log("Empieza el juego");

        propSpawner.SpawnProps();
        AssignRoles();
        StartHidingPhase();
    }

    #region FASE DE ESCONDITE (HIDING)
    private void StartHidingPhase()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        double endTime = PhotonNetwork.Time + hidingTime;

        pv.RPC(
            nameof(RPC_StartHiding),
            RpcTarget.All,
            endTime
        );

        StartCoroutine(HidingPhase());
    }

    [PunRPC]
    private void RPC_StartHiding(double endTime)
    {
        hidingEndTime = endTime;
        CurrentState = GameState.Hiding;

        Debug.Log("Hiding comenzó. Termina en PhotonTime: " + hidingEndTime);
    }

    private IEnumerator HidingPhase()
    {
        while (CurrentState == GameState.Hiding && PhotonNetwork.Time < hidingEndTime)
        {
            yield return null;
        }

        if (CurrentState != GameState.Hiding)
        {
            yield break;
        }

        StartSeekingPhase();
    }
    #endregion

    #region FASE DE BUSQUEDA (SEEKING)
    private void StartSeekingPhase()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        double endTime = PhotonNetwork.Time + seekingTime;

        pv.RPC(
            nameof(RPC_StartSeeking),
            RpcTarget.All,
            endTime
        );

        StartCoroutine(SeekingPhase());
    }

    [PunRPC]
    private void RPC_StartSeeking(double endTime)
    {
        seekingEndTime = endTime;
        CurrentState = GameState.Seeking;

        Debug.Log("Seeking comenzó. Termina en PhotonTime: " + seekingEndTime);
    }

    private IEnumerator SeekingPhase()
    {
        while (CurrentState == GameState.Seeking && PhotonNetwork.Time < seekingEndTime)
        {

            if (AreAllHidersCaptured())
            {
                Debug.Log("Todos los Hiders fueron capturados. Ganó el Seeker.");

                EndGame(PlayerRole.Role.Seeker);

                yield break;
            }

            yield return null;
        }

        // Si salimos de Seeking porque volvimos al lobby
        if (CurrentState != GameState.Seeking)
        {
            yield break;
        }

        Debug.Log("Se acabó el tiempo. Ganaron los Hiders.");

        EndGame(PlayerRole.Role.Hider);
    }

    private bool AreAllHidersCaptured()
    {
        PlayerRole[] players =
            FindObjectsOfType<PlayerRole>();

        bool foundHider = false;

        foreach (PlayerRole player in players)
        {
            if (player.CurrentRole != PlayerRole.Role.Hider)
                continue;

            foundHider = true;

            PlayerCaptured captured =
                player.GetComponent<PlayerCaptured>();

            if (captured == null || !captured.IsCaptured)
            {
                return false;
            }
        }

        return foundHider;
    }

    public void ApplyTimePenalty(float penaltySeconds)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (CurrentState == GameState.Seeking)
        {

            seekingEndTime -= penaltySeconds;
            pv.RPC(nameof(RPC_SyncSeekingTime), RpcTarget.All, seekingEndTime);
        }
    }

    [PunRPC]
    private void RPC_SyncSeekingTime(double newEndTime)
    {
        seekingEndTime = newEndTime;
        Debug.Log("Penalizacion aplicada. Tiempo restante de busqueda: " + SeekingTimeRemaining);
    }
    #endregion

    public void EndGame(PlayerRole.Role winner)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (CurrentState == GameState.GameOver)
            return;

        pv.RPC(
            nameof(RPC_EndGame),
            RpcTarget.All,
            (int)winner
        );
    }

    [PunRPC]
    private void RPC_EndGame(int winningRole)
    {
        WinningRole =
            (PlayerRole.Role)winningRole;

        CurrentState =
            GameState.GameOver;

        Debug.Log(
            "GAME OVER - Ganó: " +
            WinningRole
        );
    }

    public void ReturnToLobby()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        PhotonNetwork.CurrentRoom.IsOpen = true;
        PhotonNetwork.CurrentRoom.IsVisible = true;

        propSpawner.ClearAllProps();

        ClearAllDecoys();

        ResetRoles();

        ChangeState(GameState.Waiting);
    }

    private void AssignRoles()
    {
        PlayerRole[] players = FindObjectsOfType<PlayerRole>();
        if (players.Length == 0) return;

        int seekerIndex = Random.Range(0, players.Length);

        for (int i = 0; i < players.Length; i++)
        {
            PlayerRole.Role role = (i == seekerIndex) ? PlayerRole.Role.Seeker : PlayerRole.Role.Hider;
            players[i].SetRole(role);
        }
    }

    private void ResetRoles()
    {
        PlayerRole[] players =
            FindObjectsOfType<PlayerRole>();

        foreach (PlayerRole player in players)
        {
            PlayerCaptured captured =
                player.GetComponent<PlayerCaptured>();

            if (captured != null)
            {
                captured.ResetCaptured();
            }

            player.ResetToLobby();
        }
    }

    public void ChangeState(GameState newState)
    {
        pv.RPC(nameof(Rpc_ChangeState), RpcTarget.All, (int)newState);
    }

    [PunRPC]
    private void Rpc_ChangeState(int newState)
    {
        CurrentState = (GameState)newState;
        Debug.Log("Estado actual: " + CurrentState);
    }
    private void ClearAllDecoys()
    {
        pv.RPC(
            nameof(RPC_ClearOwnDecoys),
            RpcTarget.All
        );
    }

    [PunRPC]
    private void RPC_ClearOwnDecoys()
    {
        HiderDecoy[] decoys =
            FindObjectsOfType<HiderDecoy>();

        foreach (HiderDecoy decoy in decoys)
        {
            if (decoy.photonView.IsMine)
            {
                PhotonNetwork.Destroy(
                    decoy.gameObject
                );
            }
        }
    }
}