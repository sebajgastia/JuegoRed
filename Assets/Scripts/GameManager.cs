using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        Waiting,
        Hiding,
        Seeking,
        GameOver
    }

    public GameState CurrentState { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        CurrentState = GameState.Waiting;
    }

    public void StartGame()
    {
        Debug.Log("empieza el juego");

        ChangeState(GameState.Hiding);
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;

        Debug.Log(CurrentState);
    }
    public void SetPlayerAsSeeker(PlayerRole playerRole)
    {
        playerRole.SetRole(PlayerRole.Role.Seeker);
    }
    public void SetPlayerAsHider(PlayerRole playerRole)
    {
        playerRole.SetRole(PlayerRole.Role.Hider);
    }
    private PlayerRole[] GetPlayers()
    {
        return FindObjectsOfType<PlayerRole>();
    }
}