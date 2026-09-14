using UnityEngine;
using Photon.Pun;

public class HiderDecoy : MonoBehaviourPun
{
    [SerializeField] private float timePenalty = 15f; // Tiempo a restar en segundos

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void Start()
    {
        UpdateVisibility();
    }

    private void Update()
    {
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        if (spriteRenderer == null) return;

        PlayerRole localRole = FindLocalPlayerRole();

        if (localRole != null && localRole.CurrentRole == PlayerRole.Role.Seeker)
        {
            
            if (GameManager_v2.Instance != null && GameManager_v2.Instance.CurrentState == GameManager_v2.GameState.Hiding)
            {
                spriteRenderer.enabled = false;
                return;
            }
        }

        spriteRenderer.enabled = true;
    }

    private PlayerRole FindLocalPlayerRole()
    {
        PlayerRole[] players = FindObjectsOfType<PlayerRole>();
        foreach (PlayerRole p in players)
        {
            if (p.photonView.IsMine)
                return p;
        }
        return null;
    }

    public void InteractWithSeeker()
    {
        photonView.RPC(nameof(RPC_OnDecoyTriggered), RpcTarget.AllViaServer);
    }

    [PunRPC]
    private void RPC_OnDecoyTriggered()
    {
        
        if (PhotonNetwork.IsMasterClient && GameManager_v2.Instance != null)
        {
            GameManager_v2.Instance.ApplyTimePenalty(timePenalty);
        }

        
        if (photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}