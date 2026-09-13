using UnityEngine;
using System;

public class PropSpawner : MonoBehaviour
{
    [SerializeField] private ObjectPool objectPool;
    [SerializeField] private Transform[] spawnPoints;
    public static event Action OnSpawnProps;

    private void Start()
    {
        TriggerSpawnProps();
    }

    public void SpawnProps()
    {
        Debug.Log("se spawnean los props");

        foreach (Transform spawnPoint in spawnPoints)
        {
            GameObject prop = objectPool.GetRandomObject();

            if (prop == null)
            {
                Debug.Log("null");
                continue;
            }

            prop.transform.position = spawnPoint.position;
            prop.transform.rotation = spawnPoint.rotation;

            prop.SetActive(true);
        }

    }

    private void OnEnable()
    {
        OnSpawnProps += SpawnProps;
    }

    private void OnDisable()
    {
        OnSpawnProps -= SpawnProps;
    }

    public static void TriggerSpawnProps()
    {
        OnSpawnProps?.Invoke();
    }
}