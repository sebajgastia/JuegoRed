using Photon.Pun;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float smoothSpeed = 5f;

    [Header("Límites del mapa")]
    [SerializeField] private float minX = -21.5f;
    [SerializeField] private float maxX = 24.5f;
    [SerializeField] private float minY = -8.5f;
    [SerializeField] private float maxY = 10.5f;

    private Transform target;
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (GameManager_v2.Instance == null)
            return;

        GameManager_v2.GameState state =
            GameManager_v2.Instance.CurrentState;

        if (state != GameManager_v2.GameState.Hiding &&
            state != GameManager_v2.GameState.Seeking)
        {
            return;
        }

        if (target == null)
        {
            FindLocalPlayer();
            return;
        }

        float cameraHalfHeight =
            cam.orthographicSize;

        float cameraHalfWidth =
            cameraHalfHeight * cam.aspect;

        float cameraX = Mathf.Clamp(
            target.position.x,
            minX + cameraHalfWidth,
            maxX - cameraHalfWidth
        );

        float cameraY = Mathf.Clamp(
            target.position.y,
            minY + cameraHalfHeight,
            maxY - cameraHalfHeight
        );

        Vector3 targetPosition = new Vector3(
            cameraX,
            cameraY,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );

        Debug.Log(
            "Camera Size: " + cameraHalfHeight +
            " | Y permitido: " +
            (minY + cameraHalfHeight) +
            " a " +
            (maxY - cameraHalfHeight)
        );
    }

    private void FindLocalPlayer()
    {
        PlayerRole[] players =
            FindObjectsOfType<PlayerRole>();

        foreach (PlayerRole player in players)
        {
            PhotonView view =
                player.GetComponent<PhotonView>();

            if (view != null && view.IsMine)
            {
                target = player.transform;

                Debug.Log(
                    "La cámara encontró al Player local: " +
                    player.gameObject.name
                );

                return;
            }
        }
    }
}