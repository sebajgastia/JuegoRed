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

    public Role CurrentRole => currentRole;

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

        if (photonView.IsMine)
            Debug.Log(
                gameObject.name +
                " ahora es " +
                currentRole
            );
    }
}