using UnityEngine;
using UnityEngine.Pool;

public class Prop : MonoBehaviour
{
    [SerializeField] private bool destructible = true;

    public bool IsDestructible()
    {
        return destructible;
    }

    public void DestroyProp()
    {
        if (!destructible)
            return;

        gameObject.SetActive(false);
    }
}