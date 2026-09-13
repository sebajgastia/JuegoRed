using Photon.Pun;
using UnityEngine;

public class SeekerCapture : MonoBehaviourPun
{
    [SerializeField] private float captureRadius = 1.5f;

    private PlayerRole playerRole;

    private void Awake()
    {
        playerRole = GetComponent<PlayerRole>();
    }

    private void Update()
    {
        if (!photonView.IsMine)
            return;

        if (playerRole.CurrentRole != PlayerRole.Role.Seeker)
            return;

        if (
            GameManager_v2.Instance.CurrentState !=
            GameManager_v2.GameState.Seeking
        )
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryCapture();
        }
    }

    private void TryCapture()
    {
        Collider2D[] colliders =
            Physics2D.OverlapCircleAll(
                transform.position,
                captureRadius
            );

        foreach (Collider2D col in colliders)
        {
            PlayerRole target =
                col.GetComponent<PlayerRole>();

            if (target == null)
                continue;

            if (target == playerRole)
                continue;

            if (
                target.CurrentRole !=
                PlayerRole.Role.Hider
            )
                continue;

            PlayerCaptured captured =
                target.GetComponent<PlayerCaptured>();

            if (
                captured != null &&
                captured.IsCaptured
            )
                continue;

            PhotonView targetView =
                target.GetComponent<PhotonView>();

            if (targetView == null)
                continue;

            photonView.RPC(
                nameof(RPC_Capture),
                RpcTarget.All,
                targetView.ViewID
            );

            break;
        }
    }

    [PunRPC]
    private void RPC_Capture(int targetViewID)
    {
        PhotonView targetView =
            PhotonView.Find(targetViewID);

        if (targetView == null)
            return;

        PlayerCaptured captured =
            targetView.GetComponent<PlayerCaptured>();

        if (captured != null)
        {
            captured.Capture();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            captureRadius
        );
    }
}
