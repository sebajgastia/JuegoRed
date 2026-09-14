using Photon.Pun;
using UnityEngine;

public class PlayerRole : MonoBehaviourPun
{
    public enum Role
    {
        None,
        Hider,
        Seeker
    }

    [SerializeField] private Role currentRole;
    [SerializeField] private Color hiderColor = Color.blue;
    [SerializeField] private Color seekerColor = Color.red;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color capturedColor = Color.cyan;
    [SerializeField] private Color lobbyColor = Color.white;
    public Role CurrentRole => currentRole;
    private PlayerCaptured playerCaptured;

    public void SetRole(Role newRole)
    {
        photonView.RPC(
            nameof(RPC_SetRole),
            RpcTarget.AllBuffered,
            (int)newRole
        );
    }

    [PunRPC]
    private void RPC_SetRole(int newRole)
    {
        currentRole = (Role)newRole;

        UpdateColor();

        if (photonView.IsMine)
        {
            Debug.Log(
                gameObject.name +
                " ahora es " +
                currentRole
            );
        }
    }

    public void ResetToLobby()
    {
        photonView.RPC(
            nameof(RPC_ResetToLobby),
            RpcTarget.AllBuffered
        );
    }

    [PunRPC]
    private void RPC_ResetToLobby()
    {
        currentRole = Role.None;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = lobbyColor;
        }

        Debug.Log(
           gameObject.name +
           " reseteado al lobby. Rol actual: " +
           currentRole
        );
    }

    public void UpdateColor()
    {
        if (spriteRenderer == null)
            return;

        if (currentRole == Role.None)
        {
            spriteRenderer.color = Color.white;
        }
        else if (playerCaptured != null && playerCaptured.IsCaptured)
        {
            spriteRenderer.color = capturedColor;
        }
        else if (currentRole == Role.Seeker)
        {
            spriteRenderer.color = seekerColor;
        }
        else
        {
            spriteRenderer.color = hiderColor;
        }
    }
}