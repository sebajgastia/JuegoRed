using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Grenade : MonoBehaviourPun
{
    [SerializeField] private float speed = 6f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float explosionRadius = 2.5f;

    private Rigidbody2D rb;
    private bool hasExploded = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Mover la granada hacia adelante
        rb.velocity = transform.up * speed;

        // Solo el dueño controla cuándo explota
        if (photonView.IsMine)
        {
            Invoke(nameof(Explode), lifeTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!photonView.IsMine)
            return;

        if (other.CompareTag("Prop"))
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (!photonView.IsMine)
            return;

        if (hasExploded)
            return;

        hasExploded = true;

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            explosionRadius
        );

        foreach (Collider2D hit in hits)
        {
            Prop prop = hit.GetComponent<Prop>();

            if (prop != null && prop.IsDestructible())
            {
                prop.DestroyProp();
            }
        }

        PhotonNetwork.Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            explosionRadius
        );
    }
}