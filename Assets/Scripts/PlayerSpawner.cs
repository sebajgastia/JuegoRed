using Photon.Pun;
using UnityEngine;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    [SerializeField] private string tankPrefabName = "Tank";

    public override void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate(tankPrefabName, new Vector3(0, 0, 0), Quaternion.identity);
    }
}
