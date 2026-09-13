using Photon.Pun;
using UnityEngine;

public class PlayerRole : MonoBehaviourPun
{
    public enum Role
    {
        Hider,
        Seeker
    }

    [SerializeField] private Role currentRole;
    [SerializeField] private Color hiderColor = Color.blue;
    [SerializeField] private Color seekerColor = Color.red;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color capturedColor = Color.cyan;

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

    public void UpdateColor()
    {
        if (spriteRenderer == null)
            return;

        if (playerCaptured != null && playerCaptured.IsCaptured)
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