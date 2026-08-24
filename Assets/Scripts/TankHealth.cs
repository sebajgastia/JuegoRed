using Photon.Pun;
using UnityEngine;

public class TankHealth : MonoBehaviourPun
{
    [SerializeField] private int maxHealth = 100;

    public int CurrentHealth { get; private set; }
    private bool isDead = false;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    // El [PunRPC] es lo que permite que este método se ejecute en TODOS los
    // clientes cuando alguien lo llama con photonView.RPC(...)
    [PunRPC]
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        CurrentHealth -= amount;
        Debug.Log(gameObject.name + " recibió " + amount + " de daño. Vida: " + CurrentHealth);

        if (CurrentHealth <= 0)
            Die();
    }

    private void Die()
    {
        isDead = true;

        // Solo el dueño de ESTE tanque lo destruye (igual que con IsMine en el movimiento)
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
