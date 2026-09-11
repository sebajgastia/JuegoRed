using UnityEngine;

public class MapProp : MonoBehaviour
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

        Destroy(gameObject);
    }
}