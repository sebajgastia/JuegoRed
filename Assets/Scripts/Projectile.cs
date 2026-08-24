using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviourPun
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 30;
    [SerializeField] private float lifeTime = 3f;

    private void Start()
    {
        GetComponent<Rigidbody2D>().velocity = transform.up * speed;

        if (photonView.IsMine)
            Invoke(nameof(SelfDestruct), lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Solo quien disparó (dueño de la bala) resuelve el impacto
        if (!photonView.IsMine)
            return;

        TankHealth tank = other.GetComponent<TankHealth>();
        if (tank == null)
            return; // chocó contra otra cosa (pared, obstáculo)

        // Evitar que un tanque se dañe con su propia bala apenas nace
        if (tank.photonView.Owner != null && tank.photonView.Owner.Equals(photonView.Owner))
            return;

        tank.photonView.RPC(nameof(TankHealth.TakeDamage), RpcTarget.All, damage);
        PhotonNetwork.Destroy(gameObject);
    }

    private void SelfDestruct()
    {
        if (this != null && photonView.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }
}