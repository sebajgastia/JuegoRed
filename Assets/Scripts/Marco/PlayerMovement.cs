using Photon.Pun;
using UnityEngine;

// Poné este script en el PREFAB del jugador.
// La sincronización de posición NO va en este código: la hace el componente
// "Photon Transform View" que agregás desde el Inspector (ver instrucciones).
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