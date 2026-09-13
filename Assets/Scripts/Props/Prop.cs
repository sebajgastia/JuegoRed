using UnityEngine;
using UnityEngine.Pool;

public class Prop : MonoBehaviour
{
    [SerializeField] private bool destructible = true;

    public int PropId { get; private set; }

    private PropSpawner spawner;

    public void Initialize(int id, PropSpawner propSpawner)
    {
        PropId = id;
        spawner = propSpawner;
    }

    public bool IsDestructible()
    {
        return destructible;
    }

    public void DestroyProp()
    {
        if (!destructible)
            return;

        spawner.RequestDestroyProp(PropId);
    }

    public void DestroyPropLocal()
    {
        gameObject.SetActive(false);
    }
}