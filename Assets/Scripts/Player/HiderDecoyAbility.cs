using UnityEngine;
using Photon.Pun;

public class HiderDecoyAbility : MonoBehaviourPun
{
    [SerializeField] private GameObject decoyPrefab; 
    [SerializeField] private int maxDecoys = 2; 
    [SerializeField] private KeyCode spawnKey = KeyCode.Q;

    private int decoysPlaced = 0;
    private PlayerRole playerRole;

    private void Awake()
    {
        playerRole = GetComponent<PlayerRole>();
    }

    private void Update()
    {
        if (!photonView.IsMine) return;
        if (playerRole == null || playerRole.CurrentRole != PlayerRole.Role.Hider) return;

        
        if (GameManager_v2.Instance == null) return;
        var state = GameManager_v2.Instance.CurrentState;
        if (state != GameManager_v2.GameState.Hiding && state != GameManager_v2.GameState.Seeking) return;

        if (Input.GetKeyDown(spawnKey) && decoysPlaced < maxDecoys)
        {
            SpawnDecoy();
        }
    }

    private void SpawnDecoy()
    {
        decoysPlaced++;
       
        PhotonNetwork.Instantiate("DecoyPrefab", transform.position, transform.rotation);
    }
}