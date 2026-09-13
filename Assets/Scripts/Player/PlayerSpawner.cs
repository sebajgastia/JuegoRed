using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private bool hasSpawned = false;

    private void Start()
    {
        if (!PhotonNetwork.InRoom)
            return;

        if (hasSpawned)
            return;

        hasSpawned = true;

        int spawnIndex =
            (PhotonNetwork.LocalPlayer.ActorNumber - 1)
            % spawnPoints.Length;

        Transform spawnPoint = spawnPoints[spawnIndex];

        PhotonNetwork.Instantiate(
            playerPrefab.name,
            spawnPoint.position,
            spawnPoint.rotation
        );
    }
}