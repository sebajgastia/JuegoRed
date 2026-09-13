using Photon.Pun;
using UnityEngine;

public class HiderRescue : MonoBehaviourPun
{
    [SerializeField] private float rescueRadius = 1.5f;
    [SerializeField] private float rescueTime = 5f;

    private PlayerRole playerRole;
    private PlayerCaptured myCaptured;

    private PlayerRole rescueTarget;
    private float rescueProgress = 0f;

    private void Awake()
    {
        playerRole = GetComponent<PlayerRole>();
        myCaptured = GetComponent<PlayerCaptured>();
    }

    private void Update()
    {
        if (!photonView.IsMine) return;
        if (playerRole == null || playerRole.CurrentRole != PlayerRole.Role.Hider) return;
        if (myCaptured != null && myCaptured.IsCaptured) return;

        if (GameManager_v2.Instance != null && GameManager_v2.Instance.CurrentState != GameManager_v2.GameState.Seeking)
            return;

        PlayerRole nearbyCaptured = GetNearbyCapturedHider();

        // Validar si el objetivo sigue al alcance
        if (rescueTarget != null)
        {
            float distance = Vector2.Distance(transform.position, rescueTarget.transform.position);
            PlayerCaptured targetCaptured = rescueTarget.GetComponent<PlayerCaptured>();

            if (distance > rescueRadius || targetCaptured == null || !targetCaptured.IsCaptured)
            {
                rescueTarget = null;
                rescueProgress = 0f;
            }
        }

        if (rescueTarget == null && nearbyCaptured != null)
        {
            rescueTarget = nearbyCaptured;
            rescueProgress = 0f;
        }

        if (rescueTarget == null) return;

        if (Input.GetKey(KeyCode.E))
        {
            rescueProgress += Time.deltaTime;

            if (rescueProgress >= rescueTime)
            {
                PlayerCaptured targetCaptured = rescueTarget.GetComponent<PlayerCaptured>();
                if (targetCaptured != null)
                {
                    // Llama a Free(), el cual ejecuta el RPC buffered en la red
                    targetCaptured.Free();
                }

                rescueProgress = 0f;
                rescueTarget = null;
            }
        }
        else
        {
            rescueProgress = 0f;
        }
    }

    private PlayerRole GetNearbyCapturedHider()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, rescueRadius);

        foreach (Collider2D col in colliders)
        {
            PlayerRole target = col.GetComponentInParent<PlayerRole>();

            if (target == null || target == playerRole) continue;
            if (target.CurrentRole != PlayerRole.Role.Hider) continue;

            PlayerCaptured captured = target.GetComponent<PlayerCaptured>();
            if (captured == null || !captured.IsCaptured) continue;

            return target;
        }

        return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rescueRadius);
    }
}