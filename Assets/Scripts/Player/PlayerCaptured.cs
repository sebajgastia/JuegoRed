using UnityEngine;

public class PlayerCaptured : MonoBehaviour
{
    public bool IsCaptured { get; private set; }

    public void Capture()
    {
        if (IsCaptured)
            return;

        IsCaptured = true;

        Debug.Log(
            gameObject.name +
            " fue capturado"
        );
    }
}
