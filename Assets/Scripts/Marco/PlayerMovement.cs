using Photon.Pun;
using UnityEngine;

public class PlayerMovement : MonoBehaviourPun
{
    public float moveSpeed = 5f;
    private PlayerRole playerRole;
    private PlayerCaptured playerCaptured;

    private void Start()
    {
        playerRole = GetComponent<PlayerRole>();
        playerCaptured = GetComponent<PlayerCaptured>();
    }

    void Update()
    {              
        if (!photonView.IsMine)
            return;

        if (playerCaptured.IsCaptured)
            return;

        if (playerRole.CurrentRole == PlayerRole.Role.Seeker && GameManager_v2.Instance.CurrentState == GameManager_v2.GameState.Hiding)
            return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(h, v, 0f);
        transform.position += movement.normalized * moveSpeed * Time.deltaTime;
    }
}