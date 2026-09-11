using Photon.Pun;
using UnityEngine;

public class PlayerMovement : MonoBehaviourPun
{
    public float moveSpeed = 5f;

    void Update()
    {
        if (!photonView.IsMine)
            return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(h, v, 0f);
        transform.position += movement.normalized * moveSpeed * Time.deltaTime;
    }
}