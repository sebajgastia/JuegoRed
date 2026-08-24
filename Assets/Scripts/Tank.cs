using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Tank : MonoBehaviourPun
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 200f;

    void Update()
    {
        if (!photonView.IsMine)
            return;

        float move = Input.GetAxis("Vertical") * moveSpeed;
        float turn = -Input.GetAxis("Horizontal") * turnSpeed;

      
        transform.Rotate(0f, 0f, turn * Time.deltaTime);

        
        transform.Translate(Vector3.up * move * Time.deltaTime, Space.World == default ? Space.Self : Space.Self);
    }
}