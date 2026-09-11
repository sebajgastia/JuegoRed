using UnityEngine;
using Photon.Pun;

// Vive como objeto FIJO en la escena de juego (necesita un PhotonView agregado
// a mano en el Inspector, como el StageGenerator del ejemplo de Tanques).
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

    // Lo llama el botón "Empezar Partida" (solo lo ve el MasterClient).
    public void StartGame()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (PhotonNetwork.CurrentRoom.PlayerCount < 4) return;

        Debug.Log("empieza el juego");

        AssignRoles();
        ChangeState(GameState.Hiding);
    }

    // Lo va a llamar la lógica de captura de tus compañeros cuando se cumpla
    // la condición de victoria (cazador atrapó a todos, o se acabó el tiempo).
    public void EndGame()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        ChangeState(GameState.GameOver);
    }

    // Lo llama el botón "Volver a Jugar" (solo lo ve el MasterClient, después del GameOver).
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
            p.SetRole(PlayerRole.Role.Hider); // valor neutro hasta el próximo StartGame
    }

    public void ChangeState(GameState newState)
    {
        pv.RPC(nameof(Rpc_ChangeState), RpcTarget.All, (int)newState);
    }

    [PunRPC]
    private void Rpc_ChangeState(int newState)
    {
        CurrentState = (GameState)newState;
        Debug.Log(CurrentState);
    }
}