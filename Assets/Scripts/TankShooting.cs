using Photon.Pun;
using UnityEngine;

public class TankShooting : MonoBehaviourPun
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private string projectilePrefabName = "Projectile";
    [SerializeField] private float fireCooldown = 0.5f;

    private float lastFireTime = -999f;

    private void Update()
    {
        if (!photonView.IsMine)
            return;

        if (Input.GetButtonDown("Fire1") && Time.time - lastFireTime >= fireCooldown)
        {
            lastFireTime = Time.time;
            PhotonNetwork.Instantiate(projectilePrefabName, firePoint.position, firePoint.rotation);
        }
    }
}