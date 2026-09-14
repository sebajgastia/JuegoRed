using UnityEngine;
using System.Collections;
using Photon.Pun;

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

        // CAMBIAR A 4 PARA EL JUEGO / ESTA ASI PARA TESTEAR
        if (PhotonNetwork.CurrentRoom.PlayerCount < 2) return;

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
        while (PhotonNetwork.Time < hidingEndTime)
        {
            yield return null;
        }

        StartSeekingPhase();
    }
    #endregion

    #region FASE DE BÚSQUEDA (SEEKING)
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
        while (PhotonNetwork.Time < seekingEndTime)
        {
            yield return null;
        }

        Debug.Log("¡Se acabó el tiempo de búsqueda! Ganaron los Hiders.");
        EndGame();
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
        Debug.Log("¡Penalización aplicada! Tiempo restante de búsqueda: " + SeekingTimeRemaining);
    }
    #endregion

    public void EndGame()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        ChangeState(GameState.GameOver);
    }

    public void ReturnToLobby()
    {
        if (!PhotonNetwork.IsMasterClient) return;

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
        PlayerRole[] players = FindObjectsOfType<PlayerRole>();
        foreach (PlayerRole p in players)
            p.SetRole(PlayerRole.Role.Hider);
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
}