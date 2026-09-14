using UnityEngine;
using TMPro;

public class GameTimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    private void Update()
    {
        if (GameManager_v2.Instance == null || timerText == null) return;

        var state = GameManager_v2.Instance.CurrentState;

        if (state == GameManager_v2.GameState.Hiding)
        {
            float time = GameManager_v2.Instance.HidingTimeRemaining;
            timerText.text = "ESCONDIENDOSE: " + Mathf.CeilToInt(time).ToString() + "s";
        }
        else if (state == GameManager_v2.GameState.Seeking)
        {
            float time = GameManager_v2.Instance.SeekingTimeRemaining;
            timerText.text = "BUSCANDO: " + Mathf.CeilToInt(time).ToString() + "s";
        }
        else if (state == GameManager_v2.GameState.Waiting)
        {
            timerText.text = "Esperando jugadores...";
        }
        else if (state == GameManager_v2.GameState.GameOver)
        {
            timerText.text = "Juego Terminado!";
        }
    }
}