using Photon.Pun;
using UnityEngine;

public class PlayerVisibility : MonoBehaviourPun
{
    [SerializeField] private Renderer[] playerRenderers;

    private PlayerRole playerRole;

    private void Awake()
    {
        playerRole = GetComponent<PlayerRole>();
    }

    private void Update()
    {
        if (GameManager_v2.Instance == null)
            return;

        PlayerRole localPlayer = GetLocalPlayerRole();

        if (localPlayer == null)
            return;

        bool localIsSeeker = localPlayer.CurrentRole == PlayerRole.Role.Seeker;

        bool thisIsHider = playerRole.CurrentRole == PlayerRole.Role.Hider;

        bool hiding = GameManager_v2.Instance.CurrentState == GameManager_v2.GameState.Hiding;

        if (localIsSeeker && thisIsHider && hiding)
        {
            SetVisible(false);
        }
        else
        {
            SetVisible(true);
        }
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

    private void SetVisible(bool visible)
    {
        foreach (Renderer rend in playerRenderers)
        {
            rend.enabled = visible;
        }
    }
}
