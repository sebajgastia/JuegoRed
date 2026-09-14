using Photon.Pun;
using UnityEngine;

public class PlayerMovement : MonoBehaviourPun
{
    public float moveSpeed = 5f;
    private PlayerRole playerRole;
    private PlayerCaptured playerCaptured;
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private Transform grenadeSpawnPoint;
    private int grenadeAmount;

    private void Start()
    {
        playerRole = GetComponent<PlayerRole>();
        playerCaptured = GetComponent<PlayerCaptured>();
        grenadeAmount = 2;
    }

    void Update()
    {
        if (GameManager_v2.Instance != null && GameManager_v2.Instance.CurrentState == GameManager_v2.GameState.GameOver)
            return;

        if (!photonView.IsMine)
            return;

        if (playerCaptured.IsCaptured)
            return;

        if (playerRole.CurrentRole == PlayerRole.Role.Seeker && GameManager_v2.Instance.CurrentState == GameManager_v2.GameState.Hiding)
            return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(h, v, 0f);
        transform.position += movement.normalized * moveSpeed * Time.deltaTime;

        AimAtMouse();

        if (Input.GetKeyDown(KeyCode.F) && grenadeAmount > 0)
        {
            ThrowGrenade();
            grenadeAmount--;
        }
    }
    private void ThrowGrenade()
    {
        Debug.Log("Intento tirar granada");

        if (playerRole.CurrentRole != PlayerRole.Role.Seeker)
        {
            Debug.Log("No soy Seeker");
            return;
        }

        if (GameManager_v2.Instance.CurrentState !=
            GameManager_v2.GameState.Seeking)
        {
            Debug.Log("Todavía no estamos en Seeking");
            return;
        }

        Debug.Log("Voy a instanciar la granada");

        PhotonNetwork.Instantiate(
            grenadePrefab.name,
            grenadeSpawnPoint.position,
            grenadeSpawnPoint.rotation
        );
    }
    private void AimAtMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        Vector2 direction = mousePosition - transform.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        grenadeSpawnPoint.rotation = Quaternion.Euler(
            0f,
            0f,
            angle - 90f
        );
    }
}