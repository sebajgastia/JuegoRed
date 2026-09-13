using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    private bool hasSpawned = false;

    private void Start()
    {
        if (!PhotonNetwork.InRoom)
            return;

        if (hasSpawned)
            return;

        hasSpawned = true;

        PhotonNetwork.Instantiate(
            playerPrefab.name,
            transform.position,
            Quaternion.identity
        );
    }
}