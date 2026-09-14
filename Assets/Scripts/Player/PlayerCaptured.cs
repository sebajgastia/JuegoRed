using Photon.Pun;
using UnityEngine;

public class PlayerCaptured : MonoBehaviourPun
{
    public bool IsCaptured { get; private set; }
    private PlayerRole playerRole;

    private void Awake()
    {
        playerRole = GetComponent<PlayerRole>();
    }

    public void Capture()
    {
        if (IsCaptured) return;

        photonView.RPC(
            nameof(RPC_SetCapturedState),
            RpcTarget.AllBuffered,
            true
        );
    }

    public void Free()
    {
        if (!IsCaptured) return;

        photonView.RPC(
            nameof(RPC_SetCapturedState),
            RpcTarget.AllBuffered,
            false
        );
    }

    [PunRPC]
    private void RPC_SetCapturedState(bool state)
    {
        IsCaptured = state;

        if (IsCaptured)
        {
            Debug.Log(gameObject.name + " fue capturado");
        }
        else
        {
            Debug.Log(gameObject.name + " fue liberado");
        }
    }

    public void ResetCaptured()
    {
        photonView.RPC(
            nameof(RPC_ResetCaptured),
            RpcTarget.AllBuffered
        );
    }

    [PunRPC]
    private void RPC_ResetCaptured()
    {
        IsCaptured = false;

        if (playerRole != null)
        {
            playerRole.UpdateColor();
        }

        Debug.Log(gameObject.name + " deja de estar capturado");
    }
}